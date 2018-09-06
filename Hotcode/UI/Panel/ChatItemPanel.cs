using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ChatItemPanel : ILSharpScript
    {
//generate code begin
        public TextImage2D textImage;
        void __LoadComponet(Transform transform)
        {
            textImage = transform.Find("@textImage").GetComponent<TextImage2D>();
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
        ChatInfo m_chat;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public void SetValue(ChatInfo value)
        {
            m_chat = value;
            textImage.contents = m_chat.contents;
        }
        public object GetValue()
        {
            return m_chat;
        }
    }
}
