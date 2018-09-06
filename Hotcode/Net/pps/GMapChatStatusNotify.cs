using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapChatStatusNotifyImpl : IProtoImpl<GMapChatStatusNotify>
	{
//generate code begin
		public int PId() { return PID.GMapChatStatusNotify; }
		public GMapChatStatusNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            ChatManager.Inst().Online(proto.sId);
        }
	}
}
