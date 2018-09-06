using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetExplorerSlotUnlockResponseImpl : IProtoImpl<PetExplorerSlotUnlockResponse>
	{
//generate code begin
		public int PId() { return PID.PetExplorerSlotUnlockResponse; }
		public PetExplorerSlotUnlockResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
