using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FriendPanel : ILSharpScript
    {
//generate code begin
        public RectTransform chat;
        public LSharpListSuper chat_clip_list;
        public InputField chat_Op_input;
        public Button chat_Op_emotion;
        public Button chat_Op_send;
        public Button chat_Op_delete;
        public Button chat_Op_visit;
        public Button chat_Op_give;
        public Button chat_Op_clear;
        public RectTransform accept;
        public LSharpListSuper accept_clip_list;
        public RectTransform search;
        public InputField search_input;
        public Button search_ok;
        void __LoadComponet(Transform transform)
        {
            chat = transform.Find("@chat").GetComponent<RectTransform>();
            chat_clip_list = transform.Find("@chat/clip/@list").GetComponent<LSharpListSuper>();
            chat_Op_input = transform.Find("@chat/Op/@input").GetComponent<InputField>();
            chat_Op_emotion = transform.Find("@chat/Op/@emotion").GetComponent<Button>();
            chat_Op_send = transform.Find("@chat/Op/@send").GetComponent<Button>();
            chat_Op_delete = transform.Find("@chat/Op/@delete").GetComponent<Button>();
            chat_Op_visit = transform.Find("@chat/Op/@visit").GetComponent<Button>();
            chat_Op_give = transform.Find("@chat/Op/@give").GetComponent<Button>();
            chat_Op_clear = transform.Find("@chat/Op/@clear").GetComponent<Button>();
            accept = transform.Find("@accept").GetComponent<RectTransform>();
            accept_clip_list = transform.Find("@accept/clip/@list").GetComponent<LSharpListSuper>();
            search = transform.Find("@search").GetComponent<RectTransform>();
            search_input = transform.Find("@search/@input").GetComponent<InputField>();
            search_ok = transform.Find("@search/@ok").GetComponent<Button>();
        }
        void __DoInit()
        {
            chat_clip_list.OnInitialize();
            accept_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            chat_clip_list.OnUnInitialize();
            accept_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            chat_clip_list.OnShow();
            accept_clip_list.OnShow();
        }
        void __DoHide()
        {
            chat_clip_list.OnHide();
            accept_clip_list.OnHide();
        }
//generate code end
        Friend m_friend;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            chat_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(ChatItemPanel).Name);
            accept_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(FriendApplyItemPanel).Name);

            __DoInit();

            search_ok.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(search_input.text))
                {
                    HttpNetwork.Inst().Communicate(new FriendAddRequest() { playerId = search_input.text });
                }
            });
            chat_Op_give.onClick.AddListener(() =>
            {
                HttpNetwork.Inst().Communicate(new FriendGiveRequest() { id = m_friend.id });
            });
            chat_Op_delete.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                switch (m_friend.type)
                {
                    case Friend.Type.Ok:
                        {
                            var script = GuiManager.Instance.ShowFrame(typeof(MessageBoxFrame).Name);
                            var frame = T.As<MessageBoxFrame>(script);
                            frame.SetDelegater(() => HttpNetwork.Inst().Communicate(new FriendRemoveRequest() { id = m_friend.id }));
                            frame.SetDesc("remove friend ?");
                        }
                        break;
                }
            }));
            chat_Op_visit.onClick.AddListener(() =>
            {
                HttpNetwork.Inst().Communicate(new FriendVisitRequest() { id = m_friend.id });
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
            FriendModule.Instance.onSync.Add(OnFriendSync);
            FriendModule.Instance.onUpdate.Add(OnFriendUpdate);
            FriendModule.Instance.onChatSync.Add(OnChatSync);
        }
        public override void OnHide()
        {
            FriendModule.Instance.onSync.Rmv(OnFriendSync);
            FriendModule.Instance.onUpdate.Rmv(OnFriendUpdate);
            FriendModule.Instance.onChatSync.Rmv(OnChatSync);
            __DoHide();
            base.OnHide();
        }
        public void Display(Friend fri)
        {
            m_friend = fri;
            SwitchPage(1);
        }
        public void DisplayNeedConfirms()
        {
            SwitchPage(2);
        }
        public void DisplaySearch()
        {
            SwitchPage(3);
        }
        void SwitchPage(int page)
        {
            chat.gameObject.SetActive(page == 1);
            accept.gameObject.SetActive(page == 2);
            search.gameObject.SetActive(page == 3);
            switch (page)
            {
                case 1: OnChatSync(); break;
                case 2: OnFriendSync(); break;
                case 3: break;
            }
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
            accept_clip_list.SetValues(needConfirms);
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
        void OnChatSync()
        {
            var chats = FriendModule.Instance.chats;
            var privateChats = new List<object>();
            for (int index = 0; index < chats.Count; ++index)
            {
                if (chats[index].other == m_friend.id)
                {
                    if (chats[index].meTalk)
                    {
                        var player = PlayerModule.Inst().player;
                        privateChats.Add(new ChatInfo(player.id, player.name, chats[index].msg));
                    }
                    else
                    {
                        var fri = FriendModule.Inst().friends.Find(it => it.id == chats[index].other);
                        privateChats.Add(new ChatInfo(fri.id, fri.name, chats[index].msg));
                    }
                }
            }
            chat_clip_list.SetValues(privateChats);
        }
    }
}
