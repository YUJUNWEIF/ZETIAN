using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpReportImpl : IProtoImpl<GMapPvpReport>
	{
//generate code begin
		public int PId() { return PID.GMapPvpReport; }
		public GMapPvpReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
