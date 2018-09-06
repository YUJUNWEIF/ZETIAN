using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public enum MsgBoxType
    {
        Mbt_OkCancel,
        Mbt_Ok,
    }
    public class MessageBoxFrame : ILSharpScript
    {
//generate code begin
        public Text Image_des;
        public Button Image_ok;
        public Button Image_cancel;
        void __LoadComponet(Transform transform)
        {
            Image_des = transform.Find("Image/@des").GetComponent<Text>();
            Image_ok = transform.Find("Image/@ok").GetComponent<Button>();
            Image_cancel = transform.Find("Image/@cancel").GetComponent<Button>();
        }
        void __DoInit()
        {
        }
        void __DoUninit()
        {
        }
        void __DoShow()
        {
        }
        void __DoHide()
        {
        }
//generate code end
        Action m_ok;
        Action m_cancel;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            Image_ok.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(GetType().Name);
                if (m_ok != null) m_ok();
            });
            Image_cancel.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(GetType().Name);
                if (m_cancel != null) m_cancel();
            });
        }
        public void SetDelegater(Action ok = null, Action cancel = null)
        {
            m_ok = ok;
            m_cancel = cancel;
        }
        public void SetDesc(string desc, MsgBoxType mbt = MsgBoxType.Mbt_OkCancel)
        {
            if (Image_des != null) { Image_des.text = desc; }
            Image_cancel.gameObject.SetActive(mbt == MsgBoxType.Mbt_OkCancel);
        }
        public override void OnShow()
        {
            base.OnShow();
            m_ok = null;
            m_cancel = null;
        }
    }
}
