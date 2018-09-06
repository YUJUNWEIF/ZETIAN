using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ShopBuyResponseImpl : IProtoImpl<ShopBuyResponse>
	{
//generate code begin
		public int PId() { return PID.ShopBuyResponse; }
		public ShopBuyResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret != 0) { ProtoUtil.ErrRet(proto.ret); }
        }
	}
}
