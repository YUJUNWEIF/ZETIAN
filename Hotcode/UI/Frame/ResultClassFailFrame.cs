using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ResultClassFailFrame : ILSharpScript
    {
//generate code begin
        public Button close;
        public Button retry;
        public Button detail;
        public LSharpListSuper clip_list;
        void __LoadComponet(Transform transform)
        {
            close = transform.Find("@close").GetComponent<Button>();
            retry = transform.Find("@retry").GetComponent<Button>();
            detail = transform.Find("@detail").GetComponent<Button>();
            clip_list = transform.Find("clip/@list").GetComponent<LSharpListSuper>();
        }
        void __DoInit()
        {
            clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            clip_list.OnShow();
        }
        void __DoHide()
        {
            clip_list.OnHide();
        }
//generate code end
        Action m_action;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordWrongItemPanel).Name);
            __DoInit();
            detail.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                GuiManager.Inst().ShowFrame((string)typeof(WordWrongFrame).Name);
            }));
            retry.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                FightSceneManager.Inst().PVERetry();
            });
            close.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
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
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void Display(Action action)
        {
            m_action = action;
        }
    }
}
