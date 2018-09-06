using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MonsterStatisticFrame : ILSharpScript
    {
//generate code begin
        public Button BG_close;
        public LSharpListSuper BG_Clip_list;
        void __LoadComponet(Transform transform)
        {
            BG_close = transform.Find("BG/@close").GetComponent<Button>();
            BG_Clip_list = transform.Find("BG/Clip/@list").GetComponent<LSharpListSuper>();
        }
        void __DoInit()
        {
            BG_Clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Clip_list.OnShow();
        }
        void __DoHide()
        {
            BG_Clip_list.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(MonsterStatisticItemPanel).Name);
            __DoInit();
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
            var fishes = FightSceneManager.Inst().statistic.fishes;
            var monsters = new List<object>(fishes.Count);
            for (int index = 0; index < fishes.Count; ++index) { monsters.Add(fishes[index]); }
            BG_Clip_list.SetValues(monsters);
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
    }
}
