using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace geniusbaby
{   
    public class QuizSceneManager : PvpManager<QuizSceneManager>, IGameEvent
    {
        int battleId;
        List<pps.ProtoDetailPlayer> players;
        public static QuizModule mod { get; private set; }
        //public override ILockStepFight mod { get { return m_module; } }

        public void OnStartGame() { }
        public void OnStopGame() { }
        public void Enter(int battleId,  List<pps.ProtoDetailPlayer> players)
        {
            this.battleId = battleId;
            this.players = players;
        }
        public void Leave(int randSeed)
        {
            this.combatState = CombatState.Fighting;
            mod = new QuizModule();
            mod.Initialize(battleId);
            FEModule.Inst().AddBattle(mod);
            var gplayers = new List<QuizPlayerObj>();
            for (int index = 0; index < players.Count; ++index)
            {
                var rp = players[index];
                if (rp.srvLogic != null)
                {
                    gplayers.Add(new QuizPlayerObj(mod, rp, null));
                }
                else
                {
                    var robotCfg = tab.quizRobot.Inst().Find(rp.lv);
                    var ai = new FEQuizPlayerAI(robotCfg.ai);
                    gplayers.Add(new QuizPlayerObj(mod, rp, ai));
                }
            }
            mod.Enter(randSeed, gplayers);
            mod.StartFight();
        }
        public override void OnNosession()
        {
            PvpLeave();
            GuiManager.Inst().HideFrame(typeof(LSharpScript.QuizFrame).Name);
        }
    }
}