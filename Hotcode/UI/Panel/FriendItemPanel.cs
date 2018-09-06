using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FriendItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        public Text lvl;
        public Text union;
        public Image online;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            lvl = transform.Find("@lvl").GetComponent<Text>();
            union = transform.Find("@union").GetComponent<Text>();
            online = transform.Find("@online").GetComponent<Image>();
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
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
        }
        public void SetValue(Friend value) { OnUpdate(m_friend = value); }
        public object GetValue() { return m_friend; }
        public override void OnShow()
        {
            base.OnShow();
            FriendModule.Inst().onUpdate.Add(OnUpdate);
        }
        public override void OnHide()
        {
            FriendModule.Inst().onUpdate.Rmv(OnUpdate);
            base.OnHide();
        }
        void OnUpdate(Friend fri)
        {
            if (m_friend.id == fri.id)
            {
                m_friend = fri;
                name.text = m_friend.name;
                lvl.text = "lv.1";
                //Util.UnityHelper.ShowHide(giveButton, Util.TimerManager.Inst().RealTimeMS() / 1000 > m_friend.giveClock);
                Util.UnityHelper.ShowHide(online, m_friend.isOnline);
            }
        }
    }
}
