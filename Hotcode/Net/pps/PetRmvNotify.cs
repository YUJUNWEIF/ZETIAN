using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetRmvNotifyImpl : IProtoImpl<PetRmvNotify>
	{
//generate code begin
		public int PId() { return PID.PetRmvNotify; }
		public PetRmvNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            PetModule.Inst().PetRmv(proto.uIds);
        }
	}
}
