using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ShopNotifyImpl : IProtoImpl<ShopNotify>
	{
//generate code begin
		public int PId() { return PID.ShopNotify; }
		public ShopNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var si = proto.items.ConvertAll(it => new ShopItem() { slotId = it.slotId, shopId = it.shopId, itemId = it.itemId, onsale = it.onsale, sellout = it.sellout });
            switch (proto.act)
            {
                case NotifyAction.Sy: ShopModule.Inst().Sync(proto.tickDown, si); break;
                case NotifyAction.Up: ShopModule.Inst().Update(proto.tickDown, si); break;
            }
        }
	}
}
