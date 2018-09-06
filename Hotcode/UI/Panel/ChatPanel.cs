using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ChatPanel : ILSharpScript
    {
//generate code begin
        public LSharpListSuper clip_list;
        public InputField input;
        public Button emotion;
        public Button send;
        void __LoadComponet(Transform transform)
        {
            clip_list = transform.Find("clip/@list").GetComponent<LSharpListSuper>();
            input = transform.Find("@input").GetComponent<InputField>();
            emotion = transform.Find("@emotion").GetComponent<Button>();
            send = transform.Find("@send").GetComponent<Button>();
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
        bool m_cheatOn = false;
        const string cmdCheatOn = "whosyourdaddy";
        const string cmdCheatOff = "iamyourdaddy";
        const string cmdAddItem = "item";
        const string cmdOpenAchieve = "achieve";
        const string cmdLevel = "class";
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(ChatItemPanel).Name);
            __DoInit();

            emotion.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                var frame = GuiManager.Instance.ShowFrame(typeof(EmotionFrame).Name);
                T.As<EmotionFrame>(frame).onUseEmotion = this.OnUseEmotion;
            }));
            send.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(input.text))
                {
                    var result = input.text.TrimStart(' ', '\t').TrimEnd(' ', '\t');
                    if (!string.IsNullOrEmpty(result))
                    {
                        if (result == cmdCheatOn || result == cmdCheatOff)
                        {
                            m_cheatOn = (result == cmdCheatOn);
                            var player = PlayerModule.Inst().player;
                            var special = new ChatInfo(player.id, player.name, result);
                            ChatModule.Instance.Add(special);
                        }
                        else
                        {
                            if (m_cheatOn && (result.StartsWith(cmdAddItem) || result.StartsWith(cmdOpenAchieve) || result.StartsWith(cmdLevel)))
                            {
                                HttpNetwork.Instance.Communicate(new ChatSendRequest() { tp = ChatSendRequest.MsgType.Cheat, msg = result });
                            }
                            else
                            {
                                HttpNetwork.Instance.Communicate(new ChatSendRequest() { tp = ChatSendRequest.MsgType.Normal, msg = result });
                            }
                        }
                    }
                    input.text = string.Empty;
                }
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
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void SwitchPage(int page)
        {
            const int maxRow = 50;
            var chs = new List<object>(50);
            var chats = ChatModule.Instance.chats;
            for (int index = 0; index < chats.Count && index < maxRow; ++index)
            {
                chs.Add(chats[index]);
            }
            clip_list.SetValues(chs);
            //switch (page)
            //{
            //    case 1: break;
            //    case 2: break;
            //}
        }
        void OnUseEmotion(string emotionCmd)
        {
            input.text += ChatModule.CodecEmotion(emotionCmd);
        }
    }
}
