using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class ClassFinishResponseImpl : IProtoImpl<ClassFinishResponse>
	{
//generate code begin
		public int PId() { return PID.ClassFinishResponse; }
		public ClassFinishResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
            }
        }
	}
}
