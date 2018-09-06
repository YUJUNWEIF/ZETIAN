using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizJoinPanel : ILSharpScript
    {
//generate code begin
        public Text Clip_des;
        public Button ticketJoin;
        public Image ticketJoin_costType;
        public Text ticketJoin_costValue;
        void __LoadComponet(Transform transform)
        {
            Clip_des = transform.Find("Clip/@des").GetComponent<Text>();
            ticketJoin = transform.Find("@ticketJoin").GetComponent<Button>();
            ticketJoin_costType = transform.Find("@ticketJoin/@costType").GetComponent<Image>();
            ticketJoin_costValue = transform.Find("@ticketJoin/@costValue").GetComponent<Text>();
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
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            ticketJoin.onClick.AddListener(() =>
            {
                HttpNetwork.Inst().Communicate(new pps.GSrvPvpRequest() { combatType = GlobalDefine.GTypeQuiz });
            });
        }
    }
}
