using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpConfirmReportImpl : IProtoImpl<GMapPvpConfirmReport>
	{
//generate code begin
		public int PId() { return PID.GMapPvpConfirmReport; }
		public GMapPvpConfirmReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
