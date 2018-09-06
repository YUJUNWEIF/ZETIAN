using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FightTransmitFrame : ILSharpScript
    {
//generate code begin
        public Text BG_text;
        void __LoadComponet(Transform transform)
        {
            BG_text = transform.Find("BG/@text").GetComponent<Text>();
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
        long startMs;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public void Display(string des)
        {
            startMs = FightSceneManager.mod.timeMs;
            BG_text.text = des;
        }
        void Update()
        {
            if (FightSceneManager.mod.timeMs - startMs > 3 * 1000)
            {
                GuiManager.Inst().HideFrame(api.name);
            }
        }
    }
}
