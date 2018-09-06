using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendRemoveRequestImpl : IProtoImpl<FriendRemoveRequest>
	{
//generate code begin
		public int PId() { return PID.FriendRemoveRequest; }
		public FriendRemoveRequestImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
