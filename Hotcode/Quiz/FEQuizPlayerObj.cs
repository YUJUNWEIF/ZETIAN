using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public class QuizPlayerObj
    {
        public QuizModule room;
        public pps.ProtoDetailPlayer pdp;
        public int sessionId { get { return pdp.sessionId; } }
        public QuizRoundRank rank;
        internal IQuizAI ai { get; private set; }
        internal QuizPlayerObj(QuizModule room, pps.ProtoDetailPlayer pdp, ILockStepUpdate ai)
        {
            this.room = room;
            this.pdp = pdp;
            this.ai = ai as IQuizAI;
        }
        internal void Initialize(int round)
        {
            this.rank = new QuizRoundRank(this, round);
            room.onChange.Add(OnRoundStateChange);
            if (ai != null) { ai.Bind(this); }
        }
        public void UnInitialize()
        {
            room.onChange.Rmv(OnRoundStateChange);
            if (ai != null) { ai.UnBind(); }
        }
        internal void Reply(int round, string answer, int replyTime)
        {
            if (room.progress.current == round && room.english.original == answer)
            {
                rank.Reply(replyTime);
            }
        }
        public bool IsReplied()
        {
            return rank.IsReplied();
        }
        public void WillStop() { }
        public void OnLockStepUpdate(int deltaMs)
        {
            if (ai != null) { ai.OnLockStepUpdate(deltaMs); }
        }
        void OnRoundStateChange()
        {
            switch (room.state)
            {
                case QuizState.RoundPrepare:
                    rank.OnRoundNew();
                    if (ai != null) { ai.OnRoundNew(); }
                    break;
                case QuizState.RoundOver:
                    rank.OnRoundOver();
                    if (ai != null) { ai.OnRoundOver(); }
                    break;
            }
        }
    }
}