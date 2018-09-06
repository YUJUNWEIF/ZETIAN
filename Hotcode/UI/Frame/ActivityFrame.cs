using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ActivityFrame : ILSharpScript
    {
//generate code begin
        public LSharpAPI BG_Rc_quizJoinPanel;
        public LSharpListSuper BG_Lc_clip_typePanel;
        public GuiBar BG_Lc_bar;
        void __LoadComponet(Transform transform)
        {
            BG_Rc_quizJoinPanel = transform.Find("BG/Rc/@quizJoinPanel").GetComponent<LSharpAPI>();
            BG_Lc_clip_typePanel = transform.Find("BG/Lc/clip/@typePanel").GetComponent<LSharpListSuper>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<GuiBar>();
        }
        void __DoInit()
        {
            BG_Rc_quizJoinPanel.OnInitialize("geniusbaby.LSharpScript.QuizJoinPanel");
            BG_Lc_clip_typePanel.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Rc_quizJoinPanel.OnUnInitialize();
            BG_Lc_clip_typePanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Rc_quizJoinPanel.OnShow();
            BG_Lc_clip_typePanel.OnShow();
        }
        void __DoHide()
        {
            BG_Rc_quizJoinPanel.OnHide();
            BG_Lc_clip_typePanel.OnHide();
        }
//generate code end
        BehaviorWrapper script;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_clip_typePanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(ActivityTypeItemPanel).Name);
            __DoInit();
            BG_Lc_clip_typePanel.selectListener.Add(() =>
            {
                var no = BG_Lc_clip_typePanel.itemSelected.First;
                if (no != null)
                {
                    if (script) { Util.UnityHelper.Hide(script); }
                    switch ((int)BG_Lc_clip_typePanel.values[no.Value])
                    {
                        case cfg.CodeDefine.Activity_Quiz: Util.UnityHelper.Show(script = BG_Rc_quizJoinPanel); break;
                        default: break;
                    }
                }
            });
            
            var types = new List<object>() { cfg.CodeDefine.Activity_Quiz };
            BG_Lc_clip_typePanel.SetValues(types);
            BG_Lc_clip_typePanel.SelectIndex(0);
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
    }
}
