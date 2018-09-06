using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class EmotionFrame : ILSharpScript
    {
//generate code begin
        public Image BG_Rc_close;
        public LSharpListSuper BG_Rc_Clip_emotionPanel;
        void __LoadComponet(Transform transform)
        {
            BG_Rc_close = transform.Find("BG/Rc/@close").GetComponent<Image>();
            BG_Rc_Clip_emotionPanel = transform.Find("BG/Rc/Clip/@emotionPanel").GetComponent<LSharpListSuper>();
        }
        void __DoInit()
        {
            BG_Rc_Clip_emotionPanel.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Rc_Clip_emotionPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Rc_Clip_emotionPanel.OnShow();
        }
        void __DoHide()
        {
            BG_Rc_Clip_emotionPanel.OnHide();
        }
//generate code end
        public Action<string> onUseEmotion;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Rc_Clip_emotionPanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(EmotionItemPanel).Name);
            __DoInit();
            var emotionCmds = new List<object>(63);
            for (int index = 1; index <= 63; ++index)
            {
                emotionCmds.Add("E-" + index.ToString("D2"));
            }
            BG_Rc_Clip_emotionPanel.SetValues(emotionCmds);
            BG_Rc_Clip_emotionPanel.selectListener.Add(() =>
            {
                var no = BG_Rc_Clip_emotionPanel.itemSelected.First;
                if (no != null)
                {
                    GuiManager.Instance.HideFrame(api.name);
                    onUseEmotion((string)BG_Rc_Clip_emotionPanel.values[no.Value]);
                }
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
    }
}
