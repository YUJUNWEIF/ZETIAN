using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper BG_Lc_clip_petPanel;
        public GuiBar BG_Lc_bar;
        public LSharpAPI BG_PetDetailPanel;
        public Button BG_operate_equip;
        public Button BG_operate_lvUp;
        public Image BG_operate_lvUp_costType;
        public Text BG_operate_lvUp_costValue;
        public Button BG_operate_step;
        public RectTransform BG_step;
        public LSharpListContainer BG_step_Material;
        public Button BG_step_close;
        public Button BG_step_stepUp;
        public Image BG_step_stepUp_costType;
        public Text BG_step_stepUp_costValue;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_clip_petPanel = transform.Find("BG/Lc/clip/@petPanel").GetComponent<LSharpListSuper>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<GuiBar>();
            BG_PetDetailPanel = transform.Find("BG/@PetDetailPanel").GetComponent<LSharpAPI>();
            BG_operate_equip = transform.Find("BG/operate/@equip").GetComponent<Button>();
            BG_operate_lvUp = transform.Find("BG/operate/@lvUp").GetComponent<Button>();
            BG_operate_lvUp_costType = transform.Find("BG/operate/@lvUp/@costType").GetComponent<Image>();
            BG_operate_lvUp_costValue = transform.Find("BG/operate/@lvUp/@costValue").GetComponent<Text>();
            BG_operate_step = transform.Find("BG/operate/@step").GetComponent<Button>();
            BG_step = transform.Find("BG/@step").GetComponent<RectTransform>();
            BG_step_Material = transform.Find("BG/@step/@Material").GetComponent<LSharpListContainer>();
            BG_step_close = transform.Find("BG/@step/@close").GetComponent<Button>();
            BG_step_stepUp = transform.Find("BG/@step/@stepUp").GetComponent<Button>();
            BG_step_stepUp_costType = transform.Find("BG/@step/@stepUp/@costType").GetComponent<Image>();
            BG_step_stepUp_costValue = transform.Find("BG/@step/@stepUp/@costValue").GetComponent<Text>();
        }
        void __DoInit()
        {
            BG_Lc_clip_petPanel.OnInitialize();
            BG_PetDetailPanel.OnInitialize("geniusbaby.LSharpScript.PetDetailPanel");
            BG_step_Material.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Lc_clip_petPanel.OnUnInitialize();
            BG_PetDetailPanel.OnUnInitialize();
            BG_step_Material.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_clip_petPanel.OnShow();
            BG_PetDetailPanel.OnShow();
            BG_step_Material.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_clip_petPanel.OnHide();
            BG_PetDetailPanel.OnHide();
            BG_step_Material.OnHide();
        }
//generate code end
        Item m_lvUpNeeds;
        PetBase m_pet;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_clip_petPanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(PetItemPanel).Name);
            __DoInit();
            BG_Lc_clip_petPanel.selectListener.Add(() =>
            {
                var no = BG_Lc_clip_petPanel.itemSelected.First;
                if (no != null)
                {
                    var pet = (PetBase)BG_Lc_clip_petPanel.values[no.Value];
                    T.As<PetDetailPanel>(BG_PetDetailPanel).Display(m_pet = pet);
                    OnPetUpdate(pet);
                }
            });
            BG_operate_lvUp.onClick.AddListener(() =>
            {
                var has = TokenModule.Inst().Get(m_lvUpNeeds.mId);
                if (has >= m_lvUpNeeds.count)
                {
                    HttpNetwork.Inst().Communicate(new PetLvUpRequest() { id = T.As<PetDetailPanel>(BG_PetDetailPanel).pet.uniqueId });
                }
            });
            BG_operate_step.onClick.AddListener(() =>
            {
                Util.UnityHelper.Show(BG_step);
                Util.UnityHelper.Hide(T.As<PetDetailPanel>(BG_PetDetailPanel).Rc_com);
            });
            BG_step_close.onClick.AddListener(() =>
            {
                Util.UnityHelper.Hide(BG_step);
                Util.UnityHelper.Show(T.As<PetDetailPanel>(BG_PetDetailPanel).Rc_com);
            });
            BG_operate_equip.onClick.AddListener(() => HttpNetwork.Inst().Communicate(new PetEquipRequest() { id = m_pet.uniqueId }));
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
            PetModule.Inst().onEquipedSync.Add(OnEquipedSync);
            PetModule.Inst().onPetUpdate.Add(OnPetUpdate);
            
            var pets = PetModule.Inst().pets.ConvertAll(it => (object)it);
            BG_Lc_clip_petPanel.SetValues(pets);
            var index = pets.FindIndex(it => ((PetBase)it).uniqueId == PetModule.Inst().equipId);
            BG_Lc_clip_petPanel.SelectIndex(index);
            OnEquipedSync();
        }
        public override void OnHide()
        {
            PetModule.Inst().onEquipedSync.Rmv(OnEquipedSync);
            PetModule.Inst().onPetUpdate.Rmv(OnPetUpdate);
            __DoHide();
            base.OnHide();
        }
        void OnPetUpdate(PetBase pet)
        {
            var petCfg = tab.hero.Inst().Find(pet.mId);

            if (pet.lv < petCfg.star.lvLimit)
            {
                BG_operate_lvUp.gameObject.SetActive(true);
                var expCfg = tab.heroExp.Inst().Find(it => it.lv == pet.lv);
                m_lvUpNeeds = new Item(expCfg.cost.id) { count = expCfg.cost.count };
            }
            else
            {
                BG_operate_lvUp.gameObject.SetActive(false);
            }
        }
        void OnEquipedSync()
        {
            Util.UnityHelper.ShowHide(BG_operate_equip, PetModule.Inst().equipId != m_pet.uniqueId);
        }
    }
}
