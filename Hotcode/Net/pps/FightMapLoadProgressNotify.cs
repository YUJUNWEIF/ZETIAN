using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class FightMapLoadProgressNotifyImpl : IProtoImpl<FightMapLoadProgressNotify>
	{
//generate code begin
		public int PId() { return PID.FightMapLoadProgressNotify; }
		public FightMapLoadProgressNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            for (int index = 0; index < proto.progress.Count; ++index)
            {
                if (proto.progress[index].value < 100) { return; }
            }
        }
    }
}
