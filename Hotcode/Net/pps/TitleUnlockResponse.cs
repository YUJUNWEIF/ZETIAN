using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class TitleUnlockResponseImpl : IProtoImpl<TitleUnlockResponse>
	{
//generate code begin
		public int PId() { return PID.TitleUnlockResponse; }
		public TitleUnlockResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret != 0) { ProtoUtil.ErrRet(proto.ret); }
        }
	}
}
