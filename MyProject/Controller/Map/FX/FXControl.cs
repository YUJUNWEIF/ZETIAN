using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class FXControl : MonoBehaviour
    {
        private Util.CoroutineHelper coroutineHelper = new Util.CoroutineHelper();
        private Animator m_animator;
        private Animation m_animation;
        private List<ParticleSystem> m_particles = new List<ParticleSystem>();
        private List<TrailRenderer> m_trails = new List<TrailRenderer>();
        public bool canSuspend = true;
        public float duration = 1f;
        //public bool rotateWhenFly = true;
        public bool rotateWhenNew = true;
        //public bool playOnAwake = false;
        public FXActiveState State { get; private set; }
        public bool autoDestroy { get; private set; }
        void Awake()
        {
            var components = GetComponentsInChildren<Component>(false);
            for (int index = 0; index < components.Length; ++index)
            {
                var comp = components[index];

                var trail = comp as TrailRenderer;
                if (trail)
                {
                    m_trails.Add(trail);
                    continue;
                }
                var render = comp as Renderer;
                if (render)
                {
                    if (render.enabled)
                    {
                        render.shadowCastingMode = ShadowCastingMode.Off;
                        render.receiveShadows = false;
                    }
                    continue;
                }
                var animator = comp as Animator;
                if (animator)
                {
                    m_animator = animator;
                    //m_animator.enabled = true;
                    continue;
                }
                var animation = comp as Animation;
                if (animation)
                {
                    m_animation = animation;
                    continue;
                }
                var particle = comp as ParticleSystem;
                if (particle)
                {
                    //particle.playOnAwake = playOnAwake;
                    m_particles.Add(particle);
                    continue;
                }
            }
        }
        public void Play(Transform parent)
        {
            Play(parent, Vector3.zero);
        }
        public void Play(Transform parent, Vector3 offset)
        {
            if (parent)
            {
                Util.UnityHelper.ShowAsChild(transform, parent);
            }
            else
            {
                gameObject.SetActive(true);
            }
            transform.localPosition = offset;

            if (!rotateWhenNew) { transform.rotation = Quaternion.identity; }
            //if (m_animator)
            //{
            //    m_animator.enabled = true;
            //}
            if (m_animation)
            {
                if (m_animation.gameObject.activeInHierarchy)
                {
                    if (m_animation.isPlaying) { m_animation.Stop(); }
                    m_animation.Play();
                }
            }
            for (int index = 0; index < m_particles.Count; ++index)
            {
                var it = m_particles[index];
                //it.enableEmission = true;
                if (it.gameObject.activeInHierarchy)
                {
                    if (it.isPlaying) { it.Stop(); }
                    it.Play();
                }
            }
            for (int index = 0; index < m_trails.Count; ++index)
            {
                var it = m_trails[index];
                if (it.enabled && !it.autodestruct)
                {
                    it.Clear();
                    //it.time = 0f;
                }
            }
        }
        public void Stop()
        {
            coroutineHelper.StopAll();
            //if (m_animator)
            //{
            //    m_animator.enabled = false;
            //}
            if (m_cachePatch)
            {
                if (m_animation)
                {
                    if (m_animation.gameObject.activeInHierarchy)
                    {
                        m_animation.Stop();
                    }
                }
                for (int index = 0; index < m_particles.Count; ++index)
                {
                    var it = m_particles[index];
                    if (it.gameObject.activeInHierarchy) it.Stop();
                }
                Util.UnityHelper.Hide(this);
            }
        }
        public IEnumerator FlyWithParacurve(Vector3 from, Vector3 to, float time, float h)
        {
            var speed = (to - from).magnitude / time;
            transform.position = from;
            float usedTime = 0f;
            while (true)
            {
                if (usedTime > time)
                {
                    transform.position = to;
                    break;
                }
                else
                {
                    var pos = Vector3.MoveTowards(from, to, speed * usedTime);
                    float deltaH = h * (0.5f * 0.5f - (usedTime / time - 0.5f) * (usedTime / time - 0.5f));//抛物线方程
                    pos.y += deltaH;
                    transform.position = pos;
                    transform.forward = to - transform.position;
                    usedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }
        public IEnumerator FlyWithTime(Vector3 from, Vector3 to, float time, bool useRealTime = false)
        {
            transform.position = from;
            float usedTime = 0f;
            float speed = (to - transform.position).magnitude / time;
            while (true)
            {
                var deltaTime = useRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
                usedTime += deltaTime;
                if (usedTime >= time)
                {
                    transform.position = to;
                    break;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, to, speed * deltaTime);
                    yield return null;
                }
            }
        }
        public IEnumerator FlyWithSpeed(Vector3 from, Transform to, Vector3 offset, float speed)
        {
            return FlyWithTime(from, to, offset, (to.localToWorldMatrix.MultiplyPoint3x4(offset) - from).magnitude / speed);
        }
        public IEnumerator FlyWithTime(Vector3 from, Transform to, Vector3 offset, float time, bool useRealTime = false)
        {
            transform.position = from;
            float usedTime = 0f;
            float speed = (to.localToWorldMatrix.MultiplyPoint3x4(offset) - transform.position).magnitude / time;
            while (true)
            {
                var deltaTime = useRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
                usedTime += deltaTime;
                if (usedTime >= time)
                {
                    transform.position = to.position;
                    break;
                }
                else
                {
                    var dst = to.localToWorldMatrix.MultiplyPoint3x4(offset);
                    transform.position = Vector3.MoveTowards(transform.position, dst, speed * deltaTime);
                    //transform.forward = dst - transform.position;
                    yield return null;
                }
            }
        }
        void Update()
        {
            if (gameObject.layer == Util.TagLayers.Recycle) { return; }

            if (!canSuspend && Time.timeScale <= 0.01f)
            {
                if (m_animator)
                {
                    m_animator.Update(Time.unscaledDeltaTime);
                }
                if (m_animation)
                {
                    var clip = m_animation.clip;
                    if (clip)
                    {
                        var state = m_animation[clip.name];
                        if (state) { state.time = Time.unscaledTime; }
                    }
                }
                for (int index = 0; index < m_particles.Count; ++index)
                {
                    var it = m_particles[index];
                    if (it.gameObject.activeInHierarchy && it.IsAlive())
                    {
                        it.Simulate(Time.unscaledTime);
                    }
                }
            }
        }
        public void PlayAutoDestroy(Transform parent, bool lockstep, TimeType timeType = TimeType.Time)
        {
            PlayAutoDestroy(parent, Vector3.zero, lockstep, timeType);
        }
        public void PlayAutoDestroy(Transform parent, Vector3 offset, bool lockstep, TimeType timeType = TimeType.Time)
        {
            autoDestroy = true;
            Play(parent, offset);
            //if (duration > 0f)
            if (lockstep)
            {
                coroutineHelper.StartCoroutineImmediate(WaitSeconds.Delay(duration, timeType), DestroyGo);
            }
            else
            {
                coroutineHelper.StartCoroutine(WaitSeconds.Delay(duration, timeType), DestroyGo);
            }
        }
        public static FXControl Create(string path, string name, bool cachePatch = true)
        {
            var go = GameObjPool.Instance.CreateObjSync(path, name);
            var fx = go.GetComponent<FXControl>();
            if (!fx) { fx = go.AddComponent<FXControl>(); }
            fx.m_cachePatch = cachePatch;
            return fx;
        }
        public static void Destroy(FXControl fx)
        {
            if (fx)
            {
                if (fx.autoDestroy)
                {
                    fx.Stop();
                }
                else
                {
                    fx.Stop();
                    //fx.m_cachePatch = true;
                    fx.DestroyGo();
                }
            }
        }
        bool m_cachePatch;
        void DestroyGo()
        {
            if (m_cachePatch) GameObjPool.Instance.DestroyObj(gameObject);
            else
            {
                Util.UnityHelper.DestroyGameObjectNoUnInit(gameObject);
            }
        }
    }
}