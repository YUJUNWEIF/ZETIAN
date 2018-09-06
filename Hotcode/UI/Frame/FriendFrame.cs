using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FriendFrame : ILSharpScript
    {
//generate code begin
        public RectTransform left;
        public Button left_closeLeft;
        public Image right;
        public Text right_capcity;
        public RectTransform right_Root_friend;
        public LSharpListSuper right_Root_friend_clip_list;
        public RectTransform right_Root_apply;
        public RectTransform right_Root_apply_clip_list;
        public RectTransform right_Root_search;
        public InputField right_Root_search_nameInput;
        public Button right_Root_search_confirmAdd;
        public Toggle right_friend;
        public Toggle right_apply;
        public Image right_apply_tip;
        public Toggle right_search;
        public Button right_close;
        void __LoadComponet(Transform transform)
        {
            left = transform.Find("@left").GetComponent<RectTransform>();
            left_closeLeft = transform.Find("@left/@closeLeft").GetComponent<Button>();
            right = transform.Find("@right").GetComponent<Image>();
            right_capcity = transform.Find("@right/@capcity").GetComponent<Text>();
            right_Root_friend = transform.Find("@right/Root/@friend").GetComponent<RectTransform>();
            right_Root_friend_clip_list = transform.Find("@right/Root/@friend/clip/@list").GetComponent<LSharpListSuper>();
            right_Root_apply = transform.Find("@right/Root/@apply").GetComponent<RectTransform>();
            right_Root_apply_clip_list = transform.Find("@right/Root/@apply/clip/@list").GetComponent<RectTransform>();
            right_Root_search = transform.Find("@right/Root/@search").GetComponent<RectTransform>();
            right_Root_search_nameInput = transform.Find("@right/Root/@search/@nameInput").GetComponent<InputField>();
            right_Root_search_confirmAdd = transform.Find("@right/Root/@search/@confirmAdd").GetComponent<Button>();
            right_friend = transform.Find("@right/@friend").GetComponent<Toggle>();
            right_apply = transform.Find("@right/@apply").GetComponent<Toggle>();
            right_apply_tip = transform.Find("@right/@apply/@tip").GetComponent<Image>();
            right_search = transform.Find("@right/@search").GetComponent<Toggle>();
            right_close = transform.Find("@right/@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            right_Root_friend_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            right_Root_friend_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            right_Root_friend_clip_list.OnShow();
        }
        void __DoHide()
        {
            right_Root_friend_clip_list.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        //public static FriendFrame Instance { get; private set; }
        //public override void OnInitialize()
        //{
        //    Instance = this;
        //    base.OnInitialize();
        //    friendPanel.OnInitialize();
        //    applierPanel.OnInitialize();
        //    friendBriefPanel.OnInitialize();
        //    mailCreatePanel.OnInitialize();

        //    friendPanel.selectListener.Add(() =>
        //    {
        //        var no = friendPanel.itemSelected.First;
        //        if (no == null)
        //        {
        //            if (!Mathf.Approximately(leftTweener.tweenFactor, 0)) { leftTweener.Play(false); }
        //        }
        //        else
        //        {
        //            DisplayFriendBrief(friendPanel.values[no.Value]);
        //            leftTweener.Play();
        //        }
        //    });

        //    friendButton.onValueChanged.AddListener(isOn =>
        //    {
        //        Util.UnityHelper.ShowHide(friendRoot, isOn);
        //        if (isOn) { OnSync(); }
        //    });
        //    applierButton.onValueChanged.AddListener(isOn =>
        //    {
        //        Util.UnityHelper.ShowHide(applierRoot, isOn);
        //        if (isOn) { OnSync(); }
        //    });
        //    addFriendButton.onValueChanged.AddListener(isOn =>
        //    {
        //        Util.UnityHelper.ShowHide(friendAddRoot, isOn);

        //    });
        //    confirmAddButton.onClick.AddListener(() =>
        //    {
        //        if (!string.IsNullOrEmpty(nameInput.text))
        //        {
        //            HttpNetwork.Inst().Communicate(new pps.FriendAddRequest() { playerId = nameInput.text });
        //        }
        //    });

        //    closeLeftButton.onClick.AddListener(() => { leftTweener.Play(false); });
        //    closeButton.onClick.AddListener(() => GuiManager.Instance.HideFrame(this));
        //    friendButton.isOn = true;
        //}
        //public override void OnUnInitialize()
        //{
        //    friendPanel.OnUnInitialize();
        //    applierPanel.OnUnInitialize();
        //    friendBriefPanel.OnUnInitialize();
        //    mailCreatePanel.OnInitialize();
        //    base.OnUnInitialize();
        //    Instance = null;
        //}
        //public override void OnShow()
        //{
        //    base.OnShow();
        //    friendPanel.OnShow();
        //    applierPanel.OnShow();
        //    friendBriefPanel.OnShow();
        //    mailCreatePanel.OnShow();

        //    FriendModule.Instance.onSync.Add(OnSync);
        //    OnCapcityChanged();
        //    OnSync();
        //    leftTweener.SampleAt(0);
        //    rightTweener.Play(false);
        //}
        //public override void OnHide()
        //{
        //    friendPanel.OnHide();
        //    applierPanel.OnHide();
        //    friendBriefPanel.OnHide();
        //    mailCreatePanel.OnHide();

        //    FriendModule.Instance.onSync.Rmv(OnSync);
        //    base.OnHide();
        //}
        //public void DisplayFriendBrief(Friend friend)
        //{
        //    Util.UnityHelper.Show(friendBriefPanel);
        //    Util.UnityHelper.Hide(mailCreatePanel);
        //    friendBriefPanel.Display(friend);
        //    leftTweener.Play();
        //}
        //public void DisplayCreateMail(Friend friend)
        //{
        //    Util.UnityHelper.Hide(friendBriefPanel);
        //    Util.UnityHelper.Show(mailCreatePanel);
        //    mailCreatePanel.SetReceiver(friend.id, friend.name);
        //    leftTweener.Play();
        //}
        //void OnCapcityChanged()
        //{
        //    var alls = FriendModule.Instance.friends.FindAll(fr => fr.type == Friend.Type.Ok);
        //    capacityLabel.text = new RangeValue(alls.Count, FriendModule.Instance.capacity).ToStyle1();
        //}
        //void OnSync()
        //{
        //    if (friendButton.isOn)
        //    {
        //        var alls = FriendModule.Instance.friends.FindAll(fr => fr.type == Friend.Type.Ok);
        //        friendPanel.SetValues(alls);
        //    }
        //    else if (applierButton.isOn)
        //    {
        //        var alls = FriendModule.Instance.friends.FindAll(fr => fr.type == Friend.Type.NeedSelfConfirm);
        //        applierPanel.SetValues(alls);
        //    }
        //    Util.UnityHelper.ShowHide(applyTipRoot, FuncTipModule.Instance.GetFuncTip(Function.Friend));
        //}
    }
}
