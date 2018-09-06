using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizFrame : ILSharpScript
    {
//generate code begin
        public Text BG_QuizShow_prepare;
        public Text BG_QuizShow_tickDown;
        public Text BG_QuizShow_translation;
        public LSharpListContainer BG_QuizShow_question;
        public Transform BG_QuizShow_question_arrow;
        public LSharpListContainer BG_quizRoundRank;
        public LSharpListContainer quizPlayer;
        public Button quizPlayer_confirm;
        public LSharpListContainer quizDrag;
        void __LoadComponet(Transform transform)
        {
            BG_QuizShow_prepare = transform.Find("BG/QuizShow/@prepare").GetComponent<Text>();
            BG_QuizShow_tickDown = transform.Find("BG/QuizShow/@tickDown").GetComponent<Text>();
            BG_QuizShow_translation = transform.Find("BG/QuizShow/@translation").GetComponent<Text>();
            BG_QuizShow_question = transform.Find("BG/QuizShow/@question").GetComponent<LSharpListContainer>();
            BG_QuizShow_question_arrow = transform.Find("BG/QuizShow/@question/@arrow").GetComponent<Transform>();
            BG_quizRoundRank = transform.Find("BG/@quizRoundRank").GetComponent<LSharpListContainer>();
            quizPlayer = transform.Find("@quizPlayer").GetComponent<LSharpListContainer>();
            quizPlayer_confirm = transform.Find("@quizPlayer/@confirm").GetComponent<Button>();
            quizDrag = transform.Find("@quizDrag").GetComponent<LSharpListContainer>();
        }
        void __DoInit()
        {
            BG_QuizShow_question.OnInitialize();
            BG_quizRoundRank.OnInitialize();
            quizPlayer.OnInitialize();
            quizDrag.OnInitialize();
        }
        void __DoUninit()
        {
            BG_QuizShow_question.OnUnInitialize();
            BG_quizRoundRank.OnUnInitialize();
            quizPlayer.OnUnInitialize();
            quizDrag.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_QuizShow_question.OnShow();
            BG_quizRoundRank.OnShow();
            quizPlayer.OnShow();
            quizDrag.OnShow();
        }
        void __DoHide()
        {
            BG_QuizShow_question.OnHide();
            BG_quizRoundRank.OnHide();
            quizPlayer.OnHide();
            quizDrag.OnHide();
        }
//generate code end
        //public HTMLEngine.UGUI.UGUIDemo htmlText;
        QuizPlayerObj[] qplayers = new QuizPlayerObj[7];

        public static QuizFrame Instance { get; private set; }
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            Instance = this;
            quizPlayer_confirm.onClick.AddListener(() => QuizSceneManager.mod.ConfirmReply());
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
            MusicManager.Instance.CrossFadeTo(GamePath.music.musicCombat);
            QuizSceneManager.mod.onChange.Add(OnStateChange);
            QuizSceneManager.mod.english.onSync.Add(OnEnglishSync);
            QuizSceneManager.mod.onEnglishReplied.Add(OnEnglishReplied);
            QuizSceneManager.mod.onResult.Add(OnResult);

            Util.TimerManager.Inst().Add(OnTimer, 1000);

            quizPlayer.SetValues(T.L(QuizSceneManager.mod.players));
            OnStateChange();
        }
        public override void OnHide()
        {
            QuizSceneManager.mod.onChange.Rmv(OnStateChange);
            QuizSceneManager.mod.onEnglishReplied.Rmv(OnEnglishReplied);
            QuizSceneManager.mod.onResult.Rmv(OnResult);
            QuizSceneManager.mod.english.onSync.Rmv(OnEnglishSync);
            Util.TimerManager.Inst().Remove(OnTimer);
            __DoHide();
            base.OnHide();
        }
        void OnStateChange()
        {
            var state = QuizSceneManager.mod.state;
            if (state != QuizState.RoundRunning) { ForceStopDrag(); }
            if (state == QuizState.RoundOver)
            {
                var ranks = QuizSceneManager.mod.ranks;
                for (int index = 0; index < quizPlayer.count; ++index)
                {
                    var it = quizPlayer.GetItem(index);
                    if (!it) { continue; }

                    for (int j = 0; j < ranks.Count; ++j)
                    {
                        var script = T.As<QuizPlayerItemPanel>(it);                        
                        if (ranks[j].player.sessionId == script.GetValue().sessionId)
                        {
                            script.Display(ranks[j].GetCurRank());
                            break;
                        }
                    }
                }
            }
            OnEnglishSync();
            BG_quizRoundRank.SetValues(T.L(QuizSceneManager.mod.ranks));
        }
        void OnEnglishReplied(QuizPlayerObj player)
        {
            if (player.sessionId == QuizModule.MySId())
            {
                OnEnglishSync();
                BG_quizRoundRank.SetValues(T.L(QuizSceneManager.mod.ranks));
            }
        }
        void OnEnglishSync()
        {
            var state = QuizSceneManager.mod.state;
            var english = QuizSceneManager.mod.english;
            if (state == QuizState.Unknown || !english.Valid) { return; }

            var q = DisplayQuestion(english, state);
            var preparing = (state == QuizState.RoundPrepare);
            BG_QuizShow_prepare.enabled = preparing;
            BG_QuizShow_translation.text = !preparing ? english.translation : string.Empty;
            BG_QuizShow_question.SetValues(q);
            Display(q);

            quizDrag.SetValues(DragQuestion(english, state));
        }
        IList<object> DisplayQuestion(EnglishSwap english, QuizState state)
        {
            var q = new List<object>(english.original.Length);
            switch (state)
            {
                case QuizState.RoundPrepare:
                    for (int index = 0; index < english.tempAnswer.Length; ++index)
                    {
                        q.Add(new AlphabetDisplay() { ch = default(char), pazzled = false, focus = false, });
                    }
                    break;
                case QuizState.RoundRunning:
                case QuizState.RoundOver:
                case QuizState.GameOver:
                    var replied = QuizModule.Me().IsReplied();
                    for (int index = 0; index < english.tempAnswer.Length; ++index)
                    {
                        var exist = english.pazzles.FindIndex(it => it.Key == index);
                        var pazzled = (exist >= 0);
                        var ch = (pazzled && state != QuizState.RoundOver) ? english.tempAnswer[index] : english.original[index];
                        var color = default(Color);
                        if (pazzled) { color = (replied ? Color.green : ((state == QuizState.RoundRunning) ? Color.grey : Color.red)); }
                        q.Add(new AlphabetDisplay()
                        {
                            ch = ch,
                            pazzled = pazzled,
                            focus = false,
                            color = color,
                        });
                    }
                    break;
            }
            return q;
        }
        void Display(IList<object> q)
        {
            var targetAt = -1;
            for (int index = 0; index < q.Count; ++index)
            {
                var ad = (AlphabetDisplay)q[index];
                if (ad.focus)
                {
                    targetAt = index;
                    break;
                }
            }
            if (targetAt >= 0)
            {
                Util.UnityHelper.Show(BG_QuizShow_question_arrow, BG_QuizShow_question.GetItem(targetAt));
            }
            else
            {
                Util.UnityHelper.Hide(BG_QuizShow_question_arrow);
            }
        }
        IList<object> DragQuestion(EnglishSwap english, QuizState state)
        {
            var inputs = new object[EnglishSwap.LENGTH];
            var startAt = inputs.Length - english.pazzles.Count;
            for (int index = 0; index < inputs.Length; ++index)
            {
                var invalid = (index < startAt || state == QuizState.RoundPrepare);
                inputs[index] = invalid ? new QuizPazzle(-1, (char)0xFF) : english.pazzles[index - startAt];
            }
            return inputs;
        }
        void OnResult()
        {
            api.coroutineHelper.StartCoroutineImmediate(CleanBattle());
        }
        IEnumerator CleanBattle()
        {
            GuiManager.Inst().ShowFrame(typeof(FightCleanFrame).Name);
            yield return WaitSeconds.Delay(3f);
            GuiManager.Inst().HideFrame(typeof(FightCleanFrame).Name);
            NetworkWatcher.Inst().Stop();
            yield return new WaitMainSubStateSwitchComplete();

            GuiManager.Inst().HideFrame(api.name);
            GuiManager.Inst().ShowFrame(typeof(QuizResultFrame).Name);
        }
        void OnTimer()
        {
            if (QuizSceneManager.mod.state == QuizState.RoundPrepare ||
                QuizSceneManager.mod.state == QuizState.RoundRunning)
            {
                var bstate = QuizSceneManager.mod.bstate;                
                var leftTimeMs = bstate.param;
                if (leftTimeMs < 0) { leftTimeMs = 0; }
                BG_QuizShow_tickDown.text = (leftTimeMs / 1000).ToString();
            }
            else
            {
                BG_QuizShow_tickDown.text = string.Empty;
            }
        }
        public void Press(QuizDragItemPanel dragItem)
        {
            Util.UnityHelper.Show(BG_QuizShow_question_arrow);
            for (int index = 0; index < quizDrag.count; ++index)
            {
                var drag = T.As<QuizDragItemPanel>(quizDrag.GetItem(index));
                if (drag == dragItem)
                {
                    break;
                }
            }
            Swap(null, dragItem);
        }
        public void Swap(QuizDragItemPanel from, QuizDragItemPanel dst)
        {
            if (dst != null)
            {
                var item = BG_QuizShow_question.GetItem(dst.GetValue().Key);
                if (item)
                {
                    var pos = BG_QuizShow_question_arrow.position;
                    pos.x = item.transform.position.x;
                    BG_QuizShow_question_arrow.position = pos;
                }
            }
        }
        public void Release()
        {
            Util.UnityHelper.Hide(BG_QuizShow_question_arrow);
        }
        public void ForceStopDrag()
        {
            Desktop.evSys.SetSelectedGameObject(null);
            for (int index = 0; index < quizDrag.count; ++index)
            {
                var drag = T.As<QuizDragItemPanel>(quizDrag.GetItem(index));
                drag.ForceStopDrag();
            }
        }
    }
}
