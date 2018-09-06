using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ItemSortResponseImpl : IProtoImpl<ItemSortResponse>
	{
//generate code begin
		public int PId() { return PID.ItemSortResponse; }
		public ItemSortResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
