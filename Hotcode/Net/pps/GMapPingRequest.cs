using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPingRequestImpl : IProtoImpl<GMapPingRequest>
	{
//generate code begin
		public int PId() { return PID.GMapPingRequest; }
		public GMapPingRequestImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
