using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ResultClassWinFrame : ILSharpScript
    {
//generate code begin
        public GuiBar expBar;
        public Text expBar_value;
        public Text statistic_fight_score;
        public Text statistic_lifeTime_score;
        public Text statistic_wrong_score;
        public Text statistic_all_score;
        public Text statistic_eval;
        public Button statistic_detail;
        public LSharpListSuper statistic_clip_list;
        public LSharpListSuper ld_clip_drop;
        public Button close;
        void __LoadComponet(Transform transform)
        {
            expBar = transform.Find("@expBar").GetComponent<GuiBar>();
            expBar_value = transform.Find("@expBar/@value").GetComponent<Text>();
            statistic_fight_score = transform.Find("statistic/fight/@score").GetComponent<Text>();
            statistic_lifeTime_score = transform.Find("statistic/lifeTime/@score").GetComponent<Text>();
            statistic_wrong_score = transform.Find("statistic/wrong/@score").GetComponent<Text>();
            statistic_all_score = transform.Find("statistic/all/@score").GetComponent<Text>();
            statistic_eval = transform.Find("statistic/@eval").GetComponent<Text>();
            statistic_detail = transform.Find("statistic/@detail").GetComponent<Button>();
            statistic_clip_list = transform.Find("statistic/clip/@list").GetComponent<LSharpListSuper>();
            ld_clip_drop = transform.Find("ld/clip/@drop").GetComponent<LSharpListSuper>();
            close = transform.Find("@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            statistic_clip_list.OnInitialize();
            ld_clip_drop.OnInitialize();
        }
        void __DoUninit()
        {
            statistic_clip_list.OnUnInitialize();
            ld_clip_drop.OnUnInitialize();
        }
        void __DoShow()
        {
            statistic_clip_list.OnShow();
            ld_clip_drop.OnShow();
        }
        void __DoHide()
        {
            statistic_clip_list.OnHide();
            ld_clip_drop.OnHide();
        }
//generate code end
        Action m_action;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            statistic_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordWrongItemPanel).Name);
            ld_clip_drop.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(DropItemPanel).Name);
            __DoInit();
            //replayButton.onClick.AddListener(() =>
            //{
            //    GuiManager.Inst().HideFrame(this);
            //    var cached = FightSceneManager.dataCached;
            //    FightSceneManager.Inst().PVELevelEnter(cached.mapId, cached.classId);
            //});
            close.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                if (m_action != null) m_action();
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
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void Display(StatisticData statistic, Action action)
        {
            m_action = action;
            //statisticPanel.Display(statistic);
            AchieveModule.Inst().StatisticLevelScore(statistic.total);
            //var levelAwardCfg = tab.map.Inst().Find(FightSceneManager.dataCached.mapId);
            //var dps = new List<object>();
            //for (int index = 0; index < levelAwardCfg.drop.Length; ++index)
            //{
            //    var it = levelAwardCfg.drop[index];
            //    dps.Add(new Item(it.id) { count = it.count });
            //}
            //ld_clip_drop.SetValues(dps);
            HttpNetwork.Inst().Communicate(new pps.ClassFinishRequest()
            {
                mapId = FightSceneManager.Inst().mapId,
                classId = FightSceneManager.Inst().classId
            });
        }
        //public void AfterInitialized()
        //{
        //    GameLevel level = GameManager.Instance.CurrentLevel;
        //    int totalExp = level.Score + level.TimeAward + level.Flower * 20 + level.Star * 200;
        //    int totalCoin = level.Coin + level.Diamond * 50;
        //    LvlExp lvlExp = Bank.Instance.GetExp(level.gameType);
        //    LvlExp oldExp = new LvlExp(lvlExp.whole);
        //    lvlExp.AddExp(totalExp);
        //    StartCoroutine(ShowExp(level.gameType, oldExp, lvlExp));

        //    Bank.Instance.GoldCoin += totalCoin;

        //    scoreLabel.text = level.Score.ToString();
        //    useTimeLabel.text = ((int)level.CompleteTime).ToString();
        //    flowerLabel.text = level.Flower.ToString();
        //    starLabel.text = level.Star.ToString();
        //    coinLabel.text = level.Coin.ToString();
        //    diamondLabel.text = level.Diamond.ToString();

        //    flowerLabel.text = totalExp.ToString();
        //    moneyLabel.text = Bank.Instance.GoldCoin.ToString();
        //}
        //IEnumerator ShowExp(LvlExp oldExp, LvlExp curExp)
        //{
        //    while (curExp.lvl > oldExp.lvl || curExp.lvl == oldExp.lvl && curExp.exp.current > oldExp.exp.current)
        //    {
        //        float percent = oldExp.exp.current * 1f / oldExp.exp.max;
        //        expBar.value = percent > 1 ? 1 : percent;
        //        expText.text = oldExp.exp.ToStyle1();
        //        //titleText.text = titleCfg.name;

        //        oldExp.AddExp(1);
        //        yield return null;
        //    }
        //}
    }
}
