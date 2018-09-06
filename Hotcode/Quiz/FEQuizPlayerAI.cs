using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public class FEQuizPlayerAI : IQuizAI//, IQuizEvent
    {
        QuizPlayerObj m_playerObj;
        cfg.quizAI m_aiCfg;
        int replyTimeMs;
        public FEQuizPlayerAI(int ai)
        {
            m_aiCfg = tab.quizAI.Inst().Find(ai);
        }
        public void Bind(QuizPlayerObj playerObj)
        {
            m_playerObj = playerObj;
        }
        public void UnBind()
        {
            m_playerObj = null;
        }
        public void OnRoundNew()
        {
            var english = QuizSceneManager.mod.english;
            var time = QuizSceneManager.mod.rand.Range(m_aiCfg.speedPerChar.min, m_aiCfg.speedPerChar.max) * english.pazzles.Count;
            var replyTimeSec = FEMath.Clamp(time, m_aiCfg.limitTime.min, m_aiCfg.limitTime.max);
            replyTimeMs = (int)(replyTimeSec * 1000);
        }
        public void OnRoundOver() { }
        public void OnLockStepUpdate(int deltaMs)
        {
            var qs = QuizSceneManager.mod.GetState() as QuizRoundRunning;
            if (qs != null)
            {
                replyTimeMs -= deltaMs;
                if (replyTimeMs <= 0)
                {
                    qs.Answer(m_playerObj.pdp.uniqueId, QuizSceneManager.mod.progress.current, QuizSceneManager.mod.english.original);
                }
            }
        }
    }
}
