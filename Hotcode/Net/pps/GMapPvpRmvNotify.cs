using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpRmvNotifyImpl : IProtoImpl<GMapPvpRmvNotify>
	{
//generate code begin
		public int PId() { return PID.GMapPvpRmvNotify; }
		public GMapPvpRmvNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            PvpRoomModule.Inst().Rmv(proto.playerId);
            //if (PvpRoomModule.Inst().combatType == GlobalDefine.GTypeQuiz)
            //{
            //    var players = PvpRoomModule.Inst().players;
            //    var qplayers = players.ConvertAll(
            //        it => new QuizPlayer()
            //        {
            //            uniqueId = it.brief.uniqueId,
            //            name = it.brief.name,
            //            sessionId = it.sessionId,
            //            titleId = it.brief.titleId,
            //            titleLv = it.brief.titleLv,
            //            replyTimes = new List<int>(),
            //        });
            //    QuizModule.Inst().Sync(-1, PvpRoomModule.Inst().param, qplayers);
            //}
        }
	}
}
