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
    public class CombatFinishStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public int newGuideId;
        }
        Param param;
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            param = (Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
        }
        public override void Enter()
        {
            stepCfg.autoType = cfg.guide.StepManualNext;
            base.Enter();
            FightSceneManager.Inst().afterFight = Leave;
            GuiManager.Inst().HideFrame(frame.api.name);
        }
        public override void Update() { }
        public override void Process() { }        
    }
}