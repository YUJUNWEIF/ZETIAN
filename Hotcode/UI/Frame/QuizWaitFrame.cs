using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class QuizWaitFrame : ILSharpScript
    {
//generate code begin
        public LSharpListContainer quizWait;
        public Text c_tickDown;
        public Button c_quit;
        public Text c_room_id;
        public Text c_c_waiting;
        public Text c_c_waiting_playNumber;
        public LSharpListSuper c_c_clip_list;
        void __LoadComponet(Transform transform)
        {
            quizWait = transform.Find("@quizWait").GetComponent<LSharpListContainer>();
            c_tickDown = transform.Find("c/@tickDown").GetComponent<Text>();
            c_quit = transform.Find("c/@quit").GetComponent<Button>();
            c_room_id = transform.Find("c/room/@id").GetComponent<Text>();
            c_c_waiting = transform.Find("c/c/@waiting").GetComponent<Text>();
            c_c_waiting_playNumber = transform.Find("c/c/@waiting/@playNumber").GetComponent<Text>();
            c_c_clip_list = transform.Find("c/c/clip/@list").GetComponent<LSharpListSuper>();
        }
        void __DoInit()
        {
            quizWait.OnInitialize();
            c_c_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            quizWait.OnUnInitialize();
            c_c_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            quizWait.OnShow();
            c_c_clip_list.OnShow();
        }
        void __DoHide()
        {
            quizWait.OnHide();
            c_c_clip_list.OnHide();
        }
//generate code end
        int m_tickDownMs;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            c_c_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordPreviewItemPanel).Name);
            __DoInit();
            c_quit.onClick.AddListener(() =>
            {
                if (PvpRoomModule.Inst().players.Count < PvpRoomModule.Inst().needPlayers)
                {
                    GuiManager.Inst().HideFrame(api.name);
                    GuiManager.Inst().HideFrame(typeof(QuizFrame).Name);
                    TcpNetwork.Inst().Send(new pps.GMapPvpQuitRequest());
                    NetworkWatcher.Inst().DelayStop(GamePath.net.keepAlive / 2);
                }
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
            PvpRoomModule.Inst().onSync.Add(OnSync);
            PvpRoomModule.Inst().onTickDownSync.Add(OnTickDownSync);
            Util.TimerManager.Inst().Add(OnTimer, 1000);

            OnSync();
            OnTickDownSync();
            OnTimer();
        }
        public override void OnHide()
        {
            PvpRoomModule.Inst().onSync.Rmv(OnSync);
            PvpRoomModule.Inst().onTickDownSync.Rmv(OnTickDownSync);
            Util.TimerManager.Inst().Remove(OnTimer);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            var players = PvpRoomModule.Inst().players;
            var cap = new RangeValue(players.Count, PvpRoomModule.Inst().needPlayers);
            var sort = new object[cap.max];
            var exist = players.FindIndex(it => it.uniqueId == PlayerModule.MyId());
            var me = players[exist];
            players[exist] = players[0];
            players[0] = me;
            quizWait.SetValues(sort);
            c_c_waiting_playNumber.text = cap.ToStyle1();
        }
        void OnTickDownSync()
        {
            m_tickDownMs = PvpRoomModule.Inst().tickDown;
            if (m_tickDownMs > 0)
            {
                GuiManager.Inst().ShowFrame(api.name);

                var rand = new Util.FastRandom((uint)PvpRoomModule.Inst().randSeed);

                var classCfgs = KnowledgeModule.Inst().FindLGAP();
                int tryCount = 0;
                var wordCfgs = new List<object>();
                int count = 10;
                while (wordCfgs.Count < count)
                {
                    int classIndex = rand.Next(classCfgs.Count);
                    var classCfg = classCfgs[classIndex];
                    var wordIndex = rand.Next(classCfg.words.Count);
                    var wordCfg = classCfg.words[wordIndex];
                    if (cfg.word.Valid(wordCfg.english) && !wordCfgs.Contains(wordCfg))
                    {
                        wordCfgs.Add(wordCfg);
                    }
                    ++tryCount;
                    if (tryCount >= count * 3) { break; }
                }
                c_c_clip_list.SetValues(wordCfgs);
            }
            else
            {
                c_c_clip_list.SetValues(new List<object>());
            }
            c_c_waiting.gameObject.SetActive(m_tickDownMs <= 0);
        }
        void OnTimer()
        {
            if (m_tickDownMs > 0)
            {
                m_tickDownMs -= 1000;
                if (m_tickDownMs < 0) { m_tickDownMs = 0; }
                c_tickDown.text = (m_tickDownMs / 1000).ToString();
                c_c_waiting.gameObject.SetActive(false);
            }
            else
            {
                c_tickDown.text = string.Empty;
                c_c_waiting.gameObject.SetActive(true);
            }
        }
    }
}
