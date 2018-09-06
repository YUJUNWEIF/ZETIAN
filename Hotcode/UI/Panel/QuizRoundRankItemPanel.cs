using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizRoundRankItemPanel : ILSharpScript
    {
//generate code begin
        public Text name;
        public Text time;
        public Text score;
        void __LoadComponet(Transform transform)
        {
            name = transform.Find("@name").GetComponent<Text>();
            time = transform.Find("@time").GetComponent<Text>();
            score = transform.Find("@score").GetComponent<Text>();
        }
        void __DoInit()
        {
        }
        void __DoUninit()
        {
        }
        void __DoShow()
        {
        }
        void __DoHide()
        {
        }
//generate code end
        QuizRoundRank m_rank;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public QuizRoundRank GetValue() { return m_rank; }
        public void SetValue(QuizRoundRank value)
        {
            m_rank = value;
            if (m_rank == null || m_rank.GetCurTime() < 0)
            {
                name.text = "--";
                time.text = "--";
                time.color = Color.white;
                score.text = "--";
                score.color = Color.white;
            }
            else if (m_rank.GetCurTime() > 0)
            {
                name.text = m_rank.player.pdp.name;
                time.text = (m_rank.GetCurTime() * 0.001f).ToString("0.0");
                time.color = Color.green;
                var rank = m_rank.GetCurRank();
                if (rank < 0) { rank = 0; }
                var ruleCfg = tab.quizRule.Inst().RecordArray[0];
                var scoreCfg = Array.Find(ruleCfg.rankScores, it => it.rank == rank);
                score.text = scoreCfg.score.ToString();
                score.color = Color.green;
            }
            else if (m_rank.GetCurTime() == 0)
            {
                name.text = m_rank.player.pdp.name;
                time.text = "00";
                time.color = Color.red;
                score.text = "0";
                score.color = Color.red;
            }
        }
    }
}
