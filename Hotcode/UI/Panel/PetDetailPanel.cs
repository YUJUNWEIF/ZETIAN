using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetDetailPanel : ILSharpScript
    {
//generate code begin
        public Transform node;
        public Image Top_family;
        public Text Top_name;
        public Text Top_lvl;
        public RectTransform Top_star;
        public Text Top_des;
        public Text Rc_Attr_Hp_value;
        public Text Rc_Attr_Resistence_value;
        public Text Rc_Attr_FireSpeed_value;
        public Text Rc_Attr_BulletSpeed_value;
        public Text Rc_Attr_Damage_value;
        public Text Rc_Attr_CritRate_value;
        public RectTransform Rc_com;
        public Image Rc_com_SkiMain_icon;
        public Text Rc_com_SkiMain_name;
        public Text Rc_com_SkiMain_des;
        void __LoadComponet(Transform transform)
        {
            node = transform.Find("@node").GetComponent<Transform>();
            Top_family = transform.Find("Top/@family").GetComponent<Image>();
            Top_name = transform.Find("Top/@name").GetComponent<Text>();
            Top_lvl = transform.Find("Top/@lvl").GetComponent<Text>();
            Top_star = transform.Find("Top/@star").GetComponent<RectTransform>();
            Top_des = transform.Find("Top/@des").GetComponent<Text>();
            Rc_Attr_Hp_value = transform.Find("Rc/Attr/Hp/@value").GetComponent<Text>();
            Rc_Attr_Resistence_value = transform.Find("Rc/Attr/Resistence/@value").GetComponent<Text>();
            Rc_Attr_FireSpeed_value = transform.Find("Rc/Attr/FireSpeed/@value").GetComponent<Text>();
            Rc_Attr_BulletSpeed_value = transform.Find("Rc/Attr/BulletSpeed/@value").GetComponent<Text>();
            Rc_Attr_Damage_value = transform.Find("Rc/Attr/Damage/@value").GetComponent<Text>();
            Rc_Attr_CritRate_value = transform.Find("Rc/Attr/CritRate/@value").GetComponent<Text>();
            Rc_com = transform.Find("Rc/@com").GetComponent<RectTransform>();
            Rc_com_SkiMain_icon = transform.Find("Rc/@com/SkiMain/@icon").GetComponent<Image>();
            Rc_com_SkiMain_name = transform.Find("Rc/@com/SkiMain/@name").GetComponent<Text>();
            Rc_com_SkiMain_des = transform.Find("Rc/@com/SkiMain/@des").GetComponent<Text>();
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
        private PetDisplayObj m_petDisplay;
        private Image[] m_stars;
        public PetBase pet { get; private set; }

        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            m_petDisplay = new GameObject("heroDisplay").AddComponent<PetDisplayObj>();
            m_stars = Top_star.GetComponentsInChildren<Image>();
        }
        public override void OnUnInitialize()
        {
            GameObjPool.Instance.DestroyObj(m_petDisplay.gameObject);
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            PetModule.Inst().onPetUpdate.Add(OnPetUpdate);
        }
        public override void OnHide()
        {
            PetModule.Inst().onPetUpdate.Rmv(OnPetUpdate);
            m_petDisplay.UnInitialize();
            __DoHide();
            base.OnHide();
        }
        void OnPetUpdate(PetBase pet)
        {
            this.pet = pet;
            if (m_petDisplay.obj)
            {
                if (m_petDisplay.entityId.uniqueId != pet.uniqueId || m_petDisplay.moduleId != pet.mId)
                {
                    m_petDisplay.UnInitialize();
                }
            }
            if (!m_petDisplay.obj)
            {
                m_petDisplay.Change(pet.mId);
                m_petDisplay.gameObject.layer = Util.TagLayers.UI;
                Util.UnityHelper.Show(m_petDisplay, node, true);
                //m_petDisplay.Display(petCfg.animIdle);
                m_petDisplay.Idle();
            }
            var heroCfg = geniusbaby.tab.hero.Instance.Find(pet.mId);
            Top_name.text = heroCfg.name;
            Top_lvl.text = @"Lv." + pet.lv.ToString();
            Top_des.text = heroCfg.des;

            var skillCfg = geniusbaby.tab.skill.Inst().Find(heroCfg.star.skiId);
            Rc_com_SkiMain_name.text = skillCfg.name;
            Rc_com_SkiMain_des.text = skillCfg.des;

            //valueText.text = string.Format(tab.attr.Inst().Find((int)m_display.type).des, m_display.value.ToString("0.00"));
            Rc_Attr_FireSpeed_value.text = pet.entity.fireCd.ToString("0.0");
            Rc_Attr_BulletSpeed_value.text = pet.entity.bulletSpeed.ToString();
            Rc_Attr_Damage_value.text = pet.entity.attack.ToString();
            Rc_Attr_CritRate_value.text = pet.entity.critRate.ToString() + "%";

            for (int index = 0; index < m_stars.Length; ++index)
            {
                m_stars[index].gameObject.SetActive(index < heroCfg.star.star);
            }
        }
        public void Display(PetBase pet)
        {
            OnPetUpdate(this.pet = pet);
        }
    }
}
