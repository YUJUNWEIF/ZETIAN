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
    public class ResumePauseStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public int pauseResume;
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
            base.Enter();
            FightSceneManager.Inst().pause = param.pauseResume == 1;
        }
        public override void Update() { }
        public override void Process() { }
    }
}