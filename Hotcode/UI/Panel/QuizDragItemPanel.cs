using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizDragItemPanel : ILSharpScript
    {
//generate code begin
        public Image ch;
        public Image locked;
        void __LoadComponet(Transform transform)
        {
            ch = transform.Find("@ch").GetComponent<Image>();
            locked = transform.Find("@locked").GetComponent<Image>();
        }
        void __DoInit()
        {
        }
        void __DoUninit()
        {
        }
        void __DoShow()
        {
        }
        void __DoHide()
        {
        }
//generate code end
        QuizPazzle m_value;
        RectTransform m_cachedRc;
        DragGridCell def;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            m_cachedRc = (RectTransform)api.transform;
            def = api.GetComponent<DragGridCell>();
            def.onPress.Add(ev => QuizFrame.Instance.Press(this));
            def.onDragging.Add(ev =>
            {
                var screenPos = RectTransformUtility.WorldToScreenPoint(ev.pressEventCamera, m_cachedRc.position);
                
                for (int index = 0; index < QuizFrame.Instance.quizDrag.count; ++index)
                {
                    var targetItem = QuizFrame.Instance.quizDrag.GetItem(index);
                    if (targetItem == api) { continue; }
                    var v = (QuizPazzle)targetItem.AttachValue;
                    if (v.Key < 0) { continue; }

                    var rc = (RectTransform)targetItem.transform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rc, screenPos, ev.pressEventCamera))
                    {
                        QuizFrame.Instance.Swap(this, T.As<QuizDragItemPanel>(targetItem));
                        break;
                    }
                }
            });
            def.onSwap.Add(dst =>
            {
                var targetItem = dst.GetComponent<LSharpItemPanel>();
                if (targetItem)
                {
                    var v = (QuizPazzle)targetItem.AttachValue;
                    if (v.Key >= 0)
                    {
                        QuizSceneManager.mod.Swap(m_value.Key, v.Key);
                    }
                }
            });
            def.onRelease.Add(ev => QuizFrame.Instance.Release());
        }
        public QuizPazzle GetValue() { return m_value; }
        public void SetValue(QuizPazzle value)
        {
            m_value = value;
            bool valid = m_value.Key >= 0;
            locked.enabled = !valid;
            ch.sprite = SpritesManager.Inst().Find(char.ToLower(m_value.Value).ToString());
            Update();
        }
        public void ForceStopDrag()
        {
            def.ReleaseDrag();
        }
        void Update()
        {
            bool valid = m_value.Key >= 0;
            var canReply = (QuizSceneManager.mod.state == QuizState.RoundRunning);
            def.clickable = canReply && valid;
            def.draggle = canReply && valid;
        }
    }
}
