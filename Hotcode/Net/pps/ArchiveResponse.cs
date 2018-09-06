using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ArchiveResponseImpl : IProtoImpl<ArchiveResponse>
	{
//generate code begin
		public int PId() { return PID.ArchiveResponse; }
		public ArchiveResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
