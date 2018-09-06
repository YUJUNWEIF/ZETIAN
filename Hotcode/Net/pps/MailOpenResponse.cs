using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class MailOpenResponseImpl : IProtoImpl<MailOpenResponse>
	{
//generate code begin
		public int PId() { return PID.MailOpenResponse; }
		public MailOpenResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret != 0)
            {
                ProtoUtil.ErrRet(proto.ret);
            }
        }
	}
}
