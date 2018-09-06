using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ResultFailFrame : ILSharpScript
    {
//generate code begin
        public Button reborn;
        public Image reborn_costType;
        public Text reborn_costValue;
        public Button exit;
        void __LoadComponet(Transform transform)
        {
            reborn = transform.Find("@reborn").GetComponent<Button>();
            reborn_costType = transform.Find("@reborn/@costType").GetComponent<Image>();
            reborn_costValue = transform.Find("@reborn/@costValue").GetComponent<Text>();
            exit = transform.Find("@exit").GetComponent<Button>();
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
        Action m_finish;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            reborn.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                FightSceneManager.Inst().PVEReborn();
            });
            exit.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                m_finish();
            });
        }
        public void Display(Action finish)
        {
            m_finish = finish;
        }
    }
}
