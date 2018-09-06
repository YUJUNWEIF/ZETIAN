using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ResultPvpFrame : ILSharpScript
    {
//generate code begin
        public Text A_name;
        public Text A_title;
        public Text A_fight_score;
        public Text A_leftTime_score;
        public Text A_wrong_score;
        public Text A_all_score;
        public Text A_all_eval;
        public Text B_name;
        public Text B_title;
        public Text B_fight_score;
        public Text B_leftTime_score;
        public Text B_wrong_score;
        public Text B_all_score;
        public Text B_all_eval;
        public LSharpListSuper clip_list;
        public Button Ops_detail;
        public Button Ops_onceAgain;
        public Button Ops_leave;
        void __LoadComponet(Transform transform)
        {
            A_name = transform.Find("A/@name").GetComponent<Text>();
            A_title = transform.Find("A/@title").GetComponent<Text>();
            A_fight_score = transform.Find("A/fight/@score").GetComponent<Text>();
            A_leftTime_score = transform.Find("A/leftTime/@score").GetComponent<Text>();
            A_wrong_score = transform.Find("A/wrong/@score").GetComponent<Text>();
            A_all_score = transform.Find("A/all/@score").GetComponent<Text>();
            A_all_eval = transform.Find("A/all/@eval").GetComponent<Text>();
            B_name = transform.Find("B/@name").GetComponent<Text>();
            B_title = transform.Find("B/@title").GetComponent<Text>();
            B_fight_score = transform.Find("B/fight/@score").GetComponent<Text>();
            B_leftTime_score = transform.Find("B/leftTime/@score").GetComponent<Text>();
            B_wrong_score = transform.Find("B/wrong/@score").GetComponent<Text>();
            B_all_score = transform.Find("B/all/@score").GetComponent<Text>();
            B_all_eval = transform.Find("B/all/@eval").GetComponent<Text>();
            clip_list = transform.Find("clip/@list").GetComponent<LSharpListSuper>();
            Ops_detail = transform.Find("Ops/@detail").GetComponent<Button>();
            Ops_onceAgain = transform.Find("Ops/@onceAgain").GetComponent<Button>();
            Ops_leave = transform.Find("Ops/@leave").GetComponent<Button>();
        }
        void __DoInit()
        {
            clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            clip_list.OnShow();
        }
        void __DoHide()
        {
            clip_list.OnHide();
        }
//generate code end
        int combatType;
        int classId;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordWrongItemPanel).Name);
            __DoInit();
            Ops_onceAgain.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                HttpNetwork.Inst().Communicate(
                    new pps.GSrvPvpRequest()
                    {
                        combatType = combatType,
                        mapId = FightSceneManager.Inst().mapId,
                    });
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

            //var classId = FightSceneManager.Inst().classId;
            //var wordClass = WordPackageManager.Inst().FindClass(PlayerModule.Inst().packageId, classId + 1);
            //Util.UnityHelper.ShowHide(shareButton, wordClass != null);
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void Display(int combatType, int classId, List<StatisticData> statistic)
        {
            this.combatType = combatType;
            this.classId = classId;
            var me = statistic.FindIndex(it => it.playerId == PlayerModule.MyId());
            for (int index = 0; index < statistic.Count; ++index)
            {
                var data = statistic[index];
                //if (me == index)
                //{
                //    statisticAPanel.Display(data);
                //    winAImage.enabled = data.win;
                //}
                //else
                //{
                //    statisticBPanel.Display(data);
                //    winBImage.enabled = data.win;
                //}

                //if (fishScoreText) fishScoreText.text = statistic.fish.total.ToString();
                //if (bossScoreText) bossScoreText.text = statistic.boss.total.ToString();
                //if (taskScoreText) taskScoreText.text = statistic.mission.total.ToString();
                //if (rightWordScoreText) rightWordScoreText.text = statistic.rightAward.total.ToString();
                //if (wrongAlphabetText) wrongAlphabetText.text = statistic.wrongPunish.total.ToString();
                //if (lifeTimeScoreText) lifeTimeScoreText.text = statistic.timeAward.total.ToString();
                //if (totalScoreText) { totalScoreText.text = statistic.total.ToString(); }

            }
        }
    }
}
