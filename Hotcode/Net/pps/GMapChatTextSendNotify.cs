using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapChatTextSendNotifyImpl : IProtoImpl<GMapChatTextSendNotify>
	{
//generate code begin
		public int PId() { return PID.GMapChatTextSendNotify; }
		public GMapChatTextSendNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            ChatManager.Inst().TextRecv(proto.talkerSId, proto.msg);
        }
	}
}
