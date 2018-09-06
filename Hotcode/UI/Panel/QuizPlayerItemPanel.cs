using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizPlayerItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        public Text score;
        public Text score_add;
        public Text champion_value;
        public Text die_value;
        public RectTransform doing;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            score = transform.Find("@score").GetComponent<Text>();
            score_add = transform.Find("@score/@add").GetComponent<Text>();
            champion_value = transform.Find("champion/@value").GetComponent<Text>();
            die_value = transform.Find("die/@value").GetComponent<Text>();
            doing = transform.Find("@doing").GetComponent<RectTransform>();
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
        QuizPlayerObj m_hero;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }         
        public QuizPlayerObj GetValue() { return m_hero; }
        public void SetValue(QuizPlayerObj value)
        {
            OnEnglishReplied(m_hero = value);
            bool invalid = (m_hero == null);
            if (!invalid)
            {
                name.text = m_hero.pdp.name;
            }
        }
        public override void OnShow()
        {
            base.OnShow();
            QuizSceneManager.mod.onEnglishReplied.Add(OnEnglishReplied);
        }
        public override void OnHide()
        {
            QuizSceneManager.mod.onEnglishReplied.Rmv(OnEnglishReplied);
            base.OnHide();
        }
        public void Display(int rank)
        {
            if (rank < 0) { rank = 0; }
            var ruleCfg = tab.quizRule.Inst().RecordArray[0];
            var scoreCfg = Array.Find(ruleCfg.rankScores, it => it.rank == rank);
            score_add.text = "+" + scoreCfg.score.ToString();
            //if (scoreAddAnim) scoreAddAnim.Play();
        }
        void OnEnglishReplied(QuizPlayerObj now)
        {
            if (m_hero == null) { return; }

            if (m_hero.sessionId == now.sessionId)
            {
                m_hero = now;
                OnRoundUpdate();
            }
        }
        void OnRoundUpdate()
        {
            if (m_hero == null) { return; }
            score.text = m_hero.rank.GetTotalScore().ToString();
            champion_value.text = m_hero.rank.GetRankCount(1).ToString();
            die_value.text = m_hero.rank.GetRankCount(0).ToString();
            if (doing)
            {
                var round = QuizSceneManager.mod.progress;
                //if (QuizSceneManager.m_module.state == QuizState.RoundRunning && round >= 0)
                //{
                //    //statusImage.gameObject.SetActive(m_hero.replyTimes[round] <= 0);
                //}
                //else
                //{
                //    statusImage.gameObject.SetActive(false);
                //}
            }
            //coroutineHelper.StartCoroutineImmediate(OverDisplay());
        }
        //IEnumerator OverDisplay()
        //{
        //    var displayResult = CaculateResult();
        //    switch (displayResult)
        //    {
        //        case DisplayResult.Normal: break;
        //        case DisplayResult.Die:
        //            yield return FlyFx("Die", null, dieText.transform);
        //            break;
        //        case DisplayResult.Champion:
        //            yield return FlyFx("Champion", null, championText.transform);
        //            break;
        //    }
        //    yield return null;
        //    switch (displayResult)
        //    {
        //        case DisplayResult.Normal:
        //            scoreText.text = "100";
        //            //display score
        //            break;
        //        case DisplayResult.Die:
        //            scoreText.text = "0";
        //            //display die
        //            break;
        //        case DisplayResult.Champion:
        //            scoreText.text = "150";
        //            //display champion
        //            //display score
        //            break;
        //    }
        //}

        IEnumerator FlyFx(string fx, Transform from, Transform to)
        {
            Vector2 fromPoint = Vector2.zero;
            GameObject go = null;

            go = GameObjPool.Inst().CreateObjSync(GamePath.asset.terrain, fx);
            var anim = go.GetComponent<Animation>();
            if (anim) { anim.Play(); }
            go.transform.parent = api.transform.parent;
            Util.UnityHelper.SetLayerRecursively(go.transform, Util.TagLayers.UI);

            float startAt = Time.time;
            float flyTime = 1f;
            while (Time.time < startAt + flyTime)
            {
                var key = (Time.time - startAt) / flyTime;
                if (go)
                {
                    go.transform.position = Vector3.Lerp(fromPoint, to.position, key);
                }
                yield return null;
            }
            GameObjPool.Inst().DestroyObj(go);
        }
    }
}
