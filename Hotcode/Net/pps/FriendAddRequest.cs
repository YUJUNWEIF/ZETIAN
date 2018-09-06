using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendAddRequestImpl : IProtoImpl<FriendAddRequest>
	{
//generate code begin
		public int PId() { return PID.FriendAddRequest; }
		public FriendAddRequestImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
