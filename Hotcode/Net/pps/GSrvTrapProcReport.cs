using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GSrvTrapProcReportImpl : IProtoImpl<GSrvTrapProcReport>
	{
//generate code begin
		public int PId() { return PID.GSrvTrapProcReport; }
		public GSrvTrapProcReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var battle = FEModule.Inst().FindAs<IFEBattle>(nsession.roomId);
            if (battle != null && battle.state != FightState.Finish)
            {
                var defender = battle.FindPlayer(nsession.playerId) as FEDefenderObj;
                if (defender != null) defender.ProcTrap(proto.coord);
            }
        }
	}
}
