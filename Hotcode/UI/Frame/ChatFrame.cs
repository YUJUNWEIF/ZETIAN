using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ChatFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper BG_clip_list;
        public InputField BG_input;
        public Button BG_emotion;
        public Button BG_send;
        public Image BG_close;
        void __LoadComponet(Transform transform)
        {
            BG_clip_list = transform.Find("BG/clip/@list").GetComponent<LSharpListSuper>();
            BG_input = transform.Find("BG/@input").GetComponent<InputField>();
            BG_emotion = transform.Find("BG/@emotion").GetComponent<Button>();
            BG_send = transform.Find("BG/@send").GetComponent<Button>();
            BG_close = transform.Find("BG/@close").GetComponent<Image>();
        }
        void __DoInit()
        {
            BG_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            BG_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_clip_list.OnShow();
        }
        void __DoHide()
        {
            BG_clip_list.OnHide();
        }
//generate code end
        public int maxRow = 50;
        bool m_needAutoRoll = false;
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
            //BG_clip_list.listItem = 
            __DoInit();
            BG_emotion.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                var frame = GuiManager.Instance.ShowFrame(typeof(EmotionFrame).Name);
                var script = T.As<EmotionFrame>(frame);
                script.onUseEmotion = OnUseEmotion;
            }));
            BG_send.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(BG_input.text))
                {
                    var result = BG_input.text.TrimStart(' ', '\t').TrimEnd(' ', '\t');
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
                    BG_input.text = string.Empty;
                }
            });
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
            });
        }
        void OnUseEmotion(string emotionCmd)
        {
            BG_input.text += ChatModule.CodecEmotion(emotionCmd);
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
            ChatModule.Instance.onSync.Add(OnSync);
            OnSync();
            m_needAutoRoll = true;
        }
        public override void OnHide()
        {
            api.coroutineHelper.StopAll();
            ChatModule.Instance.onSync.Rmv(OnSync);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            var chats = ChatModule.Instance.chats;
            if (chats.Count > maxRow)
            {
                var remove = chats.Count - maxRow;
                chats.RemoveRange(0, chats.Count - maxRow);
            }
            BG_clip_list.SetValues(chats.ConvertAll(it => (object)it));
            //var player = PlayerModule.Instance.player;
            //m_needAutoRoll = current.talkerId == player.id;
        }
        //void Update()
        //{
        //    bool sizeChanged = false;
        //    for (int index = 0; index < chatPanel.count; ++index)
        //    {
        //        var it = chatPanel.GetItem(index);
        //        if (it.content.sizeChanged)
        //        {
        //            sizeChanged = true;
        //            it.content.sizeChanged = false;
        //        }
        //    }
        //    if (sizeChanged)
        //    {
        //        for (int index = 0; index < chatPanel.count; ++index)
        //        {
        //            var it = chatPanel.GetItem(index);
        //            it.Resize();
        //        }
        //        chatPanel.FireListItemChangeNotify();
        //        if (m_needAutoRoll)
        //        {
        //            m_needAutoRoll = false;
        //            var cachedRc = (RectTransform)chatPanel.transform;
        //            var offsetY = cachedRc.rect.height - groupClipWindow.rect.height;
        //            cachedRc.anchoredPosition = new Vector2(cachedRc.anchoredPosition.x, offsetY > 0 ? offsetY : 0);
        //        }
        //    }
        //}
    }
}
