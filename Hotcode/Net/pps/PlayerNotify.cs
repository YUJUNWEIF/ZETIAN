using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PlayerNotifyImpl : IProtoImpl<PlayerNotify>
	{
//generate code begin
		public int PId() { return PID.PlayerNotify; }
		public PlayerNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var player = new Player()
            {
                id = proto.uniqueId,
                name = proto.name,
            };
            PlayerModule.Inst().Sync(player);
        }
	}
}
