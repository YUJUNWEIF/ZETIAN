using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ChatNotifyImpl : IProtoImpl<ChatNotify>
	{
//generate code begin
		public int PId() { return PID.ChatNotify; }
		public ChatNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var chats = proto.chats.ConvertAll(it => new ChatInfo(it.talkerId, it.talkerName, it.msg));
            switch (proto.act)
            {
                case NotifyAction.Sy: ChatModule.Inst().Sync(chats); break;
                case NotifyAction.Up: ChatModule.Inst().Add(chats); break;
            }
        }
	}
}
