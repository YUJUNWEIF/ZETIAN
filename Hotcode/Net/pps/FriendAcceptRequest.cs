using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendAcceptRequestImpl : IProtoImpl<FriendAcceptRequest>
	{
//generate code begin
		public int PId() { return PID.FriendAcceptRequest; }
		public FriendAcceptRequestImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
