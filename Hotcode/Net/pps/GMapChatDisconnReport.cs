using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapChatDisconnReportImpl : IProtoImpl<GMapChatDisconnReport>
	{
//generate code begin
		public int PId() { return PID.GMapChatDisconnReport; }
		public GMapChatDisconnReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
