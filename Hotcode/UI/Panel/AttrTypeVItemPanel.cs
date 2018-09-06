using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class AttrTypeVItemPanel : ILSharpScript
    {
//generate code begin
        public Text value;
        void __LoadComponet(Transform transform)
        {
            value = transform.Find("@value").GetComponent<Text>();
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
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        _appendAttr m_display;
        public _appendAttr GetValue() { return m_display; }
        public void SetValue(_appendAttr v)
        {
            m_display = v;
            value.text = string.Format(tab.attr.Inst().Find((int)m_display.type).des, m_display.value.ToString("0.00"));
        }
    }
}
