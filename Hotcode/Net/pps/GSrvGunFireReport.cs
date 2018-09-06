using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GSrvGunFireReportImpl : IProtoImpl<GSrvGunFireReport>
	{
//generate code begin
		public int PId() { return PID.GSrvGunFireReport; }
		public GSrvGunFireReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var playerObj = FightSceneManager.Inst().FindPlayer(proto.castId);
            if (playerObj != null)
            {
                playerObj.NotifyFireAt(proto.targetType, proto.fireAt);
            }
        }
    }
}
