using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class AchieveNotifyImpl : IProtoImpl<AchieveNotify>
	{
//generate code begin
		public int PId() { return PID.AchieveNotify; }
		public AchieveNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var achieves = proto.achieves.ConvertAll(it => new Achieve() { id = it.id, lv = it.lv });
            switch (proto.act)
            {
                case NotifyAction.Sy: AchieveModule.Inst().Sync(achieves); break;
                case NotifyAction.Up: AchieveModule.Inst().Update(achieves); break;
            }
        }
	}
}
