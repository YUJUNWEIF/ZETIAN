using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapChatAudioSendNotifyImpl : IProtoImpl<GMapChatAudioSendNotify>
	{
//generate code begin
		public int PId() { return PID.GMapChatAudioSendNotify; }
		public GMapChatAudioSendNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            ChatManager.Inst().AudioRecv(proto.talkerSId, proto.data);
        }
	}
}
