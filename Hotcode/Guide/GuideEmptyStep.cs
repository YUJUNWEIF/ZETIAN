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
    public class EmptyStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public int time;
        }
        Param param;
        float m_time;
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            param = (Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
        }
        public override void Process() { }
        public override void Enter()
        {
            //base.Enter();
            m_time = Time.time;

            frame.mask.material.SetVector(@"_ShapePos", Vector4.zero);
            frame.mask.material.SetVector(@"_ShapeSize", Vector4.zero);
        }
        public override void Update()
        {
            if ((Time.time - m_time) * 1000 >= param.time)
            {
                frame.GotoNext();
            }
        }
    }
}