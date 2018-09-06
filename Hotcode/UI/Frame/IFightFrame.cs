using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace geniusbaby.LSharpScript
{
    public struct Cursor
    {
        public bool down;
        public Vector2 position;
        public Cursor(bool down, Vector2 position) : this()
        {
            this.down = down;
            this.position = position;
        }
    }
    public abstract class IFightFrame : ILSharpScript, IArea2d, IPhysicAttack //, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        Cursor m_gunStatus;
        public float speed = 80;
        protected RectTransform m_cachedArea;
        const int MaxQueque = 20;
        Queue<string> m_msgs = new Queue<string>();
        string m_displaying;
        RectTransform m_contentRc;
        Cursor m_lastPress;
                
        public static IFightFrame Instance { get; private set; }
        public Cursor lastPress { get { return m_lastPress; } }
        public abstract Image targetIcon { get; }
        public abstract MeshRenderer[] targetHpRoots { get; }
        public abstract Text targetHpValue { get; }

        public abstract Text tipText { get; }

        protected ui.FightFrameParam m_param;

        //public abstract Text diffText { get; }
        //public abstract Text diffUpText { get; }
        
        public RectTransform area { get { return m_cachedArea; } }
        public abstract IPlayer2DObj GetPlayerObj(int sessionId);

        void Awake()
        {
            Instance = this;
        }
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            m_cachedArea = (RectTransform)api.transform;
            m_param = api.GetComponent<ui.FightFrameParam>();
            //Util.UnityHelper.Register(m_cachedArea, EventTriggerType.PointerDown, ev =>
            //{
            //    m_prepareFire = true;
            //    var data = (PointerEventData)ev;
            //    FireAt(m_lastPos = data.position);
            //});
            //Util.UnityHelper.Register(m_cachedArea, EventTriggerType.PointerUp, ev =>
            //{
            //    if (m_prepareFire)
            //    {
            //        m_prepareFire = false;
            //        FightSceneManager.Me().gunObj.FireAt(Vector2.zero, false);
            //    }
            //});
        }
        //public void OnPointerDown(PointerEventData ev)
        //{
        //    m_gunStatus = new Cursor() { down = true, position = ev.position };
        //    FireAt(m_gunStatus.position);
        //}
        //public void OnPointerUp(PointerEventData ev)
        //{
        //    FightSceneManager.Me().gunObj.FireAt(Vector2.zero, false);
        //}
        //public void OnPointerExit(PointerEventData ev)
        //{
        //    OnPointerUp(ev);
        //}
        //public override void OnUnInitialize()
        //{
        //    FightSceneManager.Inst().UnInitialize();
        //    base.OnUnInitialize();
        //}
        public override void OnShow()
        {
            base.OnShow();
            Util.TimerManager.Inst().onFrameUpdate.Add(Update);
            FightSceneManager.Inst().Initialize(this);
            FightSceneManager.OnCoinFly = CoinFly;
            FightSceneManager.OnAlphabetFly = AlphabetFly;

            IFEBattle.onDiffLvUp.Add(OnDiffLvlUp);
            IFEBattle.onPlayerSync.Add(OnPlayerSync);
            IFEBattle.onFishUpdate.Add(OnFishUpdate);
            IFEBattle.onFishRmv.Add(OnRmv);
            FEMessageModule.Inst().onGameMessage.Add(OnGameMessage);
            FEDefenderObj.onPhysicAttacker.Add(this);
            FightSceneManager.Inst().onTargetSelect.Add(OnTargetSelect);
            FightSceneManager.Inst().onResult.Add(OnResult);

            OnDiffLvlUp();
            OnPlayerSync();
            m_gunStatus = new Cursor() { down = false, position = Vector2.zero };
            m_contentRc = tipText.GetComponent<RectTransform>();
            m_msgs.Clear();
            tipText.text = string.Empty;
            OnTargetSelect(EntityId.Empty);

            SceneManager.Inst().terrain.SetCamera(Framework.Instance.camera3d);
        }
        public override void OnHide()
        {
            IFEBattle.onDiffLvUp.Rmv(OnDiffLvlUp);
            IFEBattle.onPlayerSync.Rmv(OnPlayerSync);
            IFEBattle.onFishUpdate.Rmv(OnFishUpdate);
            IFEBattle.onFishRmv.Rmv(OnRmv);
            FEMessageModule.Inst().onGameMessage.Rmv(OnGameMessage);
            FEDefenderObj.onPhysicAttacker.Rmv(this);
            FightSceneManager.Inst().onTargetSelect.Rmv(OnTargetSelect);
            FightSceneManager.Inst().onResult.Rmv(OnResult);

            FightSceneManager.OnCoinFly = null;
            FightSceneManager.OnAlphabetFly = null;
            FightSceneManager.Inst().UnInitialize();
            Util.TimerManager.Inst().onFrameUpdate.Rmv(Update);
            base.OnHide();
        }
        void OnDiffLvlUp()
        {
            //var diff = FightSceneManager.m_battle.level;
            //diffText.text = diff.lvl.id.ToString();
            //var tipCfg = tab.combatMissionTip.Inst().Find(diff.lvUpType + 1000);
            //if (tipCfg != null && !string.IsNullOrEmpty(tipCfg.tip))
            //{
            //    var sb = new StringBuilder(tipCfg.tip);
            //    sb.Append('\n').Append(diff.lvUpProg.ToStyle1());
            //    diffUpText.text = sb.ToString();
            //}
            //else
            //{
            //    diffUpText.text = string.Empty;
            //}
        }
        protected virtual void OnPlayerSync()
        {
            var players = FightSceneManager.mod.players;
            for (int index = 0; index < players.Count; ++index)
            {
                var player = players[index] as FEDefenderObj;
                if (player != null)
                {
                    GetPlayerObj(player.sessionId).Attach(player);
                }
            }
        }
        void OnFishUpdate(IFEMonster fishObj)
        {
            if (fishObj.UId() == FightSceneManager.Inst().target.uniqueId)
            {
                targetIcon.gameObject.SetActive(true);
                var directVisit = fishObj.GetData();
                var monsterCfg = tab.monster.Inst().Find(directVisit.moduleId);
                var resCfg = tab.objRes.Inst().Find(monsterCfg.resId);
                targetIcon.sprite = SpritesManager.Inst().Find(resCfg.icon);
                targetIcon.SetNativeSize();
                for (int index = 0; index < targetHpRoots.Length; ++index)
                {
                    targetHpRoots[index].enabled = index < directVisit.hp.current;
                }
                int multi = (directVisit.hp.current / targetHpRoots.Length);
                if (multi > 10) { targetHpValue.text = "x?"; }
                else if (multi > 1) { targetHpValue.text = "x" + multi.ToString(); }
                else
                {
                    targetHpValue.text = string.Empty;
                }
            }
        }
        void OnTargetSelect(EntityId eId)
        {
            if (eId != EntityId.Empty)
            {
                OnFishUpdate(FightSceneManager.mod.FindFish(eId.uniqueId));
            }
            else
            {
                targetIcon.gameObject.SetActive(false);
            }
        }
        void OnRmv(int fishUId)
        {
            if (FightSceneManager.Inst().target.uniqueId == fishUId)
            {
                FightSceneManager.Inst().target = EntityId.Empty;
            }
        }
        void OnResult(int mySId)
        {
            NetworkWatcher.Inst().Stop();
            FightSceneManager.Inst().PvpLeave();
            api.coroutineHelper.StartCoroutineImmediate(CleanBattle(mySId));
        }
        IEnumerator CleanBattle(int mySId)
        {
            var combatType = FightSceneManager.mod.combatType;
            var datas = CreateStatisticData(mySId);
            GuiManager.Inst().ShowFrame(typeof(FightCleanFrame).Name);
            yield return WaitSeconds.Delay(3f);
            GuiManager.Inst().HideFrame(typeof(FightCleanFrame).Name);
            MainState.Instance.PopState();
            yield return new WaitMainSubStateSwitchComplete();
            RecordWrong();
            DisplayResult(combatType, datas);
        }
        void RecordWrong()
        {
            KnowledgeModule.Inst().Refresh(FightSceneManager.Inst().statistic.words);
        }
        void DisplayResult(int combatType, List<StatisticData> datas)
        {
            switch (combatType)
            {
                case GlobalDefine.GTypeFightSingle:
                    {
                        var after = FightSceneManager.Inst().afterFight;
                        if (after != null)
                        {
                            after();
                        }
                        else if (datas[0].win)
                        {
                            //var script = GuiManager.Instance.ShowFrame(typeof(ResultClassWinFrame).Name);
                            //script.Display(datas[0], FightSceneManager.dataCached.action);
                        }
                        else
                        {
                            //var script = GuiManager.Inst().ShowFrame<ResultClassFailFrame>();
                            //script.Display(FightSceneManager.dataCached.action);
                        }
                    break;
                    }
                //case GlobalDefine.GTypeFightSingle_Unit:
                //    {
                //        var script = GuiManager.Inst().ShowFrame<ui.ResultUnitFrame>();
                //        script.Display(datas[0]);
                //    }
                //    break;
                default:
                    {
                        //var script = GuiManager.Instance.ShowFrame<geniusbaby.LSharpScript.ResultPvpFrame>();
                        //script.Display(bp.combatType, FightSceneManager.dataCached.classId, datas);
                    }
                    break;
            }
        }
        List<StatisticData> CreateStatisticData(int mySId)
        {
            List<StatisticData> datas = new List<StatisticData>();

            int winnerIndex = -1;
            var data = new StatisticData();

            var players = FightSceneManager.mod.players;
            for (int index = 0; index < players.Count; ++index)
            {
                var statistic = players[index] as FEDefenderObj;

                data.playerId = statistic.uniqueId;
                data.fish.orign = statistic.data.fightScore;
                data.fish.total = (int)(data.fish.orign * (1 + statistic.title.GetAppend(TitleAttr.AddFishScore)));
                
                data.mission.orign = statistic.data.missionScore;
                data.mission.total = data.mission.orign * (100 + statistic.title.GetAppend(TitleAttr.AddMissionScore)) / 100;

                data.rightAward.orign = PlayerData.RightAward(statistic.data.rightWord);
                data.rightAward.total = data.rightAward.orign * (100 + statistic.title.GetAppend(TitleAttr.AddWordRightScore)) / 100;

                data.wrongPunish.orign = PlayerData.WrongPunish(statistic.data.wrongAlphabet);
                data.wrongPunish.total = data.wrongPunish.orign * (100 - statistic.title.GetAppend(TitleAttr.AddAlphabetWrongScore)) / 100;

                data.timeAward.orign = PlayerData.TimeAward(statistic.pet.data.hp.current);
                data.timeAward.total = data.timeAward.orign * (100 - statistic.title.GetAppend(TitleAttr.AddTimeScore)) / 100;

                var add = data.fish.total + data.mission.total + data.rightAward.total + data.timeAward.total;
                var sub = data.wrongPunish.total;
                data.total = add * (100 + statistic.title.GetAppend(TitleAttr.AppendGainScore)) / 100
                    - sub * (100 - statistic.title.GetAppend(TitleAttr.WeakSubScore)) / 100;

                if (statistic.sessionId == mySId)
                {
                    data.fishes = FightSceneManager.Inst().statistic.fishes;
                    data.words = FightSceneManager.Inst().statistic.words;
                    data.missionIds = new List<int>();
                    var missions = statistic.mission.GetMissions();
                    for (int j = 0; j < missions.Count; ++j)
                    {
                        data.missionIds.Add(missions[j].mission.id);
                    }
                }

                if (winnerIndex < 0)
                {
                    winnerIndex = index;
                    data.win = true;
                }
                else
                {
                    var champion = datas[winnerIndex];
                    if (champion.timeAward.orign <= 0 && (data.timeAward.orign > 0 || data.total > champion.total) ||
                        champion.timeAward.orign > 0 && (data.timeAward.orign > 0 && data.total > champion.total))
                    {
                        winnerIndex = index;
                        data.win = true;
                    }
                }
                datas.Add(data);
            }
            return datas;
        }
        void OnGameMessage(FEMessageModule.IMessage<GameMsgType> msg)
        {
            switch (msg.etype)
            {
                case GameMsgType.DiffLvUp:
                    {
                        //var diff = FightSceneManager.m_battle.level;
                        //diffText.text = diff.lvl.id.ToString();
                    }
                    break;
                case GameMsgType.WaveDone:
                    //waveText.text = param.ToString();
                    break;
                case GameMsgType.ModeSwitch:
                    {
                        //var afterSweep = (IAfterSweep)msg.param;
                        //if (afterSweep.mode.mode > 0)
                        //{
                        //    var diff = FightSceneManager.m_battle.level;
                        //    var dd = tab.messageTip.Inst().Find(cfg.CodeDefine.MsgTip_ModeSwitch);
                        //    var t1 = tab.messageTip.Inst().Find((afterSweep.mode.mode < 100) ? cfg.CodeDefine.MsgTip_ModeSpec : cfg.CodeDefine.MsgTip_ModeBoss);
                        //    var t2 = tab.combatMissionTip.Inst().Find(diff.lvUpType + 1000);
                        //    var des = string.Format(dd.des, diff.lvl.id.ToString(), t1.des, t2.tip + diff.lvUpProg.max.ToString());
                        //    var script = GuiManager.Inst().ShowFrame<FightTransmitFrame>();
                        //    script.Display(des);
                        //    //m_msgs.Enqueue(new MsgBoardcast()
                        //    //{
                        //    //    mId = (afterSweep.mode.mode < 100) ? cfg.CodeDefine.MsgTip_ModeSpec : cfg.CodeDefine.MsgTip_ModeBoss,
                        //    //    param = afterSweep.mode.param
                        //    //});
                        //}
                        //if (m_msgs.Count > MaxQueque) { m_msgs.Dequeue(); }
                        //OnBoardcastEvent();

                        var diff = FightSceneManager.mod.level;
                        var dd = geniusbaby.tab.messageTip.Inst().Find(geniusbaby.cfg.CodeDefine.MsgTip_ModeSwitch);
                        var des = string.Format(dd.des, diff.current.name);
                        var frame = GuiManager.Inst().ShowFrame(typeof(FightTransmitFrame).Name);
                        var script = T.As<FightTransmitFrame>(frame);
                        script.Display(des);
                    }
                    break;
                case GameMsgType.WordRight:
                    {
                        var english = (EnglishFill)msg.param;
                        FightSceneManager.Inst().statistic.ReplyWord(msg.sessionId, english.original, english.translation);
                    }
                    break;
            }
        }
        void OnBoardcastEvent()
        {
            if (m_displaying == null)
            {
                m_displaying = m_msgs.Count > 0 ? m_msgs.Dequeue() : null;
                if (m_displaying != null)
                {
                    //var tipCfg = tab.messageTip.Inst().Find(m_displaying.mId);
                    tipText.text = m_displaying;
                    tipText.CrossFadeAlpha(1f, 0f, true);
                    api.coroutineHelper.StartCoroutineImmediate(WaitSeconds.Delay(3f), () =>
                    {
                        tipText.CrossFadeAlpha(0f, 1f, true);
                        m_displaying = null;
                        OnBoardcastEvent();
                    });
                }
            }
        }
        public Cursor GetCurStatus()
        {
            Cursor gunStatus = new Cursor();
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    gunStatus.down = (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled);
                    gunStatus.position = gunStatus.down ? touch.position : Vector2.zero;
                }
                else
                {
                    gunStatus.down = false;
                    gunStatus.position = Vector2.zero;
                }
            }
            else
            {
                gunStatus.down = Input.GetMouseButton(0);
                gunStatus.position = Input.mousePosition;
            }

            if (gunStatus.down)
            {
                m_lastPress = gunStatus;
            }

            return gunStatus;
        }
        void Update()
        {
            Cursor curStatus;
            if (GuiManager.Instance.GetTopAsFrame().gameObject == api.gameObject && !FightSceneManager.Inst().pause)
            {
                curStatus = GetCurStatus();
            }
            else
            {
                curStatus = new Cursor() { down = false, position = Vector2.zero };
            }
            Press(curStatus);
            FightSceneManager.MyObj().GunUpdate();
        }
        void Press(Cursor curStatus)
        {
            if (curStatus.down)
            {
                bool fireAt = false;
                if (m_gunStatus.down)
                {
                    var delta = m_gunStatus.position - curStatus.position;
                    var torrelent = GetInputDistance();
                    fireAt = (Mathf.Abs(delta.x) >= torrelent || Mathf.Abs(delta.y) >= torrelent);
                }
                else
                {
                    fireAt = true;
                }
                if (fireAt)
                {
                    m_gunStatus = curStatus;
                    FireAt(curStatus.position);
                }
            }
            else
            {
                m_gunStatus = curStatus;
                FightSceneManager.MyObj().FireAt(Vector2.zero, false);
            }
        }
        public void FireAt(Vector2 screenPos)
        {
            var eventData = Desktop.MakeEventData(screenPos);
            List<RaycastResult> list = new List<RaycastResult>();
            (api as LSharpFrame).raycaster.Raycast(eventData, list);
            if (list.Count <= 0)
            {
                FightSceneManager.MyObj().FireAt(screenPos, true);
            }
        }
        int GetInputDistance()
        {
            bool useTouch = (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android);
            return useTouch ? 10 : 1;
        }
        public void OnFishNormalKill(int sessionId, int fishUId)
        {
            var fishObj = FightSceneManager.Inst().Find<FishObj>(Obj3DType.Fish, fishUId);
            fishObj.DisplayKill(sessionId);
        }
        public void OnFishReplyRight(int sessionId, int fishUId, int replyIndex)
        {
            var fishObj = FightSceneManager.Inst().Find<FishObj>(Obj3DType.Fish, fishUId);
            fishObj.DisplayReply(sessionId, new EnglishDisplayParam(fishObj.attachValue, true, replyIndex));
        }
        public void OnFishReplyWrong(int sessionId, int fishUId, int replyIndex)
        {
            var fishObj = FightSceneManager.Inst().Find<FishObj>(Obj3DType.Fish, fishUId);
            fishObj.DisplayReply(sessionId, new EnglishDisplayParam(fishObj.attachValue, false, replyIndex));
        }
        //public void OnBoxAttack(int sessionId, FEBox box)
        //{
        //    var fishObj = FightSceneManager.Inst().Find<FishObj>(Obj3DType.Fish, box.fish.UId());
        //    fishObj.GetAttackedBox(sessionId, box);
        //}
        public void CoinFly(int playerId, Transform from)
        {
            Vector2 fromPoint = GetPosition(from);
            GetPlayerObj(playerId).DisplayCoinFly(m_cachedArea, fromPoint, m_param.flyTime, m_param.coinCurve);
        }
        public void DirectReply(int playerId)
        {

        }
        public void AlphabetFly(int playerId, Transform from, EnglishDisplayParam edp)
        {
            Vector2 fromPoint = GetPosition(from);
            GetPlayerObj(playerId).DisplayAlphabetFly(edp, m_cachedArea, fromPoint, m_param.flyTime, m_param.charCurve);
        }
        
        public Vector2 GetPosition(Transform trans)
        {
            if (trans)
            {
                Vector2 localPos;
                Framework.C3DWorldPosRectangleLocal(trans.position, out localPos);
                return localPos;
            }
            return Vector2.zero;
        }
    }
}