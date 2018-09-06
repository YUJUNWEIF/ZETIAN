using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GuideNotifyImpl : IProtoImpl<GuideNotify>
	{
//generate code begin
		public int PId() { return PID.GuideNotify; }
		public GuideNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            GuideModule.Inst().Sync(proto.finishGuideId);
        }
	}
}
