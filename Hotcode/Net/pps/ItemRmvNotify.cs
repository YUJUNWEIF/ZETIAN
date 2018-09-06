using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ItemRmvNotifyImpl : IProtoImpl<ItemRmvNotify>
	{
//generate code begin
		public int PId() { return PID.ItemRmvNotify; }
		public ItemRmvNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            PackageModule.Inst().Rmv(proto.stackId);
        }
	}
}
