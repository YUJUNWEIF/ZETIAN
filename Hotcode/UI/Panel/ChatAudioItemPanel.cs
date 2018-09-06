using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ChatAudioItemPanel : ILSharpScript
    {
//generate code begin
        public Text talker;
        public Text content;
        public Image content_newly;
        void __LoadComponet(Transform transform)
        {
            talker = transform.Find("@talker").GetComponent<Text>();
            content = transform.Find("@content").GetComponent<Text>();
            content_newly = transform.Find("@content/@newly").GetComponent<Image>();
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
        IGMapChatData m_chat;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                m_chat.Play();
            });
        }
        public IGMapChatData GetValue(){ return m_chat; }
        public void SetValue(IGMapChatData value)
        {
            m_chat = value;
            //content.contents = m_chat.contents;
        }
    }
}
