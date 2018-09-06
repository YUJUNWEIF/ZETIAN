using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class FirstLevelCache : IPoolVisit<string, PrefabPool>
{
    Util.LRUCache<string, PrefabPool> m_cache;
    SecondLevelCache m_secondCache;
    public FirstLevelCache(SecondLevelCache secondCache, int cacheSize = 60)
    {
        m_secondCache = secondCache;
        m_cache = new Util.LRUCache<string, PrefabPool>(cacheSize);
        m_cache.popObjListener.Add(pop =>
        {
            m_secondCache.Flush(pop.key, pop);
        });
    }
    public bool Exist(string poolName)
    {
        return m_cache.ContainsKey(poolName);
    }
    public PrefabPool Get(string key)
    {
        var pool = m_cache.Get(key);
        if (pool == null)
        {
            pool = m_secondCache.Get(key);
            if (pool != null) { m_cache.Insert(key, pool); }
        }
        return pool;
    }
    public GameObject CreateObj(string key)
    {
        var pool = Get(key);
        if (pool != null)
        {
            return pool.Spawn();
        }
        return null;
    }
    public bool DestroyObj(GameObject obj)
    {
        if (obj)
        {
            var pool = Get(obj.name);
            if (pool != null) { return pool.Despawn(obj); }
        }
        return false;
    }
    public void ClearPool()
    {
        var all = m_cache.GetAll();
        for (int index = 0; index < all.Count; ++index)
        {
            var pool = all[index];
            m_secondCache.Flush(pool.key, pool);
        }
        m_cache.DeleteAll();
    }
    public void CompressPool()
    {
        List<string> recycle = new List<string>();
        var all = m_cache.GetAll();
        for (int index = 0; index < all.Count; ++index)
        {
            var pool = all[index];
            pool.CullAllDespawned();
            if (pool.totalCount == 0) { recycle.Add(pool.key); }
        }        
        ClearSpecifyPools(recycle);
    }
    void ClearSpecifyPools(List<string> recycle)
    {
        for (int index = 0; index < recycle.Count; ++index)
        {
            var pool = m_cache.Get(recycle[index]);
            m_secondCache.Flush(pool.key, pool);
            m_cache.Remove(pool.key);
        }
    }
}