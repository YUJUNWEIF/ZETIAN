using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby.ui
{
    public class G3DTo2DPrefab : BehaviorWrapper
    {
        public static G3DTo2DPrefab Instance { get; private set; }
        RectTransform m_cachedRc;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Instance = this;
            m_cachedRc = (RectTransform)transform;
        }
        public override void OnUnInitialize()
        {
            Instance = null;
            base.OnUnInitialize();
        }
        //public G3DTo2DPopText ShowPopText(TextArea ta)
        //{
        //    return ShowPopText(ta, null);
        //}
        //public G3DTo2DPopText ShowPopText(TextArea ta, Transform parent)
        //{
        //    var go = GameObjPool.Instance.CreateObjSync(GamePath.asset.ui + @"Panel/", typeof(G3DTo2DPopText).Name);
        //    var script = go.GetComponent<G3DTo2DPopText>();
        //    if (!script) { script = go.AddComponent<G3DTo2DPopText>(); }
        //    Util.UnityHelper.Show(script, this);
            
        //    if (parent)
        //    {
        //        ta.position = parent.localToWorldMatrix.MultiplyPoint3x4(ta.position);
        //    }
        //    Vector2 localPosition;
        //    if (GameManager.G3DTo2DInRectangle(m_cachedRc, ta.position, out localPosition))
        //    {
        //        script.transform.localPosition = localPosition;
        //        coroutineHelper.StartCoroutineImmediate(script.PopText(m_cachedRc, ta), () => HidePopText(script));
        //    }
        //    return script;
        //}
        public void HidePopText(G3DTo2DPopText script)
        {
            GameObjPool.Instance.DestroyObj(script.gameObject);
        }
    }
}