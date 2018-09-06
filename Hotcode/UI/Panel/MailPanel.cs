using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MailPanel : ILSharpScript
    {
//generate code begin
        public Text sender;
        public Text content;
        public LSharpListSuper clip_list;
        public Button delete;
        public Button award;
        void __LoadComponet(Transform transform)
        {
            sender = transform.Find("@sender").GetComponent<Text>();
            content = transform.Find("@content").GetComponent<Text>();
            clip_list = transform.Find("clip/@list").GetComponent<LSharpListSuper>();
            delete = transform.Find("@delete").GetComponent<Button>();
            award = transform.Find("@award").GetComponent<Button>();
        }
        void __DoInit()
        {
            clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            clip_list.OnShow();
        }
        void __DoHide()
        {
            clip_list.OnHide();
        }
//generate code end
        Mail m_mail;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(DropItemPanel).Name);
            __DoInit();

            award.onClick.AddListener(() =>
            {
                HttpNetwork.Inst().Communicate(new MailGetAttachmentRequest() { id = m_mail.id });
            });
            delete.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                if (m_mail.dropItems.Count > 0 && !m_mail.hasRead)
                {
                    var script = GuiManager.Instance.ShowFrame(typeof(MessageBoxFrame).Name);
                    var frame = T.As<MessageBoxFrame>(script);
                    frame.SetDelegater(this.OnConfirmDeleteEmail);
                    frame.SetDesc("Delete form ?");
                }
                else
                {
                    OnConfirmDeleteEmail();
                }
            }));
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
            MailModule.Instance.onMailUpdate.Add(OnMailUpdate);
        }
        public override void OnHide()
        {
            MailModule.Instance.onMailUpdate.Rmv(OnMailUpdate);
            __DoHide();
            base.OnHide();
        }
        public void SwitchPage(int page)
        {
            switch (page)
            {
                case 1: break;
                case 2: break;
            }
        }
        public void Display(Mail mail)
        {
            OnMailUpdate(m_mail = mail);
        }
        void OnConfirmDeleteEmail()
        {
            HttpNetwork.Inst().Communicate(new MailDeleteRequest() { id = m_mail.id });
        }
        void OnMailUpdate(Mail form)
        {
            content.text = m_mail.content;
            sender.text = m_mail.titleOrName;
            Util.UnityHelper.ShowHide(award, m_mail.dropItems.Count > 0);
            //clip_list.SetValues(m_mail.dropItems.ConvertAll(it => (object)it));
            if (!m_mail.hasRead)
            {
                HttpNetwork.Inst().Communicate(new MailOpenRequest() { id = m_mail.id });
            }
        }
    }
}
