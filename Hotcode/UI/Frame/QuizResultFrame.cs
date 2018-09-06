using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizResultFrame : ILSharpScript
    {
//generate code begin
        public Text Me_Rank_value;
        public Text Me_Score_value;
        public LSharpListSuper Down_Clip_list;
        public Button Ops_onceAgain;
        public Button Ops_leave;
        void __LoadComponet(Transform transform)
        {
            Me_Rank_value = transform.Find("Me/Rank/@value").GetComponent<Text>();
            Me_Score_value = transform.Find("Me/Score/@value").GetComponent<Text>();
            Down_Clip_list = transform.Find("Down/Clip/@list").GetComponent<LSharpListSuper>();
            Ops_onceAgain = transform.Find("Ops/@onceAgain").GetComponent<Button>();
            Ops_leave = transform.Find("Ops/@leave").GetComponent<Button>();
        }
        void __DoInit()
        {
            Down_Clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            Down_Clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            Down_Clip_list.OnShow();
        }
        void __DoHide()
        {
            Down_Clip_list.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            Down_Clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(QuizRankItemPanel).Name);
            __DoInit();
            Ops_onceAgain.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                HttpNetwork.Inst().Communicate(new pps.GSrvPvpRequest() { combatType = GlobalDefine.GTypeQuiz });
            });
            Ops_leave.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
            });
        }
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            var ranks = QuizSceneManager.mod.ranks;
            FEMath.SortFixUnityBugAndNotStable(ranks, (x, y) => -x.GetTotalScore().CompareTo(y.GetTotalScore()));
            Down_Clip_list.SetValues(T.L(ranks));
            for (int index = 0; index < ranks.Count; ++index)
            {
                if (ranks[index].player.sessionId == QuizModule.MySId())
                {
                    Me_Rank_value.text = (index + 1).ToString();
                    Me_Score_value.text = ranks[index].GetTotalScore().ToString();
                    break;
                }
            }
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
    }
}
