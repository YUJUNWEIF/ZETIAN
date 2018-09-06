using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class SecondLevelCache : ICacheDataVisit<string, PrefabPool>, IPoolVisit<string, PrefabPool>
{
    Dictionary<string, PrefabPool> prefabpools = new Dictionary<string, PrefabPool>();
    public bool Exist(string key)
    {
        return prefabpools.ContainsKey(key);
    }
    public PrefabPool Get(string key)
    {
        if (string.IsNullOrEmpty(key)) { return null; }
        PrefabPool pool;
        if (prefabpools.TryGetValue(key, out pool))
        {
            prefabpools.Remove(key);
        }
        return pool;
    }
    public void Flush(string key, PrefabPool pool)
    {
        prefabpools.Add(key, pool);
    }
    public void Insert(string key, PrefabPool pool)
    {
        if (!prefabpools.ContainsKey(key))
        {
            prefabpools.Add(key, pool);
        }
    }
    public void ClearPool()
    {
        ClearSpecifyPools(new List<string>(prefabpools.Keys));
    }
    public void CompressPool()
    {
        List<string> recycle = new List<string>();
        foreach (var pool in prefabpools)
        {
            if (pool.Value.fixedPool) { continue; }
            pool.Value.CullAllDespawned();
            if (pool.Value.totalCount == 0) { recycle.Add(pool.Key); }
        }
        ClearSpecifyPools(recycle);
    }
    void ClearSpecifyPools(List<string> recycle)
    {
        for (int index = 0; index < recycle.Count; ++index)
        {
            var name = recycle[index];
            PrefabPool pool;
            if (prefabpools.TryGetValue(name, out pool))
            {
                prefabpools.Remove(name);
                pool.CullAllDespawned();
            }
            BundleManager.Instance.UnloadAsset(name);
        }
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}