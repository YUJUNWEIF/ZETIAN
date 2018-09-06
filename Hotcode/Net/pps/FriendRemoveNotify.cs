using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FriendRemoveNotifyImpl : IProtoImpl<FriendRemoveNotify>
	{
//generate code begin
		public int PId() { return PID.FriendRemoveNotify; }
		public FriendRemoveNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            FriendModule.Inst().Rmv(proto.id);
        }
	}
}
