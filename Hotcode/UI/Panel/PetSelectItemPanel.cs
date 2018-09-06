using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetSelectItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        public Text lv;
        public RectTransform star;
        public Image equiped;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            lv = transform.Find("@lv").GetComponent<Text>();
            star = transform.Find("@star").GetComponent<RectTransform>();
            equiped = transform.Find("@equiped").GetComponent<Image>();
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
        SelectValue<PetBase> m_pet;
        Image[] m_stars;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            star.GetComponentsInChildren<Image>(false);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                if (m_pet.canSelect)
                {
                    if (com.ListComponent.singleSelect) { com.ListComponent.SelectIndex(com.index); }
                    else
                    {
                        com.ListComponent.SwitchSelectIndex(com.index);
                    }
                }
            });
        }
        public SelectValue<PetBase> GetValue() { return m_pet; }
        public void SetValue(SelectValue<PetBase> value)
        {
            m_pet = value;
            var petCfg = tab.hero.Inst().Find(m_pet.value.mId);
            var resCfg = tab.objRes.Inst().Find(m_pet.value.mId);
            name.text = petCfg.name;
            icon.sprite = SpritesManager.Inst().Find(resCfg.icon);
            lv.text = @"Lv." + m_pet.value.lv.ToString();
            for (int index = 0; index < m_stars.Length; ++index)
            {
                m_stars[index].enabled = index < petCfg.star.star;
            }
            equiped.enabled = (PetModule.Inst().equipId == m_pet.value.uniqueId);
        }
    }
}
