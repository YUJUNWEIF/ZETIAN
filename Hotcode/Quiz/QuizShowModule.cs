using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public interface IQuizAI : ILockStepUpdate
    {
        void Bind(QuizPlayerObj playerObj);
        void UnBind();
        void OnRoundNew();
        void OnRoundOver();
    }
    public class QuizModule : FEStateManager, ILockStepFight
    {
        int m_randSeed;
        int m_timeUsedMs;
        Util.FastRandom m_rand;
        List<cfg.word> m_knowledge;
        public Util.FastRandom rand { get { return m_rand; } }
        public int roomId { get; private set; }
        public IList<QuizPlayerObj> players { get; private set; }
        public IList<QuizRoundRank> ranks { get; private set; }
        public IFEBattleState<QuizModule, QuizState> bstate { get { return GetState() as IFEBattleState<QuizModule, QuizState>; } }
        public QuizState state { get { return bstate != null ? bstate.state : QuizState.Unknown; } }
        public EnglishSwap english = new EnglishSwap();
        public RangeValue progress;

        public Util.Param1Actions<QuizPlayerObj> onEnglishReplied = new Util.Param1Actions<QuizPlayerObj>();
        public Util.ParamActions onResult = new Util.ParamActions();

        static QuizPlayerObj m_me;
        public static QuizPlayerObj Me() { return m_me; }
        public static int MySId() { return m_me.sessionId; }

        public cfg.quizRule ruleCfg { get; private set; }
        bool m_stopNextFrame;
        
        public int randSeed { get { return m_randSeed; } }
        public uint DebugGetRSeed() { return m_rand.persudoSeed; }
        public int DebugGetUniqueId() { return 0; }
        Net.NetSession tmpSession = new Net.NetSession();
        Net.LockStep tmpStep = new Net.LockStep();
        public int stepId { get; private set; }
        public int timeMs { get; private set; }
        public int deltaMs { get { return GlobalParam.game.lockStepInterval; } }
        public void Process(Net.LockStep curStep)
        {
            stepId = curStep.stepId;
            tmpSession.roomId = roomId;
            for (int index = 0; index < curStep.steps.Count; ++index)
            {
                var sId = curStep.sIds[index];
                var proto = curStep.steps[index];
                tmpSession.playerId = FindPlayer(sId).pdp.uniqueId;
                var factory = Net.ProtoManager.manager.GetImpl(proto);
                factory(proto, tmpSession).Process();
                Net.ProtoManager.manager.Free(proto);
            }
        }

        public void Broadcast(object proto, string except = null) { }
        public void CollectStep(Net.NetSession session, object proto) { }
        public void OnLinkBroken(string playerId) { }
        public void OnReconnect(string playerId, int value) { }
        public void Send(string playerId, object proto) { }
        public void PlayerReconnect(IPlayer player) { }

        public void Initialize(int battleId)
        {
            this.roomId = battleId;
            this.ruleCfg = tab.quizRule.Inst().RecordArray[0];
        }
        public void UnInitialize()
        {
            for (int index = 0; index <players.Count; ++index) { players[index].UnInitialize(); }
            FEModule.Inst().RmvBattle(roomId);
        }
        public QuizPlayerObj FindPlayer(string playerId)
        {
            for (int index = 0; index < players.Count; ++index)
            {
                var it = players[index];
                if (it.pdp.uniqueId == playerId) { return it; }
            }
            return null;
        }
        public QuizPlayerObj FindPlayer(int sessionId)
        {
            for (int index = 0; index < players.Count; ++index)
            {
                var it = players[index];
                if (it.sessionId == sessionId) { return it; }
            }
            return null;
        }
        public void Enter(int randSeed, List<QuizPlayerObj> players)
        {
            this.m_randSeed = randSeed;
            this.players = players;
            var exist = players.FindIndex(it => it.pdp.uniqueId == PlayerModule.MyId());
            m_me = players[exist];
            players[exist] = players[0];
            players[0] = m_me;
        }
        public void StartFight()
        {
            progress = new RangeValue(0, 2);
            m_rand = new Util.FastRandom((uint)m_randSeed);
            m_knowledge = new List<cfg.word>();
            var lgap = KnowledgeModule.Inst().FindLGAP();
            for (int index = 0; index < progress.max; ++index)
            {
                var p = lgap[m_rand.Next(lgap.Count)];
                var wordCfg = p.words[m_rand.Next(p.words.Count)];
                if (cfg.word.Valid(wordCfg.english))
                m_knowledge.Add(wordCfg);
            }
            for (int index = 0; index < players.Count; ++index)
            {
                players[index].Initialize(m_knowledge.Count);
            }
            ranks = new QuizRoundRank[players.Count];
            for (int index = 0; index < players.Count; ++index) { ranks[index] = players[index].rank; }

            ChangeState<QuizRoundPrepare>().Initialize(this);
        }
        public void NewKnowledge()
        {
            var word = m_knowledge[progress.current];
            english.Active(word.english, word.chinese);
        }
        public void RankSort()
        {
            FEMath.SortFixUnityBugAndNotStable(ranks, (x, y) =>
            {
                if (x == null || y == null) { return 0; }
                if (x.GetCurTime() <= 0) { return 1; }
                if (y.GetCurTime() <= 0) { return -1; }
                return x.GetCurTime().CompareTo(y.GetCurTime());
            });
            var rank = 1;
            for (int index = 0; index < ranks.Count; ++index)
            {
                var it = ranks[index];
                if (it == null) { continue; }

                var roundTime = it.GetCurTime();
                if (roundTime < 0)
                {
                    it.SetCurRank(QuizRoundRank.UndefineValue);
                }
                else if (roundTime == 0)
                {
                    it.SetCurRank(QuizRoundRank.FailedValue);
                }
                else
                {
                    if (index > 0 && roundTime > ranks[index - 1].GetCurTime()) { ++rank; }
                    it.SetCurRank(rank);
                }
            }
        }
        public void Swap(int pazzleA, int pazzleB)
        {
            if (state != QuizState.RoundRunning || m_me.IsReplied())
                return;

            english.Swap(pazzleA, pazzleB);
        }
        public void ConfirmReply()
        {
            if (!english.Valid) { return; }

            if (english.isRightAnswer())
            {
                TcpNetwork.Inst().Send(new pps.GMapQuizReplyReport() { round = progress.current, answer = english.original });
            }
            else
            {
                english.Restore();
            }
        }
        public void WillStopFight()
        {
            for (int index = 0; index < players.Count; ++index) { players[index].WillStop(); }
        }
        public void Run(int deltaMs)
        {
            if (!m_stopNextFrame)
            {
                //++frameCounter;
                m_timeUsedMs += deltaMs;
                for (int index = 0; index < players.Count; ++index)
                {
                    players[index].OnLockStepUpdate(GlobalParam.game.lockStepInterval);
                }
                Update(deltaMs);
            }
            else
            {
                Update(0);
            }
        }
        public void NotifyFightResult()
        {
            var resultNotify = new pps.GMapQuizEndReport();
            for (int index = 0; index < ranks.Count; ++index)
            {
                var rank = ranks[index];
                resultNotify.statistic.Add(new pps.GMapQuizEndReport.Statistic() { playerId = rank.player.sessionId, totalScore = rank.GetTotalScore(), });
            }
            TcpNetwork.Inst().Send(resultNotify);
            onResult.Fire();
            m_stopNextFrame = true;
        }
    }
}
