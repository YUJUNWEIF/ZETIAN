using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendChatNotifyImpl : IProtoImpl<FriendChatNotify>
	{
//generate code begin
		public int PId() { return PID.FriendChatNotify; }
		public FriendChatNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var chats = proto.chats.ConvertAll(it => new FriendChat() { utc = it.utc, meTalk = it.meTalk, other = it.other, msg = it.msg });
            switch (proto.act)
            {
                case NotifyAction.Sy: FriendModule.Inst().SyncChat(chats); break;
                case NotifyAction.Up: FriendModule.Inst().UpdateChat(chats); break;
            }
        }
	}
}
