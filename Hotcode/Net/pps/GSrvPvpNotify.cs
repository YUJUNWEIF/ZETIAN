using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GSrvPvpNotifyImpl : IProtoImpl<GSrvPvpNotify>
	{
//generate code begin
		public int PId() { return PID.GSrvPvpNotify; }
		public GSrvPvpNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            if (!MainState.IsSubState<SubMainState>())
            {
                GuideModule.Inst().pvp = proto;
                return;
            }

            PvpRoomModule.Inst().onJoin.Fire(proto);
            var report = new pps.GMapPvpReport()
            {
                combatType = proto.combatType,
                mapId = proto.mapId,
            };

            var player = PlayerModule.Inst().player;
            report.pdp = new pps.ProtoDetailPlayer()
            {
                sessionId = proto.sessionId,
                uniqueId = player.id,
                name = player.name,
                lv = player.lvl,
                packageId = KnowledgeModule.Inst().lgapId,
                srvLogic = proto.srvlogic,
            };

            if (proto.combatType == GlobalDefine.GTypeFightMulti_1S1)
            {
                var pet = PetModule.Inst().GetEquiped();
                report.pdp.pet = pet.ToProto();
                for (int index = 0; index < proto.knows.Count; ++index)
                {
                    report.pdp.knows.Add(proto.knows[index]);
                }
            }

            switch (proto.combatType)
            {
                case GlobalDefine.GTypeQuiz: LSharpScript.NetworkWatcher.Inst().Start(QuizSceneManager.Inst(), proto.srvfight, report); break;
                default: LSharpScript.NetworkWatcher.Inst().Start(FightSceneManager.Inst(), proto.srvfight, report); break;
            }
        }
    }
}