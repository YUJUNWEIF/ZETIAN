using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class AchieveFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper clip_list;
        public Button close;
        void __LoadComponet(Transform transform)
        {
            clip_list = transform.Find("clip/@list").GetComponent<LSharpListSuper>();
            close = transform.Find("@close").GetComponent<Button>();
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
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(AchieveItemPanel).Name);
            __DoInit();
            close.onClick.AddListener(() => GuiManager.Inst().HideFrame(api.name));
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
            AchieveModule.Inst().onAchieveSync.Add(OnSync);
            OnSync();
        }
        public override void OnHide()
        {
            AchieveModule.Inst().onAchieveSync.Rmv(OnSync);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            clip_list.SetValues(AchieveModule.Inst().achieves.ConvertAll(it => (object)it));
        }
    }
}
