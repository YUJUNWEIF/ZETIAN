using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace geniusbaby.ui
{
    public class TeachingFrame : GuiFrame
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            GetComponent<Button>().onClick.AddListener(() => GuiManager.Inst().HideFrame(this));
        }
    }
}