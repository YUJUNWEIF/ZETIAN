using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetItemPanel : ILSharpScript
    {
//generate code begin
        public Image equipedRoot;
        public Image icon;
        public Text name;
        public Text lv;
        public RectTransform star;
        void __LoadComponet(Transform transform)
        {
            equipedRoot = transform.Find("@equipedRoot").GetComponent<Image>();
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            lv = transform.Find("@lv").GetComponent<Text>();
            star = transform.Find("@star").GetComponent<RectTransform>();
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
        Image[] m_stars;
        PetBase m_pet;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            m_stars = star.GetComponentsInChildren<Image>();
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                if (com.ListComponent.singleSelect)
                {
                    com.ListComponent.SelectIndex(com.index);
                }
                else
                {
                    com.ListComponent.SwitchSelectIndex(com.index);
                }
            });
        }
        public PetBase GetValue() { return m_pet; }
        public void SetValue(PetBase value)
        {
            m_pet = value;
            var petCfg = tab.hero.Inst().Find(m_pet.mId);
            var resCfg = tab.objRes.Inst().Find(petCfg.star.resId);
            name.text = petCfg.name;
            icon.sprite = SpritesManager.Inst().Find(resCfg.icon);
            lv.text = @"Lv." + m_pet.lv.ToString();
            for (int index = 0; index < m_stars.Length; ++index)
            {
                m_stars[index].enabled = index < petCfg.star.star;
            }
            OnEquipedSync();
        }
        public override void OnShow()
        {
            base.OnShow();
            PetModule.Inst().onEquipedSync.Add(OnEquipedSync);
        }
        public override void OnHide()
        {
            PetModule.Inst().onEquipedSync.Rmv(OnEquipedSync);
            base.OnHide();
        }
        void OnEquipedSync()
        {
            equipedRoot.enabled = (PetModule.Inst().equipId == m_pet.uniqueId);
        }
    }
}
