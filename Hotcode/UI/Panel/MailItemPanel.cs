using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MailItemPanel : ILSharpScript
    {
//generate code begin
        public Text title;
        public Text time;
        public Text content;
        public Image hasAward;
        public Text newly;
        void __LoadComponet(Transform transform)
        {
            title = transform.Find("@title").GetComponent<Text>();
            time = transform.Find("@time").GetComponent<Text>();
            content = transform.Find("@content").GetComponent<Text>();
            hasAward = transform.Find("@hasAward").GetComponent<Image>();
            newly = transform.Find("@newly").GetComponent<Text>();
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
        Mail m_mail;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
        }
        public void SetValue(Mail value) { OnUpdate(m_mail = value); }
        public object GetValue() { return m_mail; }

        public override void OnShow()
        {
            base.OnShow();
            MailModule.Instance.onMailUpdate.Add(OnUpdate);
        }
        public override void OnHide()
        {
            MailModule.Instance.onMailUpdate.Rmv(OnUpdate);
            base.OnHide();
        }
        void OnUpdate(Mail form)
        {
            if (m_mail.id == form.id)
            {
                m_mail = form;
                //var dt = TimerManager.ToDateTime(m_form.createUtc);
                //timeText.text = dt.ToShortDateString();

                title.text = m_mail.titleOrName;
                content.text = (m_mail.content.Length > 10) ? (m_mail.content.Substring(0, 10) + @"...") : m_mail.content;
                hasAward.color = m_mail.dropItems.Count > 0 ? Color.white : Color.clear;
                newly.color = m_mail.hasRead ? Color.clear : Color.white;
            }
        }
    }
}
