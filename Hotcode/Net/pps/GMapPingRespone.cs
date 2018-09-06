using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPingResponeImpl : IProtoImpl<GMapPingRespone>
	{
//generate code begin
		public int PId() { return PID.GMapPingRespone; }
		public GMapPingResponeImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            TcpNetwork.Inst().delay = Util.TimerManager.Inst().RealTimeMS() - proto.utcMs;
            //ClientLockStep.Inst().Synchronize(proto.utcMs);
        }
	}
}
