using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public abstract class IGameObj : IBaseObj
    {
        protected SkinAnimation m_skinAnim = new SkinAnimation();
        protected ModelAnimation m_modelAnim = new ModelAnimation();
        //public Util.ParamActions onTransform = new Util.ParamActions();
        
        public override void UnInitialize()
        {
            m_skinAnim.Stop();
            var fxes = GetComponentsInChildren<FXControl>(true);
            for (int index = 0; index < fxes.Length; ++index)
            {
                FXControl.Destroy(fxes[index]);
            }
            base.UnInitialize();
        }
        //protected virtual void LateUpdate()
        //{
        //    if (transform.hasChanged)
        //    {
        //        transform.hasChanged = false;
        //        onTransform.Fire();
        //    }
        //}
        //protected void Display(string anim)
        //{
        //    m_skinAnim.Play(anim, 1f);
        //    //var wrapMode = m_skinAnim.GetAnimationWrapMode(anim);
        //    //if (wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPong)
        //    //{
        //    //    m_skinAnim.Play(anim, 1f);
        //    //}
        //    //else
        //    //{
        //    //    m_skinAnim.FinishWith(anim, GetAnimName(AnimationType.Idle), 1f);
        //    //}
        //}
    }
}