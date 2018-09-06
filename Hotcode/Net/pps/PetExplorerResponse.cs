using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetExplorerResponseImpl : IProtoImpl<PetExplorerResponse>
	{
//generate code begin
		public int PId() { return PID.PetExplorerResponse; }
		public PetExplorerResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
