using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapChatConnectReportImpl : IProtoImpl<GMapChatConnectReport>
	{
//generate code begin
		public int PId() { return PID.GMapChatConnectReport; }
		public GMapChatConnectReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
