using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpConfirmNotifyImpl : IProtoImpl<GMapPvpConfirmNotify>
	{
//generate code begin
		public int PId() { return PID.GMapPvpConfirmNotify; }
		public GMapPvpConfirmNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            TcpNetwork.Inst().Send(new pps.GMapPvpConfirmReport());
        }
	}
}
