using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapResourceLoadNotifyImpl : IProtoImpl<GMapResourceLoadNotify>
	{
//generate code begin
		public int PId() { return PID.GMapResourceLoadNotify; }
		public GMapResourceLoadNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            PvpRoomModule.Inst().Sync(proto.combatType);

            switch (proto.combatType)
            {
                case GlobalDefine.GTypeQuiz:
                    QuizSceneManager.Instance.Enter(proto.roomId, proto.players);
                    TcpNetwork.Inst().Send(new pps.FightMapLoadProgressReport() { value = 100 });
                    break;
                default:
                    if (!MainState.Instance.IsState<SubFightState>())
                    {
                        //读条中掉线，重连将会收到resourceload协议，不能二次enter，会将battle情况
                        FightSceneManager.Inst().LoadStart(proto.roomId, proto.combatType, proto.mapId, proto.players);
                        //PvpRoomModule.Inst().Sync(proto.combatType, proto.battleId, proto.mapId, proto.players);
                        MainState.Instance.PushState<SubFightState>();
                    }
                    else if (SceneManager.Inst().GetLoadingProgress() >= 100)
                    {
                        TcpNetwork.Inst().Send(new pps.FightMapLoadProgressReport() { value = 100 });
                    }
                    break;
            }
        }
	}
}
