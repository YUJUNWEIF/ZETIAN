using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class DropFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper Rc_clip_itemPanel;
        public Button close;
        void __LoadComponet(Transform transform)
        {
            Rc_clip_itemPanel = transform.Find("Rc/clip/@itemPanel").GetComponent<LSharpListSuper>();
            close = transform.Find("@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            Rc_clip_itemPanel.OnInitialize();
        }
        void __DoUninit()
        {
            Rc_clip_itemPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            Rc_clip_itemPanel.OnShow();
        }
        void __DoHide()
        {
            Rc_clip_itemPanel.OnHide();
        }
//generate code end
        Action m_afterClose;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            Rc_clip_itemPanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(DropItemPanel).Name);
            __DoInit();
            close.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
                if (m_afterClose != null) { m_afterClose(); }
            });
        }
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void Display(List<Item> dropItems, Action afterClose = null)
        {
            m_afterClose = afterClose;
            Rc_clip_itemPanel.SetValues(dropItems.ConvertAll(it => (object)it));
        }
    }
}
