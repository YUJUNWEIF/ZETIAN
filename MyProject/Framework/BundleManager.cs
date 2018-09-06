using UnityEngine;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BundleManager : Singleton<BundleManager>
{
    public class LoadingTask
    {
        public string assetPath { get; private set; }
        public string assetName { get; private set; }
        public string assetFullpath { get; private set; }
        public bool isAssetBundle { get; private set; }
        public AsyncOperation asyncLoad { get; set; }
        public Util.Param1Actions<Object> afterLoaded = new Util.Param1Actions<Object>();
        public LoadingTask(string path, string name, string fullpath, System.Action<Object> listener, bool isBundle)
        {
            assetPath = path;
            assetName = name;
            assetFullpath = fullpath;
            isAssetBundle = isBundle;
            afterLoaded.Add(listener);
        }
    }
    Util.CoroutineHelper m_coroutineHelper = new Util.CoroutineHelper();
    Util.LRUCache<string, Object> m_loadedAsset = new Util.LRUCache<string, Object>(50);
    Dictionary<string, AssetBundle> m_loadedBundle = new Dictionary<string, AssetBundle>();
    List<LoadingTask> m_loadingQueue = new List<LoadingTask>();
    AssetBundleManifest manifest;
    string bundlePath;
    Dictionary<string, Hash128> m_hashs = new Dictionary<string, Hash128>();

    public LoadingTask currentTask { get; private set; }
    public BundleManager()
    {
        m_coroutineHelper.StartCoroutine(Update());
    }
    public void Bind(AssetBundleManifest manifest, string bundlePath)
    {
        this.manifest = manifest;
        this.bundlePath = bundlePath;
        var hashs = manifest.GetAllAssetBundles();
        for (int index = 0; index < hashs.Length; ++index)
        {
            var hash = manifest.GetAssetBundleHash(hashs[index]);
            m_hashs.Add(hashs[index], hash);
        }
    }
    public void ClearAll()
    {
        foreach (var bundle in m_loadedBundle) { bundle.Value.Unload(true); }
        m_loadedBundle.Clear();
        m_loadedAsset.DeleteAll();
    }
    public Object[] LoadUnitySprites(string spritePath)
    {
        spritePath = spritePath.Replace("file://", string.Empty);
        Object[] resObj = null;
        AssetBundle bundle;
        if (!m_loadedBundle.TryGetValue(spritePath, out bundle))
        {
            try
            {
                if (File.Exists(spritePath))
                {
                    bundle = AssetBundle.LoadFromMemory(File.ReadAllBytes(spritePath));
                    if (bundle != null) { m_loadedBundle.Add(spritePath, bundle); }
                }
            }
            catch { Util.Logger.Instance.Error(@"Read asset error : " + spritePath); }
        }
        if (bundle != null)
        {
            resObj = bundle.LoadAllAssets(typeof(Sprite));
            bundle.Unload(false);
        }
        else
        {
            resObj = Resources.LoadAll(spritePath, typeof(Sprite));
        }
        return resObj;
    }
    public T LoadSync<T>(string assetPath, string assetName) where T : Object
    {
        return LoadSync<T>(assetPath, assetName, assetPath + assetName);
    }
    public T LoadSync<T>(string assetPath, string assetName, string fullpath) where T : Object
    {
        const string prefix = "file://";
        if (assetPath.Contains(prefix)) { assetPath = assetPath.Replace(prefix, string.Empty); }

        var resObj = m_loadedAsset.Get(fullpath) as T;
        if (resObj != null) { return resObj; }
        AssetBundle bundle;
        if (!m_loadedBundle.TryGetValue(assetPath, out bundle))
        {
            try
            {
                if (File.Exists(assetPath))
                {
                    bundle = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetPath));
                    if (bundle != null) { m_loadedBundle.Add(assetPath, bundle); }
                }
            }
            catch { Util.Logger.Instance.Error(@"Read asset error : " + assetPath); }
        }
        if (bundle != null)
        {
            resObj = bundle.LoadAsset<T>(assetName);
            if (resObj == null) { resObj = bundle.mainAsset as T; }
            bundle.Unload(false);
            //m_loadedAsset.Insert(key, resObj);
        }
        else
        {
            resObj = Resources.Load<T>(fullpath);
            //if (resObj != null) { m_loadedAsset.Insert(key, resObj); }
        }
        return resObj;
    }
    public T[] LoadSyncAll<T>(string assetPath) where T : Object
    {
        AssetBundle bundle;
        if (!m_loadedBundle.TryGetValue(assetPath, out bundle))
        {
            try
            {
                if (File.Exists(assetPath))
                {
                    bundle = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetPath));
                    if (bundle != null) { m_loadedBundle.Add(assetPath, bundle); }
                }
            }
            catch { Util.Logger.Instance.Error(@"Read asset error : " + assetPath); }
        }
        T[] resObj = null;
        if (bundle != null)
        {
            resObj = bundle.LoadAllAssets<T>();
            bundle.Unload(false);
        }
        else
        {
            resObj = Resources.LoadAll<T>(assetPath);
        }
        return resObj;
    }
    public void LoadAsync(string assetPath, string assetName, System.Action<Object> eventReceiver)
    {
        LoadAsync(assetPath, assetName, assetPath + assetName, eventReceiver);
    }
    public void LoadAsync(string assetPath, string assetName, string fullpath, System.Action<Object> eventReceiver)
    {
        if (eventReceiver == null) { return; }
        
        var resObj = m_loadedAsset.Get(fullpath);
        if (resObj != null)
        {
            eventReceiver(resObj);
        }
        else
        {
            var lq = m_loadingQueue.Find(q => q.assetPath == assetPath);
            if (lq != null) { lq.afterLoaded.Add(eventReceiver); }
            else
            {
                if (!string.IsNullOrEmpty(assetName) && assetName != "0" && assetName != " ")
                {
                    m_loadingQueue.Add(new LoadingTask(assetPath, assetName, fullpath, eventReceiver, false));
                }
                else
                {
                    eventReceiver(null);
                }
            }
        }
    }
    public void CancelAsyncLoad(string assetPath, System.Action<Object> eventReceiver)
    {
        var result = m_loadingQueue.Find(queue => queue.assetPath == assetPath);
        if (result != null)
        {
            result.afterLoaded.Rmv(eventReceiver);
            if (result.afterLoaded.IsEmpty()) { m_loadingQueue.Remove(result); }
        }
    }
    IEnumerator ActualLoadAsync(LoadingTask lt)
    {
        if (lt.isAssetBundle)
        {
            string[] depends = manifest.GetAllDependencies(lt.assetPath);
            AssetBundle[] dependsAssetBundle = new AssetBundle[depends.Length];

            for (int index = 0; index < depends.Length; index++)
            {
                using (WWW dwww = new WWW(Path.Combine(bundlePath, depends[index])))
                {
                    yield return dwww;
                    dependsAssetBundle[index] = dwww.assetBundle;
                }
            }
            using (WWW www = new WWW(Path.Combine(bundlePath, lt.assetPath)))
            {
                yield return www;
                if (www.error == null)
                {
                    AssetBundle assetBundle = www.assetBundle;
                    var loadObj = assetBundle.LoadAssetAsync(lt.assetName);
                    lt.asyncLoad = loadObj;
                    yield return loadObj;
                    var obj = loadObj.asset;
                    if (obj == null) { obj = assetBundle.mainAsset; }
                    assetBundle.Unload(false);
                    if (obj != null)
                    {
                        m_loadedAsset.Insert(lt.assetFullpath, obj);
                        lt.afterLoaded.Fire(obj);
                    }
                }
            }
            //卸载依赖文件的包    
            for (int index = 0; index < depends.Length; index++)
            {
                dependsAssetBundle[index].Unload(false);
            }
        }
        else
        {
            var loadObj = Resources.LoadAsync(lt.assetFullpath);
            lt.asyncLoad = loadObj;
            yield return loadObj;
            var obj = loadObj.asset;
            if (obj != null)
            {
                m_loadedAsset.Insert(lt.assetPath, obj);
                lt.afterLoaded.Fire(obj);
            }
        }
    }
    public void GCClear()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    public void UnloadAsset(string bundlePath)
    {
        //bug remain here!!!!
        AssetBundle bundle;
        if (m_loadedBundle.TryGetValue(bundlePath, out bundle))
        {
            bundle.Unload(true);
            m_loadedBundle.Remove(bundlePath);
        }
    }
    IEnumerator Update()
    {
        while (true)
        {
            if (m_loadingQueue.Count > 0)
            {
                currentTask = m_loadingQueue[0];
                m_loadingQueue.RemoveAt(0);
                yield return ActualLoadAsync(currentTask);
            }
            else
            {
                yield return null;
            }
        }
    }
}

