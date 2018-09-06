using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace geniusbaby.LSharpScript
{
    public class IGuideUIStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public string frame;
            public string[] clickers;
        }
        protected RectTransform clickTrans;
        protected Param param;
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            param = (Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
        }
        public override void Enter()
        {
            base.Enter();
            var script = GuiManager.Instance.GetCachedFrame(param.frame);
            if (param.clickers.Length > 0)
            {
                clickTrans = (RectTransform)script.transform.Find(param.clickers[0]);
            }
            else
            {
                clickTrans = (RectTransform)script.transform;
            }
            Util.UnityHelper.Show(frame.highlight);

            frame.Dialog(this);
            Update();
            frame.PlayArrowTip(direction);
        }
        public override void Update()
        {
            SetUI(frame.highlight, clickTrans);

            var rect = DisplayUIMask(clickTrans);
            frame.mask.material.SetInt(@"_ShapeType", 1);
            frame.mask.material.SetVector(@"_ShapePos", new Vector4(rect.x, rect.y));
            frame.mask.material.SetVector(@"_ShapeSize", new Vector4(rect.z, rect.w));

            var rc = (RectTransform)frame.highlight.transform;
            rc.sizeDelta = new Vector2(rect.z * frame.mask.rectTransform.sizeDelta.x, rect.w * frame.mask.rectTransform.sizeDelta.y) * 2;

        }
        public override void Process()
        {
            GuideMaskFrame.GetClickHandler(clickTrans.gameObject).OnPointerClick(tped);
        }
    }
    public class ClickUIStep : IGuideUIStep
    {
        public override void Enter()
        {
            base.Enter();
            frame.highlight.clickable = true;
        }
        public override void Process()
        {
            GuideMaskFrame.GetClickHandler(clickTrans.gameObject).OnPointerClick(tped);
        }
    }
    public class TipUIStep : IGuideUIStep
    {
        public override void Enter()
        {
            base.Enter();
            frame.highlight.clickable = true;
            //frame.buttonAll.interactable = true;
        }
        public override void Process()
        {
        }
    }
}