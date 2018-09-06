using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace geniusbaby
{
    public enum LANG_ID
    {
        English = 1,

    }
    public struct LGAP
    {
        public byte langId;
        public byte gradeId;
        public byte areaId;
        public byte packageId;

        public int Encode() { return Encode(this); }
        public static int Encode(LGAP lgap)
        {
            return (lgap.langId << 24) + (lgap.gradeId << 16) + (lgap.areaId << 8) + lgap.packageId;
        }
        public static LGAP Decode(int lgapId)
        {
            return new LGAP()
            {
                langId = (byte)((lgapId >> 24) & 0xFF),
                gradeId = (byte)((lgapId >> 16) & 0xFF),
                areaId = (byte)((lgapId >> 8) & 0xFF),
                packageId = (byte)((lgapId >> 0) & 0xFF),
            };
        }
        public static int LangId(int lgfId) { return (lgfId >> 24) & 0xFF; }
        public static int GradeId(int lgfId) { return (lgfId >> 16) & 0xFF; }
        public static int CategoryId(int lgfId) { return (lgfId >> 8) & 0xFF; }
        public static int PackageId(int lgfId) { return (lgfId >> 0) & 0xFF; }
        public string MakeFileName()
        {
            return new StringBuilder().Append(langId).Append('_').Append(gradeId).Append('_').Append(areaId).Append(".zip").ToString();
        }
        public bool empty { get { return langId == 0 && gradeId == 0 && areaId == 0 && packageId == 0; } }
    }

    public class GAPInfo
    {
        public class _areaInfo
        {
            public int areaId;
            public string name;
            public string file;
            public string md5;
            public int size;
        }
        [Util.PrimaryKeyAttribute]
        public int gradeId;
        public string name;
        public List<_areaInfo> areaInfos = new List<_areaInfo>();
    }
    public class LGAPManager : config.CSVTable<LGAPManager, int, GAPInfo>
    {
        IFileReader m_reader;
        string m_curAreaZip;
        Dictionary<string, List<cfg.wordClass>> m_gradeClasses = new Dictionary<string, List<cfg.wordClass>>();
        Util.CoroutineHelper coroutineHelper = new Util.CoroutineHelper();
        
        public Util.ParamActions onPackageSync = new Util.ParamActions();

        public AssetMetaManifest localLGAPInfo = new AssetMetaManifest(GamePath.manifest.lgap);
        public void LoadLGAPInfo(IFileReader reader)
        {
            this.m_reader = reader;
            var path = Path.Combine(UnityEngine.Application.persistentDataPath, localLGAPInfo.meteInfoFile);
            var bytes = File.ReadAllBytes(path);
            var length = Util.DependencyUtil.zlibDecompressBuffered(bytes, 0, bytes.Length);
            localLGAPInfo.UnMarsh(Util.DependencyUtil.buffer, 0);
        }
        public void SaveLGAPInfo()
        {
            var bytes = Util.PoolByteArrayAlloc.alloc.Alloc(1024 * 4);
            var pos = localLGAPInfo.Marsh(bytes, 0);
            if (pos <= 0) { return; }

            var length = Util.DependencyUtil.zlibCompressBuffered(bytes, 0, pos);
            var path = Path.Combine(UnityEngine.Application.persistentDataPath, localLGAPInfo.meteInfoFile);
            File.Delete(path);
            var fs = File.Open(path, FileMode.CreateNew);
            fs.Write(Util.DependencyUtil.buffer, 0, length);
            fs.Close();
        }
        string m_fileGap;
        public void DownloadGAPInfo(string fileName, Action after = null)
        {
            if (string.IsNullOrEmpty(m_fileGap))
            {
                coroutineHelper.StartCoroutineImmediate(GetRemoteGAPInfo(fileName, after));
            }
        }
        public IEnumerator GetRemoteGAPInfo(string fileName, Action after)
        {
            GuiManager.Inst().ShowFrame(typeof(LSharpScript.NetWatchFrame).Name);
            var url = new StringBuilder(GamePath.url.updateURL).Append("knowledge/english/").Append(fileName);
            yield return DownloadManager.Instance.DownloadFileWithTimeout(url.ToString(), 5,
                (www, succeed) =>
                {
                    if (succeed)
                    {
                        LGAPManager.Inst().RecordArray.Clear();
                        LGAPManager.Inst().RecordDict.Clear();

                        var os = new MemoryStream(www.bytes);
                        LGAPManager.Inst().UnMarsh(new BinaryReader(os));

                        m_fileGap = fileName;
                        if (after != null) after();
                    }
                });
            GuiManager.Inst().HideFrame(typeof(LSharpScript.NetWatchFrame).Name);
        }
        public void ParseAfterDownload(LGAP lgap)
        {
            if (lgap.empty) { return; }

            var localFile = lgap.MakeFileName();
            var localUrl = Path.Combine(Application.persistentDataPath, localFile);
            if (File.Exists(localUrl)) { LGAPManager.Inst().ParseAll(localFile); }
            else
            {
                var remoteGradeInfo = Find(it => it.gradeId == lgap.gradeId);
                var remoteAreaInfo = remoteGradeInfo.areaInfos.Find(it => it.areaId == lgap.areaId);

                var frame = GuiManager.Inst().ShowFrame(typeof(LSharpScript.DownloadFileFrame).Name);
                var script = LSharpScript.T.As<LSharpScript.DownloadFileFrame>(frame);
                script.Download(new pps.ProtoAssetMetaInfo() { file = "knowledge/english/" + remoteAreaInfo.file, md5 = remoteAreaInfo.md5, size = remoteAreaInfo.size }, localFile, () =>
                {
                    var hash = localLGAPInfo.hashs.Find(it => it.name == localFile);
                    if (hash != null)
                    {
                        hash.md5 = remoteAreaInfo.md5;
                        hash.size = remoteAreaInfo.size;
                    }
                    else
                    {
                        localLGAPInfo.hashs.Add(new AssetMetaInfo() { name = localFile, md5 = remoteAreaInfo.md5, size = remoteAreaInfo.size });
                    }
                    SaveLGAPInfo();
                    ParseAll(localFile);
                });
            }
        }
        public void ParseAll(string areaZip)
        {
            if (m_curAreaZip == areaZip) { return; }

            m_curAreaZip = areaZip;
            m_gradeClasses.Clear();
            tab.__title.Inst().RecordArray.Clear();
            tab.__title.Inst().RecordDict.Clear();

            FindZip(areaZip, (file, bytes) =>
            {
                var os = new MemoryStream(bytes);
                if (file.StartsWith("__title"))
                {
                    tab.__title.Inst().UnMarsh(new BinaryReader(os));
                }
                else
                {
                    var array = DeBinary.Deserializer.Deserialize<List<cfg.wordClass>>(new BinaryReader(os));
                    m_gradeClasses.Add(file, array);
                }
            });
            onPackageSync.Fire();
        }
        public List<cfg.wordClass> FindLGAP(int lgapId)
        {
            var lgap = LGAP.Decode(lgapId);
            var titleCfg = tab.__title.Inst().Find(it => it.id == lgap.packageId);
            if (titleCfg == null) { return null; }

            List<cfg.wordClass> classes;
            m_gradeClasses.TryGetValue(titleCfg.file, out classes);
            return classes;
        }
        public cfg.wordClass FindClass(int lgcpId, int classId)
        {
            var package = FindLGAP(lgcpId);
            return package.Find(it => it.id == classId);
        }
        void FindZip(string zipFile, string file, Action<byte[]> parser)
        {
            var ms = new MemoryStream(m_reader.Load(zipFile));
            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ms))
            {
                ICSharpCode.SharpZipLib.Zip.ZipEntry theEntry = null;
                while ((theEntry = zip.GetNextEntry()) != null)
                {
                    if (theEntry.Name != file) { continue; }
                    var fdata = new byte[theEntry.Size];
                    Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
                    parser(fdata);
                    break;
                }
            }
        }
        void FindZip(string zipFile, Action<string, byte[]> parser)
        {
            var ms = new MemoryStream(m_reader.Load(zipFile));
            using (var zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ms))
            {
                ICSharpCode.SharpZipLib.Zip.ZipEntry theEntry = null;
                byte[] buffer = new byte[2048 * 4];
                while ((theEntry = zip.GetNextEntry()) != null)
                {
                    var fdata = new byte[theEntry.Size];
                    Util.DependencyUtil.ZipEntryUncompress(zip, theEntry, fdata);
                    parser(theEntry.Name, fdata);
                }
            }
        }
    }
}
