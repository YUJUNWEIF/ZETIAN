using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct ChatInfo
    {
        public string talkerId;
        public string talkerName;
        public string codeStr;
        public List<Content> contents;
        public ChatInfo(string uId, string name, string content) : this()
        {
            this.talkerId = uId;
            this.talkerName = name;
            this.codeStr = content;
            contents = new List<Content>();
            Content.Decode(content, contents);
        }
    }
    public class ChatModule : Singleton<ChatModule>, IModule
    {
        const int MaxCount = 1000;
        public List<ChatInfo> chats = new List<ChatInfo>();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { chats.Clear(); }

        public readonly Util.ParamActions onSync = new Util.ParamActions();
        public void Sync(List<ChatInfo> sync)
        {
            chats = sync;
            if (chats.Count > MaxCount)
            {
                chats.RemoveRange(0, this.chats.Count - MaxCount);
            }
            onSync.Fire();
        }
        public void Add(List<ChatInfo> adds)
        {
            for (int index = 0; index < adds.Count; ++index)
            {
                chats.Add(adds[index]);
            }
            if (chats.Count > MaxCount)
            {
                chats.RemoveRange(0, chats.Count - MaxCount);
            }
            onSync.Fire();
        }
        public void Add(ChatInfo add)
        {
            chats.Add(add);
            if (chats.Count > MaxCount)
            {
                chats.RemoveRange(0, chats.Count - MaxCount);
            }
            onSync.Fire();
        }
        public static string CodecEmotion(string emotionKey)
        {
            return new System.Text.StringBuilder().Append("<e ").Append(emotionKey).Append("/>").ToString();
        }
        public static string CodecHref(string value, string display)
        {
            return new System.Text.StringBuilder().Append("<a href=").Append(value).Append('>').Append(display).Append("</a>").ToString();
        }
    }
}
