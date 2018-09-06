using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FightSingleFrame : IFightFrame
    {
//generate code begin
        public Image Target_icon;
        public RectTransform Target_icon_hp;
        public Text Target_icon_value;
        public Text Clip_tip;
        public Button Skill_mainSki;
        public Image Skill_mainSki_icon;
        public Image Skill_mainSki_mask;
        public Button Skill_extraSki;
        public Image Skill_extraSki_icon;
        public LSharpAPI fightPlayerPanel;
        public Button quit;
        void __LoadComponet(Transform transform)
        {
            Target_icon = transform.Find("Target/@icon").GetComponent<Image>();
            Target_icon_hp = transform.Find("Target/@icon/@hp").GetComponent<RectTransform>();
            Target_icon_value = transform.Find("Target/@icon/@value").GetComponent<Text>();
            Clip_tip = transform.Find("Clip/@tip").GetComponent<Text>();
            Skill_mainSki = transform.Find("Skill/@mainSki").GetComponent<Button>();
            Skill_mainSki_icon = transform.Find("Skill/@mainSki/@icon").GetComponent<Image>();
            Skill_mainSki_mask = transform.Find("Skill/@mainSki/@mask").GetComponent<Image>();
            Skill_extraSki = transform.Find("Skill/@extraSki").GetComponent<Button>();
            Skill_extraSki_icon = transform.Find("Skill/@extraSki/@icon").GetComponent<Image>();
            fightPlayerPanel = transform.Find("@fightPlayerPanel").GetComponent<LSharpAPI>();
            quit = transform.Find("@quit").GetComponent<Button>();
        }
        void __DoInit()
        {
            fightPlayerPanel.OnInitialize("geniusbaby.LSharpScript.FightPlayerPanel");
        }
        void __DoUninit()
        {
            fightPlayerPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            fightPlayerPanel.OnShow();
        }
        void __DoHide()
        {
            fightPlayerPanel.OnHide();
        }
//generate code end
        Scenario m_scenerio = new Scenario();
        MeshRenderer[] m_targetHpRoots;
        public override Image targetIcon { get { return Target_icon; } }
        public override MeshRenderer[] targetHpRoots { get { return m_targetHpRoots; } }
        public override Text targetHpValue { get { return Target_icon_value; } }
        public override Text tipText { get { return Clip_tip; } }
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            m_targetHpRoots = Target_icon_hp.GetComponentsInChildren<MeshRenderer>(false);

            Skill_mainSki.onClick.AddListener(() =>
            {
                var defender = FightSceneManager.Me() as FEDefenderObj;
                CastSki(defender.pet.main);
            });
            Skill_extraSki.onClick.AddListener(() =>
            {
                var defender = FightSceneManager.Me() as FEDefenderObj;
                CastSki(defender.extra);
            });
            quit.onClick.AddListener(() =>
            {
                var frame = GuiManager.Inst().ShowFrame(typeof(MessageBoxFrame).Name);
                var script = T.As<MessageBoxFrame>(frame);
                script.SetDesc("quit game ?", MsgBoxType.Mbt_OkCancel);
                script.SetDelegater(() =>
                {
                    FightSceneManager.Inst().PvpLeave();
                    MainState.Instance.PopState();
                });
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
            FEDefenderObj.onPlayerUpdate.Add(OnPlayerUpdate);
            IFESkill.onSkillSync.Add(OnSkillSync);
            MusicManager.Instance.CrossFadeTo(GamePath.music.musicCombat);
            m_scenerio.InitScenario();
        }
        public override void OnHide()
        {
            m_scenerio.UnInitScenario();
            FEDefenderObj.onPlayerUpdate.Rmv(OnPlayerUpdate);
            IFESkill.onSkillSync.Rmv(OnSkillSync);
            __DoHide();
            base.OnHide();
        }

        public override IPlayer2DObj GetPlayerObj(int sessionId) { return T.As<FightPlayerPanel>(fightPlayerPanel); }
        protected override void OnPlayerSync()
        {
            base.OnPlayerSync();
            var defender = FightSceneManager.Me() as FEDefenderObj;
            var skiCfg = tab.skill.Inst().Find(defender.pet.main.skiId);
            Skill_mainSki_icon.sprite = SpritesManager.Inst().Find(skiCfg.icon);
            OnSkillSync(defender);
            OnEnengyUpdate();
        }
        void OnSkillSync(FEDefenderObj defender)
        {
            if (defender.sessionId == FightSceneManager.MySId())
            {
                if (defender.extra.skiId > 0)
                {
                    Skill_extraSki.gameObject.SetActive(true);
                    var skiCfg = tab.skill.Inst().Find(defender.extra.skiId);
                    Skill_extraSki_icon.sprite = SpritesManager.Inst().Find(skiCfg.icon);
                }
                else
                {
                    Skill_extraSki.gameObject.SetActive(false);
                }
            }
        }
        void OnPlayerUpdate(FEDefenderObj player)
        {
            if (player.sessionId == FightSceneManager.MySId())
            {
                OnEnengyUpdate();
            }
        }
        void OnEnengyUpdate()
        {
            var defender = FightSceneManager.Me() as FEDefenderObj;
            Skill_mainSki_mask.fillAmount = 1f - defender.pet.main.GetPercent();
        }
        void CastSki(IFESkill ski)
        {
            if (ski.CanPrepare())
            {
                var skiCfg = tab.skill.Inst().Find(ski.skiId);
                if (skiCfg.castType == cfg.skill.NeedPrepare)
                {
                    FightSceneManager.MyObj().PrepareSkill(skiCfg.id);
                }
                else
                {
                    TcpNetwork.Inst().Send(new pps.GSrvSkillCastReport() { skiId = skiCfg.id, coord = 0 });
                }
            }
        }
    }
}
