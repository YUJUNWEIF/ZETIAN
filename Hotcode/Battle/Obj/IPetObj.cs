using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public class IPetObj : IBaseObj
    {
        protected SkinAnimation m_skinAnim = new SkinAnimation();
        public GameObject obj { get; protected set; }
        public cfg.hero petCfg { get; private set; }
        public void Change(int mId)
        {
            UnInitialize();
            this.moduleId = mId;
            petCfg = tab.hero.Instance.Find(moduleId);
            var resCfg = tab.objRes.Inst().Find(petCfg.star.resId);
            obj = GameObjPool.Instance.CreateObjSync(GamePath.asset.pet.prefab, resCfg.name);
            Util.UnityHelper.Show(obj.transform, transform);
            obj.transform.localScale = Vector3.one;

            //string absolutePath = GamePath.asset.fish.path + GamePath.asset.fish.anim;
            //var clips = GameObjPool.Instance.LoadAnim(absolutePath);
            //if (clips == null)
            //{
            //    var tmp = Resources.LoadAll(absolutePath, typeof(AnimationClip));
            //    clips = System.Array.ConvertAll(tmp, it => it as AnimationClip);
            //    if (clips != null) { GameObjPool.Instance.AddClip(absolutePath, clips); }
            //}
            Patch("TD_AttackGround");
            Patch("TD_Idles");
        }
        public override void UnInitialize()
        {
            m_skinAnim.Stop();
            if (obj)
            {
                GameObjPool.Instance.DestroyObj(obj);
                obj = null;
            }
            base.UnInitialize();
        }
        void Patch(string anim)
        {
            string absolutePath = GamePath.asset.pet.anim + anim;
            var clips = GameObjPool.Instance.LoadAnim(absolutePath);
            if (clips == null)
            {
                var tmp = Resources.LoadAll(absolutePath, typeof(AnimationClip));
                clips = System.Array.ConvertAll(tmp, it => it as AnimationClip);
                if (clips != null) { GameObjPool.Instance.AddClip(absolutePath, clips); }
            }
            m_skinAnim.SetVisibleData(obj.transform, clips);
        }
        public void Display(string anim)
        {
            var wrapMode = m_skinAnim.GetAnimationWrapMode(anim);
            if (wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPong)
            {
                m_skinAnim.Play(anim, 1f);
            }
            else
            {
                //m_skinAnim.FinishWith(anim, petCfg.animIdle, 1f);
            }
        }
        float time = 0;
        public void Attack()
        {
            time = 0f;
            //var firstIndex = Random.Range(0, starCfg.animIdles.firsts.Length);
            //m_skinAnim.FinishWith(starCfg.animAttack, starCfg.animIdles.firsts[firstIndex], 1f);
        }
        public void Idle()
        {
            time = 0f;
            //var firstIndex = Random.Range(0, starCfg.animIdles.firsts.Length);
            //m_skinAnim.Play(starCfg.animIdles.firsts[firstIndex], 1f);
        }
        protected void Update()
        {
            time += Time.deltaTime;
            if (time >= 10f)
            {
                time = 0f;
                //var firstIndex = Random.Range(0, starCfg.animIdles.firsts.Length);
                //var secondIndex = Random.Range(0, starCfg.animIdles.seconds.Length);
                //m_skinAnim.FinishWith(starCfg.animIdles.seconds[secondIndex], starCfg.animIdles.firsts[firstIndex], 1f);
            }
        }
    }
}