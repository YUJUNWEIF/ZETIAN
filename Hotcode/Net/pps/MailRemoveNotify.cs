using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class MailRemoveNotifyImpl : IProtoImpl<MailRemoveNotify>
	{
//generate code begin
		public int PId() { return PID.MailRemoveNotify; }
		public MailRemoveNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            MailModule.Inst().Remove(proto.id);
        }
	}
}
