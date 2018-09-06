using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FriendApplyItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        public Text lv;
        public Button refuse;
        public Button accept;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            lv = transform.Find("@lv").GetComponent<Text>();
            refuse = transform.Find("@refuse").GetComponent<Button>();
            accept = transform.Find("@accept").GetComponent<Button>();
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
        Friend m_friend;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            accept.onClick.AddListener(() =>
            {
                HttpNetwork.Inst().Communicate(new FriendAcceptRequest() { playerId = m_friend.id });
            });
            refuse.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                var frame = GuiManager.Instance.ShowFrame(typeof(MessageBoxFrame).Name);
                var script = T.As<MessageBoxFrame>(frame);
                script.SetDelegater(() => HttpNetwork.Inst().Communicate(new FriendRemoveRequest() { id = m_friend.id }));
                script.SetDesc("delete friend ? ");
            }));
        }
        public void SetValue(Friend friend)
        {
            m_friend = friend;
            name.text = m_friend.name;
            lv.text = "Lv.1";
        }
        public Friend GetValue() { return m_friend; }
    }
}
