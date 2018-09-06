using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class TimeNotifyImpl : IProtoImpl<TimeNotify>
	{
//generate code begin
		public int PId() { return PID.TimeNotify; }
		public TimeNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            Util.TimerManager.Inst().Synchronize(proto.timestamp);
        }
	}
}
