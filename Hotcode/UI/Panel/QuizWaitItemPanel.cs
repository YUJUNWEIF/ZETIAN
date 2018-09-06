using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizWaitItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        public RectTransform empty;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            empty = transform.Find("@empty").GetComponent<RectTransform>();
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
        pps.ProtoDetailPlayer m_hero;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }         
        public pps.ProtoDetailPlayer GetValue(){ return m_hero; }
        public void SetValue(pps.ProtoDetailPlayer value)
        {
            m_hero = value;
            bool invalid = (m_hero == null);
            if (!invalid) { name.text = m_hero.name; }
            if (empty) { Util.UnityHelper.ShowHide(empty, invalid); }
        }
    }
}
