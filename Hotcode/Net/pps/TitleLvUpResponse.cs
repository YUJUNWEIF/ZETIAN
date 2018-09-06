using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class TitleLvUpResponseImpl : IProtoImpl<TitleLvUpResponse>
	{
//generate code begin
		public int PId() { return PID.TitleLvUpResponse; }
		public TitleLvUpResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret != 0) { ProtoUtil.ErrRet(proto.ret); }
        }
	}
}
