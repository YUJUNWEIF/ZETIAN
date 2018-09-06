using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ArchiveNotifyImpl : IProtoImpl<ArchiveNotify>
	{
//generate code begin
		public int PId() { return PID.ArchiveNotify; }
		public ArchiveNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            ArchiveModule.Inst().Sync(proto.data);
        }
	}
}
