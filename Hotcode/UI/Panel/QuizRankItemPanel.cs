using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizRankItemPanel : ILSharpScript
    {
//generate code begin
        public Text rank;
        public Image icon;
        public Text icon_name;
        public Text score;
        public Text award;
        public Text die;
        void __LoadComponet(Transform transform)
        {
            rank = transform.Find("@rank").GetComponent<Text>();
            icon = transform.Find("@icon").GetComponent<Image>();
            icon_name = transform.Find("@icon/@name").GetComponent<Text>();
            score = transform.Find("@score").GetComponent<Text>();
            award = transform.Find("@award").GetComponent<Text>();
            die = transform.Find("@die").GetComponent<Text>();
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
            icon_name.text = m_rank.player.pdp.name;
            score.text = m_rank.GetTotalScore().ToString();
            var sb = new StringBuilder();
            for (int index = 1; index <= 3; ++index)
            {
                if (index > 1)
                {
                    sb.Append('/');
                }
                sb.Append(m_rank.GetRankCount(index));
            }
            award.text = sb.ToString();
            die.text = m_rank.GetRankCount(0).ToString();
        }
    }
}
