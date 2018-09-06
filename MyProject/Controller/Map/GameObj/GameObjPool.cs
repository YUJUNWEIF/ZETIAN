using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class AsyncLoadGameObj : Util.IYieldInstruction
{
    public GameObject obj;
    public bool done { get; set; }
}

public class PrefabPool
{
    LinkedList<GameObject> m_used = new LinkedList<GameObject>();
    LinkedList<GameObject> m_cached = new LinkedList<GameObject>();
    const int cullCheckFreq = 20;
    public string key { get; private set; }
    public GameObject prefab { get; private set; }
    public int totalCount { get { return (m_used.Count + m_cached.Count); } }
    public bool fixedPool { get; private set; }
    long m_statistics;
    long m_statisticsTimes;
    public PrefabPool(string key, GameObject prefab, bool fixedPool)
    {
        this.prefab = prefab;
        this.key = key;
        this.fixedPool = fixedPool;
        //var obj = Spawn();
        //Despawn(obj);
    }
    public GameObject Spawn()
    {
        GameObject obj = null;
        if (m_cached.Count > 0)
        {
            obj = m_cached.First.Value;
            m_cached.RemoveFirst();
            obj.gameObject.SetActive(true);
        }
        else
        {
            if (prefab)
            {
                obj = Util.UnityHelper.CloneGameObjectNoInit(prefab);
                obj.name = key;
                DisableShadow(obj);
            }
        }
        if (obj)
        {
            m_used.AddLast(obj);
        }
        StatisticsAndCull();
        return obj;
    }
    void DisableShadow(GameObject obj)
    {
        var renders = obj.GetComponentsInChildren<Renderer>();
        for (int index = 0; index < renders.Length; ++index)
        {
            var it = renders[index];
            it.shadowCastingMode = ShadowCastingMode.Off;
            it.receiveShadows = false;
        }
    }
    public bool Despawn(GameObject obj)
    {
        bool contains = m_used.Contains(obj);
        if (obj && m_used.Contains(obj))
        {
            m_used.Remove(obj);
            m_cached.AddLast(obj);
            obj.transform.position = new Vector3(0, 0, -10000);
            //obj.layer = Util.TagLayers.Recycle;
            obj.gameObject.SetActive(false);
            //Util.UnityHelper.Hide(obj.transform);
            //Util.UnityHelper.CallUnInitialize(obj.transform);

            obj.transform.parent = null;
            //CullDespawned();
            return true;
        }
        return false;
    }
    void StatisticsAndCull()        //(k*(N+1)*N/2 + x*(x+1)/2) / (k* N + x) >= N/3
    {
        m_statistics += m_used.Count;
        ++m_statisticsTimes;
        if (m_statisticsTimes % cullCheckFreq == 0)
        {
            var averageUsed = m_statistics / m_statisticsTimes;
            if (averageUsed < totalCount / 3)
            {
                var cachedCount = averageUsed * 3 - m_used.Count;
                if (cachedCount < 0) { cachedCount = 1; }
                CullDespawned(cachedCount);
            }
        }
    }
    void CullDespawned(long cullAbove)
    {
        while (m_cached.Count > cullAbove)
        {
            var no = m_cached.Last;
            if (no.Value != null)
            {
                Util.UnityHelper.DestroyGameObjectNoUnInit(no.Value.gameObject);
                m_cached.Remove(no);
            }
        }
    }
    public void CullAllDespawned()
    {
        var no = m_cached.First;
        while (no != null)
        {
            Util.UnityHelper.DestroyGameObjectNoUnInit(no.Value.gameObject);
            no = no.Next;
        }
        m_cached.Clear();
    }
}
public class GameObjPool : Singleton<GameObjPool>
{
    class AsyncCreatePool : Util.IYieldInstruction
    {
        public bool done { get; set; }
    }
    FirstLevelCache m_firstCache;
    SecondLevelCache m_secondCache;
    
    Dictionary<string, AnimationClip[]> m_animClips = new Dictionary<string, AnimationClip[]>();
    Dictionary<string, AudioClip> m_audios = new Dictionary<string, AudioClip>();
    public Transform group { get; set; }
    public GameObjPool()
    {
        m_secondCache = new SecondLevelCache();
        m_firstCache = new FirstLevelCache(m_secondCache);
    }
    public GameObject GetPrefabSync(string assetPath, string assetName)
    {
        string fullPath = assetPath + assetName;
        PrefabPool pool = m_firstCache.Get(fullPath);
        if (pool == null)
        {
            pool = _CreatePoolSync(assetPath, assetName, fullPath);
        }
        return pool != null ? pool.prefab : null;
    }
    public GameObject CreateObjSync(string assetPath, string assetName)
    {
        string fullPath = assetPath + assetName;
        var obj = m_firstCache.CreateObj(fullPath);
        if (obj)
        {
            return obj;
        }
        _CreatePoolSync(assetPath, assetName, fullPath);
        return m_firstCache.CreateObj(fullPath);
    }
    public void DestroyObj(GameObject obj)
    {
        if (obj)
        {
            var cached = m_firstCache.DestroyObj(obj);
            if (!cached)
            {
                Util.UnityHelper.DestroyGameObjectNoUnInit(obj);
            }
        }
    }
    public void ClearPool()
    {
        m_secondCache.ClearPool();
    }
    public void CompressPool()
    {
        m_secondCache.CompressPool();
    }
    public bool ExistPool(string poolName)
    {
        return m_firstCache.Exist(poolName) ||
                  m_secondCache.Exist(poolName);
    }
    public Util.IYieldInstruction CreatePoolAsync(string assetPath, string assetName, bool fixedPool = false)
    {
        AsyncCreatePool callback = new AsyncCreatePool();
        if (string.IsNullOrEmpty(assetName) || assetName == @"0") { callback.done = true; }
        else
        {
            string fullPath = assetPath + assetName;
            if (ExistPool(fullPath))
            {
                callback.done = true;
            }
            else
            {
                BundleManager.Instance.LoadAsync(assetPath, assetName, fullPath,
                    obj =>
                    {
                        callback.done = true;
                        m_secondCache.Insert(fullPath, new PrefabPool(fullPath, obj as GameObject, fixedPool));
                    });
            }
        }
        return callback;
    }
    public void CreatePoolSync(string assetPath, string assetName, bool fixedPool = false)
    {
        _CreatePoolSync(assetPath, assetName, assetPath + assetName);
    }
    PrefabPool _CreatePoolSync(string assetPath, string assetName, string fullPath, bool fixedPool = false)
    {
        if (string.IsNullOrEmpty(assetName)) { return null; }
        var pool = m_firstCache.Get(fullPath);
        if (pool == null)
        {
            var prefab = BundleManager.Instance.LoadSync<GameObject>(assetPath, assetName, fullPath);
            m_secondCache.Insert(fullPath, pool = new PrefabPool(fullPath, prefab, fixedPool));
        }
        return pool;
    }
    public AudioClip LoadAudioSync(string assetPath, string assetName)
    {
        if (string.IsNullOrEmpty(assetName)) { return null; }
        string fullPath = assetPath + assetName;
        AudioClip clip;
        if (!m_audios.TryGetValue(fullPath, out clip))
        {
            clip = BundleManager.Instance.LoadSync<AudioClip>(assetPath, assetName, fullPath);
            m_audios.Add(fullPath, clip);
        }
        return clip;
    }
    public void AddClip(string name, AnimationClip[] clips)
    {
        if (!m_animClips.ContainsKey(name))
        {
            m_animClips.Add(name, clips);
        }
    }
    public AnimationClip[] LoadAnim(string name)
    {
        AnimationClip[] clip;
        if (!m_animClips.TryGetValue(name, out clip))
        {
            clip = BundleManager.Instance.LoadSyncAll<AnimationClip>(name);
            m_animClips.Add(name, clip);
        }
        return clip;
    }
}