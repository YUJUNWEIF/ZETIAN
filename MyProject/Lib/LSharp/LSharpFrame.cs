using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby.LSharpScript
{
    public class LSharpFrame : LSharpAPI, IGuiFrame
    {
        public bool transparentMessage = false;
        public bool fullscreen = false;
        public bool autoRelease = false;
        public bool firstLevelFrame = false;

        public Canvas canvas { get { return impl.canvas; } }
        public GraphicRaycaster raycaster { get { return impl.raycaster; } }
        public Button __close { get { return impl.__close; } }

        public bool TransparentMessage { get { return transparentMessage; } }
        public bool Fullscreen { get { return fullscreen; } }
        public bool AutoRelease { get { return autoRelease; } }
        public bool IsShow { get { return impl.IsShow; } }

        FrameImpl impl;
        public override void OnInitialize()
        {
            base.OnInitialize();
            impl = new FrameImpl(transform, firstLevelFrame);
        }
        public void Show(Transform desktop, int layer = 0)
        {
            impl.Show(desktop, layer);
        }
        public void Hide()
        {
            impl.Hide();
        }
        public void SetLayer(int layer)
        {
            impl.SetLayer(layer);
        }
    }
}