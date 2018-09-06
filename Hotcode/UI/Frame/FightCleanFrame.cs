using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FightCleanFrame : ILSharpScript
    {
//generate code begin
        public Text Image_tick;
        void __LoadComponet(Transform transform)
        {
            Image_tick = transform.Find("Image/@tick").GetComponent<Text>();
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
        int m_sec;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public override void OnShow()
        {
            base.OnShow();
            m_sec = 4;
            Util.TimerManager.Inst().Add(OnTimer, 1000);
            OnTimer();
        }
        public override void OnHide()
        {
            Util.TimerManager.Inst().Remove(OnTimer);
            base.OnHide();
        }
        void OnTimer()
        {
            --m_sec;
            if (m_sec < 0) { m_sec = 0; }
            Image_tick.text = m_sec.ToString();
        }
    }
}
