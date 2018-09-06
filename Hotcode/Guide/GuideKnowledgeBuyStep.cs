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
    public class KnowledgeBuyStep : GuideStep
    {
        public override void Process() { }
        public override void Enter()
        {
            base.Enter();
            GuideMaskFrame.Trigger(false);
        }
        public override void Leave()
        {
            GuideMaskFrame.Trigger(true);
            base.Leave();
        }
        public override void Update()
        {
            Leave();
        }
    }
}