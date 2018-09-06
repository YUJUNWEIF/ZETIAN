using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MailFrame : ILSharpScript
    {
//generate code begin
        public Image right;
        public LSharpListSuper right_BG_clip_list;
        public Toggle right_Group_email;
        public Text right_Group_email_CountBG_count;
        public Toggle right_Group_system;
        public Text right_Group_system_CountBG_count;
        public Button right_close;
        public Image left;
        public Button left_leftClose;
        void __LoadComponet(Transform transform)
        {
            right = transform.Find("@right").GetComponent<Image>();
            right_BG_clip_list = transform.Find("@right/BG/clip/@list").GetComponent<LSharpListSuper>();
            right_Group_email = transform.Find("@right/Group/@email").GetComponent<Toggle>();
            right_Group_email_CountBG_count = transform.Find("@right/Group/@email/CountBG/@count").GetComponent<Text>();
            right_Group_system = transform.Find("@right/Group/@system").GetComponent<Toggle>();
            right_Group_system_CountBG_count = transform.Find("@right/Group/@system/CountBG/@count").GetComponent<Text>();
            right_close = transform.Find("@right/@close").GetComponent<Button>();
            left = transform.Find("@left").GetComponent<Image>();
            left_leftClose = transform.Find("@left/@leftClose").GetComponent<Button>();
        }
        void __DoInit()
        {
            right_BG_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            right_BG_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            right_BG_clip_list.OnShow();
        }
        void __DoHide()
        {
            right_BG_clip_list.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        //public static MailFrame Instance { get; private set; }
        //public override void OnInitialize()
        //{
        //    Instance = this;
        //    base.OnInitialize();
        //    formPanel.OnInitialize();
        //    formDetailPanel.OnInitialize();
        //    formPanel.selectListener.Add(() =>
        //    {
        //        var no = formPanel.itemSelected.First;
        //        if (no != null)
        //        {
        //            var form = formPanel.values[no.Value];
        //            formDetailPanel.Display(form);
        //            if (!Mathf.Approximately(leftTweener.tweenFactor, 1))
        //            {
        //                leftTweener.Play();
        //            }
        //        }
        //        else
        //        {
        //            if (!Mathf.Approximately(leftTweener.tweenFactor, 0))
        //            {
        //                leftTweener.Play(false);
        //            }
        //        }
        //    });
        //    emailToggle.onValueChanged.AddListener(isOn => { if (isOn) OnMailSync(); });
        //    systemToggle.onValueChanged.AddListener(isOn => { if (isOn) OnMailSync(); });
        //    closeButton.onClick.AddListener(() => GuiManager.Instance.HideFrame(this));
        //    closeLeftButton.onClick.AddListener(() => { leftTweener.Play(false); });
        //    emailToggle.isOn = true;
        //}
        //public override void OnUnInitialize()
        //{
        //    formPanel.OnUnInitialize();
        //    formDetailPanel.OnUnInitialize();
        //    base.OnUnInitialize();
        //    Instance = null;
        //}
        //public override void OnShow()
        //{
        //    base.OnShow();
        //    formPanel.OnShow();
        //    formDetailPanel.OnShow();
        //    MailModule.Instance.onMailSync.Add(OnMailSync);
        //    MailModule.Instance.onMailUpdate.Add(OnMailUpdate);
        //    OnMailSync();
        //    leftTweener.SampleAt(0);
        //    rightTweener.Play(false);
        //}
        //public override void OnHide()
        //{
        //    MailModule.Instance.onMailSync.Rmv(OnMailSync);
        //    MailModule.Instance.onMailUpdate.Rmv(OnMailUpdate);
        //    formPanel.OnHide();
        //    formDetailPanel.OnHide();
        //    base.OnHide();
        //}
        //void OnMailUpdate(Mail form)
        //{
        //    UpdateCounter();
        //    for (int index = 0; index < formPanel.values.Count; ++index)
        //    {
        //        var it = formPanel.values[index];
        //        if (it.id == form.id)
        //        {
        //            formPanel.values[index] = form;
        //        }
        //    }
        //}
        //void OnMailSync()
        //{
        //    UpdateCounter();
        //    var mails = MailModule.Instance.mails.FindAll(it
        //        => systemToggle.isOn && string.IsNullOrEmpty(it.senderId) || emailToggle.isOn && !string.IsNullOrEmpty(it.senderId));
        //    mails.Sort((x, y) =>
        //    {
        //        int result = x.hasRead.CompareTo(y.hasRead);
        //        if (result != 0) return result;
        //        return -x.createUtc.CompareTo(y.createUtc);
        //    });
        //    formPanel.SetValues(mails);
        //}
        //void UpdateCounter()
        //{
        //    int mailCount = 0;
        //    int systemCount = 0;
        //    MailModule.Instance.mails.ForEach(it =>
        //    {
        //        if (!it.hasRead)
        //        {
        //            if (string.IsNullOrEmpty(it.senderId))
        //            {
        //                ++systemCount;
        //            }
        //            else
        //            {
        //                ++mailCount;
        //            }
        //        }
        //    });
        //    const int fontWidth = 16;
        //    emailCountText.text = mailCount.ToString();
        //    var parent = (RectTransform)emailCountText.transform.parent;
        //    parent.sizeDelta = new Vector2(fontWidth * emailCountText.text.Length, parent.sizeDelta.y);

        //    systemCountText.text = systemCount.ToString();
        //    parent = (RectTransform)systemCountText.transform.parent;
        //    parent.sizeDelta = new Vector2(fontWidth * systemCountText.text.Length, parent.sizeDelta.y);
        //}
    }
}
