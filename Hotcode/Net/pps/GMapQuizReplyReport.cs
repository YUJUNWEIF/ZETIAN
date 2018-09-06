using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapQuizReplyReportImpl : IProtoImpl<GMapQuizReplyReport>
	{
//generate code begin
		public int PId() { return PID.GMapQuizReplyReport; }
		public GMapQuizReplyReportImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var qs = QuizSceneManager.mod.GetState() as QuizRoundRunning;
            if (qs!= null)
            {
                qs.Answer(nsession.playerId, proto.round, proto.answer);
            }
        }
	}
}
