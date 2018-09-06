using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace geniusbaby.LSharpScript
{
    public class UIShowHideStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public string frame;
            public string[] children;
        }
        bool flag;
        Param param;
        public UIShowHideStep(bool flag) { this.flag = flag; }
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            param = (Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
        }
        public override void Process() { }
        public override void Enter()
        {
            if (param.children.Length <= 0)
            {
                if (flag)
                {
                    GuiManager.Inst().ShowFrame(param.frame);
                }
                else
                {
                    var script = GuiManager.Inst().GetCachedFrame(param.frame);
                    if (script) { GuiManager.Inst().HideFrame(script); }
                }
            }
            else
            {
                var script = GuiManager.Inst().GetCachedFrame(param.frame);
                if (script)
                {
                    for (int index = 0; index < param.children.Length; ++index)
                    {
                        var node = script.transform.Find(param.children[index]);
                        node.gameObject.SetActive(flag);
                    }
                }
            }
            base.Enter();
        }
        public override void Update() { }
    }
}