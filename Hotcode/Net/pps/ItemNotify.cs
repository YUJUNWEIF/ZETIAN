using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ItemNotifyImpl : IProtoImpl<ItemNotify>
	{
//generate code begin
		public int PId() { return PID.ItemNotify; }
		public ItemNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var items = proto.items.ConvertAll(it => new PackageItem(it.stackId, it.id) { count = it.count });
            switch (proto.act)
            {
                case NotifyAction.Sy: PackageModule.Inst().Sync(items); break;
                case NotifyAction.Up: PackageModule.Inst().Update(items); break;
            }
        }
	}
}
