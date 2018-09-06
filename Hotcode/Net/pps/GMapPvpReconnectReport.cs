using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpReconnectReportImpl : IProtoImpl<GMapPvpReconnectReport>
	{
//generate code begin
		public int PId() { return PID.GMapPvpReconnectReport; }
		public GMapPvpReconnectReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
