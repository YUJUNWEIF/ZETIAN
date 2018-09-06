using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ExperienceFrame : ILSharpScript
    {
//generate code begin
        public Button panel;
        void __LoadComponet(Transform transform)
        {
            panel = transform.Find("@button").GetComponent<Button>();
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
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            panel.transform.localPosition = new Vector3(-277, 101, 0);
            panel.onClick.AddListener(() => Util.TimerManager.Inst().onFrameUpdate.Add(ShowText));
            //ShowText();
        }
        public override void OnUnInitialize()
        {
            Util.TimerManager.Inst().onFrameUpdate.Rmv(ShowText);
            base.OnUnInitialize();
        }
        void ShowText()
        {
            GuiManager.Inst().ShowFrame(typeof(geniusbaby.LSharpScript.TextFrame).Name);

        }
    }
}
