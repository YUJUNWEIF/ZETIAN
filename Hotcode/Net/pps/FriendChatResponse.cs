using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendChatResponseImpl : IProtoImpl<FriendChatResponse>
	{
//generate code begin
		public int PId() { return PID.FriendChatResponse; }
		public FriendChatResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
