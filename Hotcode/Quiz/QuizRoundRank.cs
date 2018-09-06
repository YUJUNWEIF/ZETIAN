using System;
using System.Collections.Generic;
using System.Net;
using Util;

namespace geniusbaby
{
    public class QuizRoundRank
    {
        public const int UndefineValue = -1;
        public const int FailedValue = 0;

        QuizPlayerObj m_player;
        int[] m_ranks;
        int[] m_replyTimes;
        public QuizRoundRank(QuizPlayerObj player, int round)
        {
            m_player = player;
            m_ranks = new int[round];
            m_replyTimes = new int[round];
            for (int index = 0; index < round; ++index)
            {
                m_ranks[index] = UndefineValue;
                m_replyTimes[index] = UndefineValue;
            }
        }
        public QuizPlayerObj player
        {
            get { return m_player; }
        }
        public void OnRoundNew() { }
        public void OnRoundOver()
        {
            var progress = m_player.room.progress;
            if (m_ranks[progress.current] == UndefineValue) { m_ranks[progress.current] = FailedValue; }
            if (m_replyTimes[progress.current] == UndefineValue) { m_replyTimes[progress.current] = FailedValue; }
        }
        internal void Reply(int replyTime)
        {
            var progress = m_player.room.progress;
            if (m_replyTimes[progress.current] <= 0)
            {
                m_replyTimes[progress.current] = replyTime;
                player.room.RankSort();
                player.room.onEnglishReplied.Fire(player);
            }
        }
        public int GetCurTime() { return m_replyTimes[player.room.progress.current]; }

        public void SetCurRank(int rank)
        {
            m_ranks[player.room.progress.current] = rank;
        }
        public int GetCurRank()
        {
            return m_ranks[player.room.progress.current];
        }
        public int GetRankCount(int rank)
        {
            int count = 0;
            for (int index = 0; index < m_ranks.Length; ++index)
            {
                if (rank == m_ranks[index]) { ++count; }
            }
            return count;
        }
        public int GetTotalScore()
        {
            var totalScore = 0;
            for (int index = 0; index < player.room.ruleCfg.rankScores.Length; ++index)
            {
                var rs = player.room.ruleCfg.rankScores[index];
                totalScore += GetRankCount(rs.rank) * rs.score;
            }
            return totalScore;
        }
        internal bool IsReplied()
        {
            return m_replyTimes[player.room.progress.current] > 0;
        }
    }
}
