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
    public class DragStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public string frame;
            public string src;
            public string dst;
        }
        RectTransform clickTrans;
        RectTransform dstTrans;
        Param param;
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            param = (Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
        }
        public override void Process()
        {
            clickTrans.GetComponent<DragGridCell>().onSwap.Fire(dstTrans.GetComponent<DragGridCell>());
        }

        public override void Enter()
        {
            base.Enter();
            var script = GuiManager.Instance.GetCachedFrame(param.frame);
            clickTrans = (RectTransform)script.transform.Find(param.src);
            dstTrans = (RectTransform)script.transform.Find(param.dst);
            frame.highlight.draggle = true;

            Util.UnityHelper.Show(frame.highlight);
            Util.UnityHelper.Show(frame.dst);

            frame.Dialog(this);
            Update();
            frame.PlayArrowTip(direction);
        }
        public override void Update()
        {
            SetUI(frame.highlight, clickTrans);
            SetUI(frame.dst, dstTrans);

            var click = DisplayUIMask(clickTrans);
            var dst = DisplayUIMask(dstTrans);

            frame.mask.material.SetInt(@"_ShapeType", 1);
            frame.mask.material.SetVector(@"_ShapePos", new Vector4(click.x, click.y, dst.x, dst.y));
            frame.mask.material.SetVector(@"_ShapeSize", new Vector4(click.z, click.w, dst.z, dst.w));

            var rc = (RectTransform)frame.highlight.transform;
            rc.sizeDelta = new Vector2(click.z * frame.mask.rectTransform.sizeDelta.x, click.w * frame.mask.rectTransform.sizeDelta.y) * 2;

        }
    }
}