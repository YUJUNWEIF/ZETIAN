using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FightPlayerPanel : ILSharpScript, IPlayer2DObj, IBfGetter
    {
//generate code begin
        public Text info_name;
        public Text info_score;
        public GuiBar info_hp;
        public Text info_hp_value;
        public TextImage2D question;
        public Text translation;
        void __LoadComponet(Transform transform)
        {
            info_name = transform.Find("info/@name").GetComponent<Text>();
            info_score = transform.Find("info/@score").GetComponent<Text>();
            info_hp = transform.Find("info/@hp").GetComponent<GuiBar>();
            info_hp_value = transform.Find("info/@hp/@value").GetComponent<Text>();
            question = transform.Find("@question").GetComponent<TextImage2D>();
            translation = transform.Find("@translation").GetComponent<Text>();
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
        FEDefenderObj m_playerObj;
        EnglishFill m_english;
        BuffDisplayer m_displayer;
        ui.FightFrameParam m_param;
        //FXControl m_prepareFx;
        PlayerState oldState;
        char[] chaos = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '-', '+', '?', '|' };
        char[] chaosEnglish = new char[32];
        
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            m_displayer = new BuffDisplayer(api.transform.parent, this);
            m_param = api.GetComponent<ui.FightFrameParam>();
        }
        public bool ShouldProc()
        {
            return m_playerObj == FightSceneManager.Me();
        }
        public string Get(int skiId)
        {
            //var fishCfg = tab.skill.Inst().Find(skiId);
            //return fishCfg != null ? fishCfg.buffPrepFx : null;
            return string.Empty;
        }
        public override void OnShow()
        {
            base.OnShow();
            FELevel.onMission.Add(OnMissionCompSync);
            FEDefenderObj.onPlayerUpdate.Add(OnPlayerUpdate);
            FEDefenderObj.onChangePet.Add(OnChangeGun);
            FEDefenderObj.onEnglishSync.Add(OnEnglishSync);
            CombatMissionManager.onMissionSync.Add(OnMissionSync);

            Util.TimerManager.Inst().Add(OnTimer, 1000);
            MusicManager.Instance.CrossFadeTo(GamePath.music.musicCombat);
        }
        public override void OnHide()
        {
            //if (m_prepareFx)
            //{
            //    FXControl.Destroy(m_prepareFx);
            //    m_prepareFx = null;
            //}          
            Dettach();
            //m_gunObj.UnInitialize();
            //m_petObj.UnInitialize();
            FELevel.onMission.Rmv(OnMissionCompSync);
            FEDefenderObj.onPlayerUpdate.Rmv(OnPlayerUpdate);
            FEDefenderObj.onChangePet.Rmv(OnChangeGun);
            FEDefenderObj.onEnglishSync.Rmv(OnEnglishSync);
            CombatMissionManager.onMissionSync.Rmv(OnMissionSync);

            Util.TimerManager.Inst().Remove(OnTimer);
            base.OnHide();
        }
        public void Attach(FEDefenderObj playerObj)
        {
            m_playerObj = playerObj;
            oldState = PlayerState.Normal;
            if (m_playerObj != null)
            {
                m_playerObj.buff.listeners.Add(m_displayer);
                info_name.text = playerObj.pdp.name;
            }
            OnPlayerUpdate(m_playerObj);
            OnEnglishSync(m_playerObj);
            OnMissionSync(m_playerObj);

            //m_gunObj.Initialize(playerObj);
            //m_petObj.Initialize(playerObj);
        }
        void Dettach()
        {
            if (m_playerObj != null)
            {
                m_playerObj.buff.listeners.Rmv(m_displayer);
                m_playerObj = null;
            }
            m_displayer.OnSync();
        }
        //public void DisplayPrepareSkill(int skiId)
        //{
        //    var skillCfg = tab.skill.Inst().Find(skiId);
        //    if (!string.IsNullOrEmpty(skillCfg.buffPrepFx))
        //    {
        //        m_prepareFx = FXControl.Create(GamePath.asset.fx3D, skillCfg.buffPrepFx);
        //        m_prepareFx.Play(m_playerObj.gunObj.transform);
        //        Util.UnityHelper.SetLayerRecursively(m_prepareFx.transform, Util.TagLayers.UI);
        //    }
        //}
        //public void DisplayCastSkill(int skiId)
        //{
        //    if (m_prepareFx)
        //    {
        //        FXControl.Destroy(m_prepareFx);
        //        m_prepareFx = null;
        //    }
        //}

        void OnPlayerUpdate(FEDefenderObj obj)
        {
            if (m_playerObj == null) { return; }

            if (m_playerObj.sessionId == obj.sessionId)
            {
                m_playerObj = obj;

                PlayerState nowState = obj.state;
                if ((oldState & PlayerState.Chaos) != (obj.state & PlayerState.Chaos))
                {
                    OnEnglishSync(obj);
                    oldState = obj.state;
                }

                var up = obj.data;
                info_score.text = (up.fightScore + up.missionScore).ToString();
                OnTimer();
            }
        }
        public void OnEnglishSync(FEDefenderObj obj)
        {
            if (m_playerObj == null) { return; }

            if (obj == m_playerObj)
            {
                m_english = obj.aboxer.english;
                if (m_english != null)
                {
                    var e = m_english.original;
                    var trans = m_english.translation.Split(';');
                    var c = trans[0];

                    if (m_playerObj.AnyExist(PlayerState.Chaos))
                    {
                        var se = new StringBuilder();
                        for (int index = 0; index < e.Length; ++index)
                        {
                            var chs = Framework.rand.Range(0, chaos.Length);
                            se.Append(chaos[chs]);
                        }
                        var sc = new StringBuilder();
                        for (int index = 0; index < c.Length; ++index)
                        {
                            var chs = Framework.rand.Range(0, chaos.Length);
                            sc.Append(chaos[chs]);
                        }
                        Display(se.ToString(), sc.ToString(), m_english);
                    }
                    else
                    {
                        Display(e, c, m_english);
                    }
                }
            }
        }
        void Display(string e, string c, EnglishFill english)
        {
            var sv = new Segment.Value()
            {
                bold = (question.fontStyle & FontStyle.Bold) == FontStyle.Bold,
                italic = (question.fontStyle & FontStyle.Italic) == FontStyle.Italic,
                underline = false,
                color = question.color,
                size = question.fontSize,
            };
            var sb = new StringBuilder();
            for (int index = 0; index < e.Length; ++index)
            {
                var exist = english.indexs.FindIndex(it => it == index);
                if (exist >= 0)
                {
                    var filled = (english.states[exist] == EnglishFill.State.Filled);
                    sv.underline = filled;
                    sv.color = filled ? Color.green : Color.grey;
                    var ssb = new StringBuilder(english.original[index].ToString());
                    ssb = UnderlineRegex.Encode(ssb);
                    ssb = ColorRegex.Encode(ssb, sv.color);
                    sb.Append(ssb);
                }
                else
                {
                    sb.Append(english.original[index]);
                }
            }
            question.text = sb.ToString();
            translation.text = c;

            var ap = question.rectTransform.anchoredPosition;
            ap.x = (question.width - question.singleLineWidth) * 0.5f;
            question.rectTransform.anchoredPosition = ap;
        }
        
        void OnMissionCompSync() { OnMissionSync(m_playerObj); }
        void OnMissionSync(FEDefenderObj playerObj)
        {
            //if (m_playerObj == null) { return; }

            //if (m_playerObj.sessionId == playerObj.sessionId)
            //{
            //    var totalMissions = playerObj.mission.GetMissions();
            //    //if (FightSceneManager.m_battle.combatType == GlobalDefine.CombatPVPMulti)
            //    //{
            //    var doings = FightSceneManager.m_battle.level.doings;
            //    var missions = new List<CombatMission>();
            //    for (int index = 0; index < doings.Count; ++index)
            //    {
            //        var exist = totalMissions.FindIndex(it => it.uId == doings[index].uId);
            //        if (exist >= 0) { missions.Add(totalMissions[exist]); }
            //    }
            //    missionPanel.SetValues(missions);
            //    //}
            //    //else
            //    //{
            //    //    var missions = totalMissions.FindAll(it => it.missionId <= 10000);
            //    //    missionPanel.SetValues(missions);
            //    //}
            //    missionPanel.SetColor(m_outlineColor);
            //}
        }
        void OnTimer()
        {
            if (m_playerObj == null) { return; }

            if (FightSceneManager.mod.state == FightState.Normal &&
                !FightSceneManager.Inst().pause)
            {
                var dv = m_playerObj.pet.data.hp;
                if (dv.current < 0) { dv.current = 0; }
                //tickBar.value = dv.current * 1f / dv.max;
                info_hp_value.text = (dv.current / 1000).ToString();
                //Ct_time_sec.color = m_fontColor;
            }
        }
        void OnChangeGun(FEDefenderObj playerObj)
        {
            if (m_playerObj == playerObj)
            {
                var obj = FightSceneManager.Inst().FindPlayer(m_playerObj.sessionId);
                obj._Change(m_playerObj.pet.GetGun());
                //m_gunObj._Change();
            }
        }
        //public void DisplayFlyAndHurtEffect(IPlayer2DObj caster, cfg.skill skillCfg)
        //{
        //    var from = (caster as FightPlayerPanel).gunRoot;
        //    coroutineHelper.StartCoroutine(_Display(from.position, skillCfg));
        //}
        //public void DisplayFlyAndHurtEffect(Vector2 casterPos, cfg.skill skillCfg)
        //{
        //    coroutineHelper.StartCoroutine(_Display(casterPos, skillCfg));
        //}
        public void DisplayCoinFly(Transform scene2d, Vector2 from, float flyTime, AnimationCurve curve)
        {
            var coinGo = GameObjPool.Inst().CreateObjSync(GamePath.asset.terrain, "Coin");
            var coinTo = info_score.transform;

            var anim = coinGo.GetComponent<Animation>();
            if (anim) { anim.Play(); }
            coinGo.transform.parent = scene2d;
            Util.UnityHelper.SetLayerRecursively(coinGo.transform, Util.TagLayers.UI);

            api.coroutineHelper.StartCoroutineImmediate(FlyTo(coinGo, from, coinTo.position, flyTime, curve),
                () => GameObjPool.Inst().DestroyObj(coinGo));
        }
        static CharacterInfo m_temp;
        public void DisplayAlphabetFly(EnglishDisplayParam edp, Transform scene2d, Vector2 fromPoint, float flyTime, AnimationCurve curve)
        {
            //OnEnglishSync(m_playerObj);
            var parser = new RichTextParser();
            var sv = new Segment.Value()
            {
                bold = (question.fontStyle & FontStyle.Bold) == FontStyle.Bold,
                italic = (question.fontStyle & FontStyle.Italic) == FontStyle.Italic,
                underline = false,
                color = question.color,
                size = question.fontSize,
            };
            var segs = parser.Parser(m_english.original, sv);
            float x = 0;
            int replyIndex = 0;
            for (int index = 0; index < segs.Count; ++index)
            {
                var seg = segs[index];
                var style = FontStyle.Normal;
                if (sv.bold) { style |= FontStyle.Bold; }
                if (sv.italic) { style |= FontStyle.Italic; }
                question.font.RequestCharactersInTexture(seg.text, sv.size, style);

                for (int at = 0; at < seg.text.Length; ++at)
                {
                    var c = seg.text[at];
                    if (c < ' ') { continue; }
                    bool hasChar = question.font.GetCharacterInfo(c, out m_temp, seg.value.size, FontStyle.Normal);

                    if (replyIndex == edp.replyIndex)
                    {
                        x += (m_temp.minX + m_temp.maxX) * 0.5f;
                        break;
                    }
                    else
                    {
                        x += m_temp.advance;
                    }
                }
            }
            var rt = question.rectTransform;
            var q = rt.position;
            q.x = rt.position.x - rt.rect.width * 0.5f + x;
            
            var chGo = GameObjPool.Inst().CreateObjSync(GamePath.asset.terrain, edp.rightWrong ? "CharacterRight" : "CharacterWrong");
            if (!edp.rightWrong)
            {
                FightSceneManager.Inst().statistic.WrongAlphabet(m_playerObj.sessionId, m_english.original, m_english.translation);
            }

            var tex = chGo.GetComponentInChildren<Image>();
            tex.sprite = SpritesManager.Inst().Find(edp.fish.GetData().alphabet.ToString());
            Util.UnityHelper.ShowAsChild(chGo.transform, scene2d);
            Util.UnityHelper.SetLayerRecursively(chGo.transform, Util.TagLayers.UI);

            api.coroutineHelper.StartCoroutineImmediate(FlyTo(chGo, fromPoint, q, flyTime, curve),
                () =>
                {
                    GameObjPool.Inst().DestroyObj(chGo);
                    OnEnglishSync(m_playerObj);
                });
        }
        IEnumerator FlyTo(GameObject chGo, Vector2 from, Vector3 to, float flyTime, AnimationCurve curve)
        {
            float startAt = Time.time;
            while (Time.time < startAt + flyTime)
            {
                var key = (Time.time - startAt) / flyTime;
                if (chGo)
                {
                    var chKeyValue = curve.Evaluate(key);
                    chGo.transform.position = Vector3.Lerp(from, to, chKeyValue);
                }
                yield return null;
            }
        }
        //IEnumerator _Display(Vector3 casterPos, cfg.skill skillCfg)
        //{
        //    var fx = FXControl.Create(GamePath.asset.fx3D, skillCfg.castFx, false);
        //    if (fx)
        //    {
        //        fx.Play(transform);
        //        fx.transform.position = casterPos;
        //        fx.transform.rotation = Quaternion.identity;
        //        yield return fx.FlyWithTime(casterPos, gunRoot, Vector2.zero, skillCfg.delayRmvMs * 0.001f);
        //        fx.Stop();
        //        FXControl.Destroy(fx);
        //    }
        //}
    }
}
