using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace geniusbaby.ui
{
    public class VersionCheckFrame : GuiFrame
    {
        LSharpScript.VersionCheckFrame script = new LSharpScript.VersionCheckFrame();
        public override void OnInitialize()
        {
            base.OnInitialize();
            script.OnInitialize(this);
        }
        public override void OnUnInitialize()
        {
            script.OnUnInitialize();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            script.OnShow();
        }
        public override void OnHide()
        {
            script.OnHide();
            base.OnHide();
        }
    }
}