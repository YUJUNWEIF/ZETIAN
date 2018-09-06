using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FightMapLoadProgressReportImpl : IProtoImpl<FightMapLoadProgressReport>
	{
//generate code begin
		public int PId() { return PID.FightMapLoadProgressReport; }
		public FightMapLoadProgressReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            //var battle = FEModule.Inst().Find(0);
            //battle.UpdateLoadProgress(PlayerModule.MyId(), proto.value);
            
            FightSceneManager.Inst().Refresh(PlayerModule.MyId(), proto.value);
        }
	}
}
