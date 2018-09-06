using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FrameImpl
{
    public const int hideLayer = 0;
    public const int layerDelteY = -200;
    public bool IsShow { get; private set; }    
    public Canvas canvas { get; private set; }
    public GraphicRaycaster raycaster { get; private set; }
    public Button __close { get; private set; }
    Transform m_trans;
    int m_layerOrder = -1;

    static List<Renderer> renders = new List<Renderer>();
    public FrameImpl(Transform trans, bool firstLevel)
    {
        m_trans = trans;    
        canvas = m_trans. GetComponent<Canvas>();
        if (!canvas) { canvas = m_trans.gameObject.AddComponent<Canvas>(); }
        raycaster = m_trans.GetComponent<GraphicRaycaster>();
        if (!raycaster) { raycaster = m_trans.gameObject.AddComponent<GraphicRaycaster>(); }
        canvas.overrideSorting = true;
        if (firstLevel)
        {
            __close = m_trans.Find("__msg_eater").gameObject.AddComponent<Button>();
            __close.onClick.AddListener(() => GuiManager.Inst().HideFrame(trans.name));
        }
    }
    public void Show(Transform desktop, int layer = 0)
    {
        if (!IsShow)
        {
            IsShow = true;
            Util.UnityHelper.Show(m_trans, desktop, true, true);
        }
        SetLayer(layer);
    }
    public void Hide()
    {
        if (!IsShow) { return; }

        IsShow = false;
        Util.UnityHelper.Hide(m_trans);
        SetLayer(hideLayer);
    }
    public void SetLayer(int layer)
    {
        if (m_layerOrder != layer)
        {
            m_layerOrder = layer;
            if (layer >= 0)// && !canvas.isRootCanvas)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = layer * 2;

                m_trans.GetComponentsInChildren<Renderer>(true, renders);
                for (int index = 0; index < renders.Count; ++index)
                {
                    var render = renders[index];
                    //render.sortingLayerName = "ui";
                    //render.material.renderQueue = order;
                    render.sortingOrder = layer * 2 + 1;
                    //render.sortingLayerName = m_layerName;
                }
            }
            Vector3 loc = m_trans.localPosition;
            loc.z = layer * layerDelteY;
            m_trans.localPosition = loc;
        }
    }
}
namespace geniusbaby.ui
{
    public class GuiFrame : BehaviorWrapper, IGuiFrame
    {
        public bool transparentMessage = false;
        public bool autoRelease = false;
        public bool fullscreen = false;
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