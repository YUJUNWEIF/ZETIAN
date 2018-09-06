using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FightMultiFrame : IFightFrame
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
        public LSharpAPI fightPlayerPanel_1;
        public LSharpAPI fightPlayerPanel_2;
        public Toggle chat;
        public Button chatSetting;
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
            fightPlayerPanel_1 = transform.Find("@fightPlayerPanel_1").GetComponent<LSharpAPI>();
            fightPlayerPanel_2 = transform.Find("@fightPlayerPanel_2").GetComponent<LSharpAPI>();
            chat = transform.Find("@chat").GetComponent<Toggle>();
            chatSetting = transform.Find("@chatSetting").GetComponent<Button>();
        }
        void __DoInit()
        {
            fightPlayerPanel_1.OnInitialize("geniusbaby.LSharpScript.FightPlayerPanel");
            fightPlayerPanel_2.OnInitialize("geniusbaby.LSharpScript.FightPlayerPanel");
        }
        void __DoUninit()
        {
            fightPlayerPanel_1.OnUnInitialize();
            fightPlayerPanel_2.OnUnInitialize();
        }
        void __DoShow()
        {
            fightPlayerPanel_1.OnShow();
            fightPlayerPanel_2.OnShow();
        }
        void __DoHide()
        {
            fightPlayerPanel_1.OnHide();
            fightPlayerPanel_2.OnHide();
        }
//generate code end
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
            chatSetting.onClick.AddListener(() =>
            {
                //GuiManager.Inst().ShowFrame<GMapChatFrame>();
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
            MusicManager.Instance.CrossFadeTo(GamePath.music.musicCombat);
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public override IPlayer2DObj GetPlayerObj(int sessionId)
        {
            switch (sessionId)
            {
                case 0:
                case 1: return T.As<FightPlayerPanel>(fightPlayerPanel_1);
                case 2: return T.As<FightPlayerPanel>(fightPlayerPanel_2);
            }
            return null;
        }
    }
}
