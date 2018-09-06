using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public class Scenario
    {
        LinkedList<cfg.scenario._sub> m_subs = new LinkedList<cfg.scenario._sub>();
        public void InitScenario()
        {
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
            if (GuideModule.Inst().tutoring)
            {
                var guide = GuideModule.Inst().guide;
                if (guide.guideCfg.taskType.type == cfg.guide.InsideCombat)
                {
                    //ui.GuideMaskFrame.RestoreGuide(guide);
                }
            }
            m_subs.Clear();
            //var mapCfg = tab.map.Inst().Find(FightSceneManager.Inst().mapId);
            //var scenerioCfg = tab.scenario.Inst().Find(mapCfg.scenerioId);
            //if (scenerioCfg != null)
            //{
            //    if (scenerioCfg.subs.Count > 0) { m_subs = new LinkedList<cfg.scenario._sub>(scenerioCfg.subs); }
            //}
        }
        public void UnInitScenario()
        {
            m_subs.Clear();
            Util.TimerManager.Inst().onFrameUpdate.Rmv(OnUpdate);
        }
        void OnUpdate()
        {
            var no = m_subs.First;
            while (no!= null)
            {
                var tmp = no;
                no = no.Next;
                var sub = tmp.Value;
                if (!CheckCond(sub)) {  continue; }
                m_subs.Remove(tmp);

                switch (sub.content.contentType)
                {
                    case cfg.scenario.ContentType.AcceptGuideTask:
                        GuideModule.Inst().ScenarioAccept(sub.content.param);
                        //ui.GuideMaskFrame.RestoreGuide(GuideModule.Inst().guide);
                        break;
                    case cfg.scenario.ContentType.Dialog:
                        FightSceneManager.Instance.pause = true;                        
                        //ui.DialogFrame.NotifyWhenFinished(sub.content.param, () => FightSceneManager.Instance.pause = false);
                        break;
                    case cfg.scenario.ContentType.Summon:
                        {
                            //var battle = FEModule.Inst().FindAs<IFEBattle>(0);
                            //var emitter = tab.monsterEmitter.Inst().Find(sub.content.param);
                            //new FEFishEmitter(battle, emitter, 0, 0).OnLockStepUpdate(0);
                        }
                        break;
                    case cfg.scenario.ContentType.AlterMp:
                        {
                            var battle = FEModule.Inst().FindAs<IFEBattle>(0);
                            var player = battle.FindPlayer(PlayerModule.MyId()) as FEDefenderObj;
                            player.pet.MpReset(sub.content.param);
                        }
                        break;
                    case cfg.scenario.ContentType.FishEmitterOpen:
                        {
                            var battle = FEModule.Inst().FindAs<IFEBattle>(0);
                            battle.level.Trigger(sub.content.param, true);
                        }
                        break;
                    case cfg.scenario.ContentType.FishEmitterClose:
                        {
                            var battle = FEModule.Inst().FindAs<IFEBattle>(0);
                            battle.level.Trigger(sub.content.param, false);
                        }
                        break;
                }
            }
        }
        bool CheckCond(cfg.scenario._sub sudden)
        {
            var battle = FEModule.Inst().FindAs<IFEBattle>(0);
            switch (sudden.time.timeType)
            {
                case cfg.scenario.TimeType.TimeAbsolute: return sudden.time.param < battle.timeMs;
                case cfg.scenario.TimeType.Round: return FightSceneManager.mod.level.index >= sudden.time.param;
            }
            return false;
        }
    }
}
