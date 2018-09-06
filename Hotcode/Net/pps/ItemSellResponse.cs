using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ItemSellResponseImpl : IProtoImpl<ItemSellResponse>
	{
//generate code begin
		public int PId() { return PID.ItemSellResponse; }
		public ItemSellResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
            }
            else
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
