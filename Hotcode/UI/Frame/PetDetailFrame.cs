using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetDetailFrame : ILSharpScript
    {
//generate code begin
        public LSharpAPI BG_PetDetailPanel;
        public Button BG_equip;
        public Button BG_close;
        public Image BG_fx;
        void __LoadComponet(Transform transform)
        {
            BG_PetDetailPanel = transform.Find("BG/@PetDetailPanel").GetComponent<LSharpAPI>();
            BG_equip = transform.Find("BG/@equip").GetComponent<Button>();
            BG_close = transform.Find("BG/@close").GetComponent<Button>();
            BG_fx = transform.Find("BG/@fx").GetComponent<Image>();
        }
        void __DoInit()
        {
            BG_PetDetailPanel.OnInitialize("geniusbaby.LSharpScript.PetDetailPanel");
        }
        void __DoUninit()
        {
            BG_PetDetailPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_PetDetailPanel.OnShow();
        }
        void __DoHide()
        {
            BG_PetDetailPanel.OnHide();
        }
//generate code end
        PetBase m_pet;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            //BG_equip.onClick.AddListener(() => HttpNetwork.Inst().Communicate(new PetEquipRequest() { id = m_pet.uniqueId }));
            BG_close.onClick.AddListener(() => GuiManager.Inst().HideFrame(api.name));
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
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void Display(PetBase pet)
        {
            T.As<PetDetailPanel>(BG_PetDetailPanel).Display(m_pet = pet);
            //OnEquipedSync();
        }
        //void OnEquipedSync()
        //{
        //    Util.UnityHelper.ShowHide(equipButton, PetModule.Inst().equipId != m_pet.uniqueId);
        //}
    }
}
