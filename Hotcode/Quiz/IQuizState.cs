using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace geniusbaby
{
    public enum QuizState
    {
        Unknown = 0,
        RoundPrepare = 1,
        RoundRunning = 2,
        RoundOver = 3,
        GameOver = 4,
    } 

    public class QuizRoundPrepare : IFEBattleState<QuizModule, QuizState>
    {
        int m_timeLeftMs;
        public override int param { get { return m_timeLeftMs; } }
        public void Initialize(QuizModule battle)
        {
            Attach(battle, QuizState.RoundPrepare);
            m_timeLeftMs = room.ruleCfg.roundPrepareTime;
        }
        public override void Enter()
        {
            room.NewKnowledge();
        }
        public override void Leave() { }
        public override void Update(int deltaMs)
        {
            m_timeLeftMs -= room.deltaMs;
            if (m_timeLeftMs <= 0)
            {
                room.ChangeState<QuizRoundRunning>().Initialize(room);
            }
        }
    }
    public class QuizRoundRunning : IFEBattleState<QuizModule, QuizState>
    {
        int m_timeLeftMs;
        public override int param { get { return m_timeLeftMs; } }
        public void Initialize(QuizModule battle)
        {
            Attach(battle, QuizState.RoundRunning);
            m_timeLeftMs = room.ruleCfg.roundTime;
        }
        public override void Enter()
        {
        }
        public override void Leave() { }
        public override void Update(int deltaMs)
        {
            m_timeLeftMs -= room.deltaMs;
            if (m_timeLeftMs <= 0)
            {
                room.ChangeState<QuizRoundOver>().Initialize(room);
            }
            else
            {
                for (int index = 0; index < room.players.Count; ++index)
                {
                    if (!room.players[index].IsReplied()) { return; }
                }
                room.ChangeState<QuizRoundOver>().Initialize(room);
            }
        }
        public void Answer(string playerId, int round, string answer)
        {
            room.FindPlayer(playerId).Reply(round, answer, m_timeLeftMs);
        }
    }
    public class QuizRoundOver : IFEBattleState<QuizModule, QuizState>
    {
        int m_timeLeftMs;
        public override int param { get { return m_timeLeftMs; } }
        public void Initialize(QuizModule battle)
        {
            Attach(battle, QuizState.RoundOver);
            m_timeLeftMs = room.ruleCfg.roundOverTime;
        }
        public override void Enter()
        {
            room.RankSort();
        }
        public override void Leave() { }
        public override void Update(int deltaMs)
        {
            m_timeLeftMs -= room.deltaMs;
            if (m_timeLeftMs <= 0)
            {
                if (room.progress.current + 1 < room.progress.max)
                {
                    ++room.progress.current;
                    room.ChangeState<QuizRoundPrepare>().Initialize(room);
                }
                else
                {
                    room.ChangeState<QuizGameOver>().Initialize(room);
                }
            }
        }
    }
    public class QuizGameOver : IFEBattleState<QuizModule, QuizState>
    {
        public override int param { get; }
        public void Initialize(QuizModule battle)
        {
            Attach(battle, QuizState.RoundOver);
        }
        public override void Enter()
        {
            room.NotifyFightResult();
        }
        public override void Leave() { }
        public override void Update(int deltaMs)
        {
        }
    }
}