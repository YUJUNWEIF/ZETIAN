using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby.ui
{
    public class GuiPanel : BehaviorWrapper
    {
        RectTransform cachedRc;
        public void Insert(RectTransform parent, AnchorMask anchor, Vector2 offset)
        {
            Util.UnityHelper.Show(cachedRc, parent);
            //VVPanel.Display(cachedRc, parent, anchor, offset);
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            cachedRc = transform as RectTransform;
        }
    }
}