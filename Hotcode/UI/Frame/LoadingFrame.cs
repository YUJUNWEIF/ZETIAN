using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class LoadingFrame : ILSharpScript
    {
//generate code begin
        public GuiBar bar;
        public Text bar_progress;
        public Text content;
        void __LoadComponet(Transform transform)
        {
            bar = transform.Find("@bar").GetComponent<GuiBar>();
            bar_progress = transform.Find("@bar/@progress").GetComponent<Text>();
            content = transform.Find("@content").GetComponent<Text>();
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
        public float duration = 2f;
        float m_passTime;
        float m_delayHide;
        float m_delayPercent;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public override void OnShow()
        {
            base.OnShow();
            m_passTime = 0f;
            m_delayHide = -1f;
            m_delayPercent = 0.8f + UnityEngine.Random.Range(0f, 1f) * 0.15f;
            Util.TimerManager.Inst().onFrameUpdate.Add(Update);
        }
        public override void OnHide()
        {
            Util.TimerManager.Inst().onFrameUpdate.Rmv(Update);
            base.OnHide();
        }
        void Update()
        {
            m_passTime += Time.unscaledDeltaTime;
            if (m_delayHide >= 0f)
            {
                if (Time.unscaledTime >= m_delayHide)
                {
                    GuiManager.Instance.HideFrame(api.name);
                }
                else
                {
                    bar.value = Mathf.Clamp01(m_passTime / (duration * m_delayPercent + 0.5f));
                    bar_progress.text = string.Format("{0:N0}%", bar.value * 100);
                }
            }
            else
            {
                var tor = duration * m_delayPercent;
                if (m_passTime > tor) m_passTime = tor;
                bar.value = m_passTime / duration;
                bar_progress.text = string.Format("{0:N0}%", bar.value * 100);
            }
        }
        public void DelayHide()
        {
            m_delayHide = Time.unscaledTime + 0.5f;
        }
    }
}
