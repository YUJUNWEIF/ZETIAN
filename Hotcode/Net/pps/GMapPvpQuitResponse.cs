using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpQuitResponseImpl : IProtoImpl<GMapPvpQuitResponse>
	{
//generate code begin
		public int PId() { return PID.GMapPvpQuitResponse; }
		public GMapPvpQuitResponseImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (proto.ret == 0)
            {
                PvpRoomModule.Inst().Cancel();
            }
        }
	}
}
