using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class CommunicateFrame : ILSharpScript
    {
//generate code begin
        public GuiBar BG_Top_Bar;
        public Button BG_Top_friend;
        public Button BG_Top_chat;
        public Button BG_Top_mail;
        public GuiBar BG_Lc_bar;
        public RectTransform BG_Lc_friend;
        public Button BG_Lc_friend_accept;
        public Button BG_Lc_friend_search;
        public Button BG_Lc_friend_clip;
        public LSharpListSuper BG_Lc_friend_clip_list;
        public RectTransform BG_Lc_mail;
        public Text BG_Lc_mail_count;
        public LSharpListSuper BG_Lc_mail_clip_list;
        public RectTransform BG_Lc_chat;
        public Toggle BG_Lc_chat_broadcast;
        public Toggle BG_Lc_chat_union;
        public LSharpAPI BG_Rc_friendPanel;
        public LSharpAPI BG_Rc_chatPanel;
        public LSharpAPI BG_Rc_mailPanel;
        void __LoadComponet(Transform transform)
        {
            BG_Top_Bar = transform.Find("BG/Top/@Bar").GetComponent<GuiBar>();
            BG_Top_friend = transform.Find("BG/Top/@friend").GetComponent<Button>();
            BG_Top_chat = transform.Find("BG/Top/@chat").GetComponent<Button>();
            BG_Top_mail = transform.Find("BG/Top/@mail").GetComponent<Button>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<GuiBar>();
            BG_Lc_friend = transform.Find("BG/Lc/@friend").GetComponent<RectTransform>();
            BG_Lc_friend_accept = transform.Find("BG/Lc/@friend/@accept").GetComponent<Button>();
            BG_Lc_friend_search = transform.Find("BG/Lc/@friend/@search").GetComponent<Button>();
            BG_Lc_friend_clip = transform.Find("BG/Lc/@friend/@clip").GetComponent<Button>();
            BG_Lc_friend_clip_list = transform.Find("BG/Lc/@friend/@clip/@list").GetComponent<LSharpListSuper>();
            BG_Lc_mail = transform.Find("BG/Lc/@mail").GetComponent<RectTransform>();
            BG_Lc_mail_count = transform.Find("BG/Lc/@mail/@count").GetComponent<Text>();
            BG_Lc_mail_clip_list = transform.Find("BG/Lc/@mail/clip/@list").GetComponent<LSharpListSuper>();
            BG_Lc_chat = transform.Find("BG/Lc/@chat").GetComponent<RectTransform>();
            BG_Lc_chat_broadcast = transform.Find("BG/Lc/@chat/@broadcast").GetComponent<Toggle>();
            BG_Lc_chat_union = transform.Find("BG/Lc/@chat/@union").GetComponent<Toggle>();
            BG_Rc_friendPanel = transform.Find("BG/Rc/@friendPanel").GetComponent<LSharpAPI>();
            BG_Rc_chatPanel = transform.Find("BG/Rc/@chatPanel").GetComponent<LSharpAPI>();
            BG_Rc_mailPanel = transform.Find("BG/Rc/@mailPanel").GetComponent<LSharpAPI>();
        }
        void __DoInit()
        {
            BG_Lc_friend_clip_list.OnInitialize();
            BG_Lc_mail_clip_list.OnInitialize();
            BG_Rc_friendPanel.OnInitialize("geniusbaby.LSharpScript.FriendPanel");
            BG_Rc_chatPanel.OnInitialize("geniusbaby.LSharpScript.ChatPanel");
            BG_Rc_mailPanel.OnInitialize("geniusbaby.LSharpScript.MailPanel");
        }
        void __DoUninit()
        {
            BG_Lc_friend_clip_list.OnUnInitialize();
            BG_Lc_mail_clip_list.OnUnInitialize();
            BG_Rc_friendPanel.OnUnInitialize();
            BG_Rc_chatPanel.OnUnInitialize();
            BG_Rc_mailPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_friend_clip_list.OnShow();
            BG_Lc_mail_clip_list.OnShow();
            BG_Rc_friendPanel.OnShow();
            BG_Rc_chatPanel.OnShow();
            BG_Rc_mailPanel.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_friend_clip_list.OnHide();
            BG_Lc_mail_clip_list.OnHide();
            BG_Rc_friendPanel.OnHide();
            BG_Rc_chatPanel.OnHide();
            BG_Rc_mailPanel.OnHide();
        }
//generate code end

        Mail m_mail;
        Friend m_friend;

        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_friend_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(FriendItemPanel).Name);
            BG_Lc_mail_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(MailItemPanel).Name);
            __DoInit();

            BG_Top_friend.onClick.AddListener(() => SwitchPage(1));
            BG_Top_chat.onClick.AddListener(() => SwitchPage(2));
            BG_Top_mail.onClick.AddListener(() => SwitchPage(3));

            BG_Lc_friend_clip_list.selectListener.Add(() =>
            {
                var no = BG_Lc_friend_clip_list.itemSelected.First;
                if (no != null)
                {
                    T.As<FriendPanel>(BG_Rc_friendPanel).Display((Friend)BG_Lc_friend_clip_list.values[no.Value]);
                }
            });
            BG_Lc_friend_clip.onClick.AddListener(() =>
            {
                var no = BG_Lc_friend_clip_list.itemSelected.First;
                T.As<FriendPanel>(BG_Rc_friendPanel).Display(no != null ? (Friend)BG_Lc_friend_clip_list.values[no.Value] : null);
            });
            BG_Lc_friend_accept.onClick.AddListener(() => T.As<FriendPanel>(BG_Rc_friendPanel).DisplayNeedConfirms());
            BG_Lc_friend_search.onClick.AddListener(() => T.As<FriendPanel>(BG_Rc_friendPanel).DisplaySearch());

            BG_Lc_mail_clip_list.selectListener.Add(() =>
            {
                var no = BG_Lc_mail_clip_list.itemSelected.First;
                if (no != null)
                {
                    T.As<MailPanel>(BG_Rc_mailPanel).Display((Mail)BG_Lc_mail_clip_list.values[no.Value]);
                }
            });

            BG_Lc_chat_broadcast.onValueChanged.AddListener(isOn => OnChatSync());
            BG_Lc_chat_union.onValueChanged.AddListener(isOn => OnChatSync());
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

            FriendModule.Instance.onSync.Add(OnFriendSync);
            OnFriendSync();

            MailModule.Instance.onMailSync.Add(OnMailSync);
            OnMailSync();

            ChatModule.Instance.onSync.Add(OnChatSync);
            OnChatSync();
        }
        public override void OnHide()
        {
            MailModule.Instance.onMailSync.Rmv(OnMailSync);

            FriendModule.Instance.onSync.Rmv(OnFriendSync);

            ChatModule.Instance.onSync.Rmv(OnChatSync);
            __DoHide();
            base.OnHide();
        }
        void SwitchPage(int page)
        {
            Util.UnityHelper.ShowHide(BG_Lc_friend, page == 1);
            Util.UnityHelper.ShowHide(BG_Rc_friendPanel, page == 1);
            Util.UnityHelper.ShowHide(BG_Lc_chat, page == 2);
            Util.UnityHelper.ShowHide(BG_Rc_chatPanel, page == 2);
            Util.UnityHelper.ShowHide(BG_Lc_mail, page ==3);
            Util.UnityHelper.ShowHide(BG_Rc_mailPanel, page ==3);
        }
        void OnFriendSync()
        {
            var friends = FriendModule.Instance.friends;
            var oks = new List<object>();
            var needConfirms = new List<object>();
            for (int index = 0; index < friends.Count; ++index)
            {
                switch (friends[index].type)
                {
                    case Friend.Type.Ok: oks.Add(friends[index]); break;
                    case Friend.Type.NeedSelfConfirm: needConfirms.Add(friends[index]); break;
                }
            }
            BG_Lc_friend_clip_list.SetValues(oks);
            //BG_Rc_friend_accept_clip_list.SetValues(needConfirms);
            //Util.UnityHelper.ShowHide(applyTipRoot, FuncTipModule.Instance.GetFuncTip(Function.Friend));
        }
        void OnFriendUpdate(Friend fri)
        {
            if (m_friend.id == fri.id)
            {
                m_friend = fri;
                //Util.UnityHelper.ShowHide(BG_Rc_friend_chat_Op_give, Util.TimerManager.Inst().RealTimeMS() / 1000 > m_friend.giveClock);
            }
        }
        void OnMailSync()
        {
            BG_Lc_mail_count.text = new RangeValue(MailModule.Instance.mails.Count, 30).ToStyle1();
            var mails = MailModule.Instance.mails;
            mails.Sort((x, y) =>
            {
                int result = x.hasRead.CompareTo(y.hasRead);
                if (result != 0) return result;
                return -x.createUtc.CompareTo(y.createUtc);
            });
            BG_Lc_mail_clip_list.SetValues(mails.ConvertAll(it => (object)it));
        }

        void OnChatSync()
        {
            if (BG_Lc_chat_broadcast.isOn)
            {
                T.As<ChatPanel>(BG_Rc_chatPanel).SwitchPage(1);
            }
            else if (BG_Lc_chat_union.isOn)
            {
                T.As<ChatPanel>(BG_Rc_chatPanel).SwitchPage(2);
            }
        }
    }
}
