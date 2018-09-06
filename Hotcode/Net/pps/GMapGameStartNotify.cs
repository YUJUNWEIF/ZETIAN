using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class GMapGameStartNotifyImpl : IProtoImpl<GMapGameStartNotify>
	{
//generate code begin
		public int PId() { return PID.GMapGameStartNotify; }
		public GMapGameStartNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            var combatType = PvpRoomModule.Inst().combatType;

            if (combatType == GlobalDefine.GTypeQuiz)
            {
                GuiManager.Inst().HideFrame(typeof(LSharpScript.QuizWaitFrame).Name);
                QuizSceneManager.Inst().Leave(proto.randSeed);
                ClientLockStep.Inst().Enter(QuizSceneManager.mod);
                GuiManager.Inst().ShowFrame(typeof(LSharpScript.QuizFrame).Name);
            }
            else
            {
                FightSceneManager.Inst().LoadFinish(proto.randSeed);
                ClientLockStep.Inst().Enter(FightSceneManager.mod);
                switch (combatType)
                {
                    case GlobalDefine.GTypeFightSingle: GuiManager.Inst().ShowFrame(typeof(LSharpScript.FightSingleFrame).Name); break;
                    case GlobalDefine.GTypeFightMulti_1S1: GuiManager.Inst().ShowFrame(typeof(LSharpScript.FightMultiFrame).Name); break;
                }
                SubFightState.Instance.CompleteSwitch();
            }
        }
    }
}
