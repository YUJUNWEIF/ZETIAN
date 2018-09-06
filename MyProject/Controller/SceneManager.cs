using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace geniusbaby
{
    public struct EntityId
    {
        public int type;
        public int uniqueId;
        public EntityId(int type, int uId) { this.type = type; this.uniqueId = uId; }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(EntityId)) { return this == (EntityId)obj; }
            return false;
        }
        public override int GetHashCode() { return ((int)type << 28) | (uniqueId.GetHashCode() & 0x0FFFFFFF); }
        public static bool operator ==(EntityId x, EntityId y)
        {
            return x.type == y.type && x.uniqueId == y.uniqueId;
        }
        public static bool operator !=(EntityId x, EntityId y)
        {
            return !(x == y);
        }
        public static EntityId Empty { get { return default(EntityId); } }
        public static implicit operator bool(EntityId foo) { return foo.type != 0; }
    }
    public interface ISceneManager
    {
        void Enter();
        void Leave();
    }
    public class SceneManager : Singleton<SceneManager>
    {
        struct loading
        {
            const float duration = 2f;
            const float delay = 0.3f;
            int m_delayPercent;
            float m_startTime;
            float m_completeTime;
            float m_endTime;
            public void Start()
            {
                m_delayPercent = UnityEngine.Random.Range(80, 95);
                m_startTime = Time.time;
                m_completeTime = -1f;
                m_endTime = -1f;
            }
            public void Complete()
            {
                m_completeTime = Time.time;
                m_endTime = m_completeTime + delay;
            }
            public void Reset()
            {
                m_completeTime = -1;
                m_delayPercent = 0;
            }
            public int Percent()
            {
                if (m_completeTime > 0 && Time.time > m_completeTime)
                {
                    var percent = Mathf.Min(Mathf.RoundToInt(m_completeTime * 100 / duration), m_delayPercent);
                    return percent + Mathf.RoundToInt((100 - percent) * (Time.time - m_completeTime) / delay);
                }
                else
                {
                    var percent = 100 * (Time.time - m_startTime)/ duration;
                    if (percent > m_delayPercent) percent = m_delayPercent;
                    return Mathf.RoundToInt(percent);
                }
            }
        }
        string mapName;
        loading m_loading;
        public Util.MapVector<EntityId, IBaseObj> m_objs = new Util.MapVector<EntityId, IBaseObj>(Util.PoolObjAlloc<Util.MapVector<EntityId, IBaseObj>.Node>.alloc, null);
        public Util.CoroutineHelper coroutineHelper = new Util.CoroutineHelper();
        public Util.ParamActions onComplete = new Util.ParamActions();
        public delegate IEnumerator BeforeLoaded();
        public delegate IEnumerator AfterLoader();
        public TerrainParam terrain { get; private set; }
        public int LoadingProgress() { return m_loading.Percent(); }
        public void StartGame() { }
        public void StopGame() { }
        public void EnterMap(string mapName, BeforeLoaded beforeLoaded, AfterLoader prepareLoader)
        {
            coroutineHelper.StartCoroutineImmediate(CreateMap(mapName, beforeLoaded, prepareLoader));            
        }
        public void LeaveMap()
        {
            coroutineHelper.StopAll();
            KeepSceneFree();
            FreeScene();
            this.mapName = string.Empty;
            //UnityEngine.RenderSettings.fog = false;
        }
        public void KeepSceneFree()
        {
            m_objs.ForEach(it =>
            {
                if (it.Value)
                {
                    it.Value.UnInitialize();
                    GameObjPool.Instance.DestroyObj(it.Value.gameObject);
                }
            });
            m_objs.Clear();
            GameObjPool.Instance.CompressPool();
        }
        void FreeScene()
        {
            if (terrain != null) { GameObjPool.Instance.DestroyObj(terrain.gameObject); terrain = null; }
        }
        IEnumerator CreateMap(string mapName, BeforeLoaded beforeLoaded, AfterLoader prepareLoader)
        {
            m_loading.Reset();
            m_loading.Start();
            if (this.mapName != mapName)
            {
                FreeScene();
                KeepSceneFree();
                this.mapName = mapName;

                if (beforeLoaded != null) { yield return beforeLoaded(); }

                yield return GameObjPool.Instance.CreatePoolAsync(GamePath.asset.terrain, mapName);
                var obj = GameObjPool.Instance.CreateObjSync(GamePath.asset.terrain, mapName);
                obj.transform.position = Vector3.zero;
                //Util.UnityHelper.SetLayerRecursively(obj.transform, Util.TagLayers.Terrain);
                terrain = obj.GetComponent<TerrainParam>();
                obj.gameObject.SetActive(true);
                CameraControl.Instance.Attach(terrain);
                if (prepareLoader != null) { yield return prepareLoader(); }
            }
            else
            {
                FreeScene();
                if (beforeLoaded != null) { yield return beforeLoaded(); }
                if (prepareLoader != null) { yield return prepareLoader(); }
            }
            m_loading.Complete();
            while (m_loading.Percent() <= 100) { yield return null; }
            onComplete.Fire();
            //    UnityEngine.RenderSettings.fog = true;
        }
        public void AddObj(IBaseObj script, Transform parent = null, bool useParentLayer = false)
        {
            //Util.UnityHelper.Show(script, parent, useParentLayer);
            script.gameObject.SetActive(true);
            m_objs.Add(script.entityId, script);
        }
        public void AddObj(IBaseObj script, Vector3 position)
        {
            script.transform.position = position;
            script.transform.rotation = Quaternion.identity;
            script.transform.localScale = Vector3.one;
            //Util.UnityHelper.Show(script);
            script.gameObject.SetActive(false);
            m_objs.Add(script.entityId, script);
        }
        public IBaseObj RmvObj(EntityId eId)
        {
            return m_objs.Remove(eId);
        }
        public T GetAs<T>(EntityId objId) where T : IBaseObj
        {
            return m_objs.Get(objId) as T;
        }
        public List<T> FindAll<T>(Predicate<T> predicate) where T : IBaseObj
        {
            var results = new List<T>();
            m_objs.FindAll(it =>
            {
                var obj = it as T;
                return (obj != null && predicate(obj));
            });
            return results;
        }
        public int GetLoadingProgress() { return m_loading.Percent(); }
    }
}