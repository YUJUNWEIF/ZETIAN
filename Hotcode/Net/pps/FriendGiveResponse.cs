using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendGiveResponseImpl : IProtoImpl<FriendGiveResponse>
	{
//generate code begin
		public int PId() { return PID.FriendGiveResponse; }
		public FriendGiveResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
