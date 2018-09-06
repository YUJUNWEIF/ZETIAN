using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ShopResponseImpl : IProtoImpl<ShopResponse>
	{
//generate code begin
		public int PId() { return PID.ShopResponse; }
		public ShopResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
