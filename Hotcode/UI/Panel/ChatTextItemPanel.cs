using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ChatTextItemPanel : ILSharpScript
    {
//generate code begin
        public Text talker;
        public Text content;
        void __LoadComponet(Transform transform)
        {
            talker = transform.Find("@talker").GetComponent<Text>();
            content = transform.Find("@content").GetComponent<Text>();
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
        TextChatData m_chat;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public IGMapChatData GetValue() { return m_chat; }
        public void SetValue(IGMapChatData value)
        {
            m_chat = (TextChatData)value;
            content.text = m_chat.msg;
        }
    }
}
