using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using MiniJSON;
using DeJson;
using Util;

public class DownloadManager : Singleton<DownloadManager>
{
    public enum State
    {
        Unknown,
        Checking,
        Downloading
    }
    public enum DownloadState
    {
        Unknown,
        Download,
        CheckSum,
        Save,
    }
    public enum Code
    {
        Unknown,
        Normal,
        ReadError,
        CheckSumError,
        WriteError,
        InvalidClient,
    }
    interface IThreadCallBack
    {
        void Work(object x);
        bool done { get; }
    }
    class CaculateMd5 : IThreadCallBack
    {
        byte[] m_bytes;
        public volatile string md5;
        public CaculateMd5(byte[] bytes) { m_bytes = bytes; }
        public void Work(object x) { md5 = Crypt.GetMD5_32(m_bytes); }
        public bool done { get { return md5 != null; } }
    }
    class SaveFile : IThreadCallBack
    {
        byte[] bytes;
        string fullPath;
        public volatile Code status;
        public SaveFile(byte[] bytes, string fullPath)
        {
            this.bytes = bytes;
            this.fullPath = fullPath;
        }
        public void Work(object x)
        {
            status = Code.Unknown;
            try
            {
                File.WriteAllBytes(fullPath, bytes);
                status = Code.Normal;
            }
            catch (System.Exception) { status = Code.WriteError; }
        }
        public bool done { get { return status != Code.Unknown; } }
    }
    public Param1Actions<AssetMetaInfo> onDownload = new Param1Actions<AssetMetaInfo>();
    public ParamActions onDownloadState = new ParamActions();
    public ParamActions onCode = new ParamActions();

    private State m_state = State.Unknown;
    private DownloadState m_downloadState = DownloadState.Unknown;
    private Code m_code = Code.Unknown;
    public RangeValue progress;
    public State state { get { return m_state; } }
    public DownloadState downloadState
    {
        get { return m_downloadState; }
        private set { m_downloadState = value; onDownloadState.Fire(); }
    }
    public Code code
    {
        get { return m_code; }
        private set { m_code = value; onCode.Fire(); }
    }
    AssetMetaManifest cfglocal;
    AssetMetaManifest artlocal;
    AssetMetaManifest cfgremote;
    AssetMetaManifest artremote;
    public IEnumerator StartUp(AssetMetaManifest cfg, AssetMetaManifest art)
    {
        if (!Directory.Exists(Application.persistentDataPath)) { Directory.CreateDirectory(Application.persistentDataPath); }
        m_state = State.Checking;
        m_downloadState = DownloadState.Unknown;
        m_code = Code.Normal;
        cfglocal = cfg;
        artlocal = art;
        cfgremote = new AssetMetaManifest(cfglocal.meteInfoFile);
        artremote = new AssetMetaManifest(artlocal.meteInfoFile);

        yield return DownloadFileWithTimeout(GamePath.url.updateURL + cfglocal.meteInfoFile, 5f,
            (www, succeed) =>
            {
                if (succeed) { cfgremote.UnMarsh(www.bytes, 0); }
                else { code = Code.ReadError; }
            });
        yield return DownloadFileWithTimeout(GamePath.url.updateURL + artlocal.meteInfoFile, 5f,
            (www, succeed) =>
            {
                if (succeed) { artremote.UnMarsh(www.bytes, 0); }
                else { code = Code.ReadError; }
            });
        if (code != Code.Normal) { yield break; }

        var cfgUps = cfgremote.Substract(cfglocal);
        var artUps = artremote.Substract(artlocal);
        if (artUps.Count > 0)
        {
            yield return DownloadFileWithTimeout(GamePath.url.updateURL + artlocal.name, 5f,
                (www, succeed) =>
                {
                    if (succeed)
                    {
                        var fullPath = Path.Combine(Application.persistentDataPath, artlocal.name);
                        if (File.Exists(fullPath)) { File.Delete(fullPath); }
                        File.WriteAllBytes(fullPath, www.bytes);
                    }
                    else
                    {
                        code = Code.ReadError;
                    }
                });
        }

        progress = new RangeValue(0, GetUpSize(cfgUps) + GetUpSize(artUps));
        m_state = State.Downloading;
        if (cfgUps.Count > 0) { yield return UpdateCfg(cfglocal, cfgremote, cfgUps); }
        if (artUps.Count > 0) { yield return UpdateCfg(artlocal, artremote, artUps); }
    }
    int GetUpSize(List<AssetMetaInfo> ups)
    {
        int max = 0;
        for (int index = 0; index < ups.Count; ++index) { max += ups[index].size; }
        return max;
    }
    public IEnumerator DownloadFileWithTimeout(string URL, float timeOut, Action<WWW, bool> after, int resSize = 0)
    {
        if (code != Code.Normal) { yield break; }

        WWW www = new WWW(URL);
        float timer = 0;
        bool failed = false;
        float lastProgress = www.progress;
        var downloadSize = progress.current;
        while (!www.isDone)
        {
            timer += Time.deltaTime;
            if (timer > timeOut)
            {
                if (www.progress - lastProgress < 0.01f)
                {
                    failed = true;
                    break;
                }
                else
                {
                    lastProgress = www.progress;
                    timer = 0f;
                }
            }
            progress.current = downloadSize + (int)(www.progress * resSize);
            yield return null;
        }
        after(www, !failed && string.IsNullOrEmpty(www.error));
        www.Dispose();
    }
    public IEnumerator UpdateCfg(AssetMetaManifest local, AssetMetaManifest remote, List<AssetMetaInfo> ups)
    {
        if (code != Code.Normal) { yield break; }
        if (ups.Count > 0)
        {
            for (int index = 0; index < ups.Count; ++index)
            {
                var up = ups[index];
                yield return DownloadFromUrl(Path.Combine(Application.persistentDataPath, up.name), GamePath.url.updateURL + up.name, up.md5);
                if (code == Code.Normal)
                {
                    local.Update(up);
                }
                else
                {
                    break;
                }
            }
            if (code == Code.Normal) { SaveMeta(remote); }
        }
    }
    void SaveMeta(AssetMetaManifest now)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, now.name);
        if (File.Exists(fullPath)) { File.Delete(fullPath); }
        var buffer = Util.PoolByteArrayAlloc.alloc.Alloc(4 * 1024);
        var pos = now.Marsh(buffer, 0);
        var fs = File.Open(fullPath, FileMode.CreateNew);
        fs.Write(buffer, 0, pos);
        fs.Close();
    }
    public IEnumerator DownloadFromUrl(string finalFilePath, string url, string md5)
    {
        byte[] bytes = null;
        yield return DownloadFileWithTimeout(url, 5f, (www, succeed) =>
        {
            if (succeed)
            {
                bytes = www.bytes;
            }
            else
            {
                code = Code.ReadError;
            }
        });
        if (code != Code.Normal) { yield break; }

        downloadState = DownloadState.CheckSum;
        var caculateMd5 = new CaculateMd5(bytes);
        yield return AsyncProcess(caculateMd5);
        if (md5 == null || md5 == caculateMd5.md5)
        {
            downloadState = DownloadState.Save;
            if (File.Exists(finalFilePath)) { File.Delete(finalFilePath); }
            string parentFolder = finalFilePath.Substring(0, finalFilePath.LastIndexOf("/"));
            if (!Directory.Exists(parentFolder)) { Directory.CreateDirectory(parentFolder); }
            var saveFile = new SaveFile(bytes, finalFilePath);
            yield return AsyncProcess(saveFile);
            code = saveFile.status;
        }
        else
        {
            code = Code.CheckSumError;
        }
    }
    IEnumerator AsyncProcess(IThreadCallBack calback)
    {
        ThreadPool.QueueUserWorkItem(calback.Work, calback);
        while (!calback.done) { yield return null; }
    }
}