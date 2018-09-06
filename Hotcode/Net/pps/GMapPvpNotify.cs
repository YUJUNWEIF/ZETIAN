using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapPvpNotifyImpl : IProtoImpl<GMapPvpNotify>
	{
//generate code begin
		public int PId() { return PID.GMapPvpNotify; }
		public GMapPvpNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
        public override void Process()
        {
            switch (proto.act)
            {
                case NotifyAction.Sy:
                    PvpRoomModule.Inst().Sync(proto.roomId, proto.combatType, proto.needPlayers, proto.players);
                    break;
                case NotifyAction.Up:
                    PvpRoomModule.Inst().Update(proto.players);
                    break;
            }
            PvpRoomModule.Inst().SyncTickDown(proto.tickDownMs, proto.randSeed);
            if (proto.act == NotifyAction.Sy)
            {
                switch (proto.combatType)
                {
                    case GlobalDefine.GTypeQuiz: GuiManager.Inst().ShowFrame(typeof(LSharpScript.QuizWaitFrame).Name); break;
                    case GlobalDefine.GTypeFightSingle:
                    case GlobalDefine.GTypeFightMulti_1S1: GuiManager.Inst().ShowFrame(typeof(LSharpScript.PvpMatchFrame).Name); break;
                }
            }
        }
	}
}
