using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetExplorerSlotUseResponseImpl : IProtoImpl<PetExplorerSlotUseResponse>
	{
//generate code begin
		public int PId() { return PID.PetExplorerSlotUseResponse; }
		public PetExplorerSlotUseResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
