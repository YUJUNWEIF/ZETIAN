using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PlayerStartGameResponseImpl : IProtoImpl<PlayerStartGameResponse>
	{
//generate code begin
		public readonly static int PID = 202;
		public int PId() { return PID; }
		public PlayerStartGameResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process() { }
	}
}
