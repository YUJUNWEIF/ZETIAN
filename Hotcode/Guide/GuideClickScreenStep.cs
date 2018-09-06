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
    public class ClickScreenStep : GuideStep
    {
        class Param
        {
            public int type;
            public int x;
            public int y;
        }
        RectTransform clickTrans;
        Param param;
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            param =(Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
        }

        public override void Enter()
        {
            base.Enter();
            var script = GuiManager.Instance.GetCachedFrame(typeof(FightSingleFrame).Name);
            clickTrans = (RectTransform)script.transform;
            frame.highlight.clickable = true;
            Util.UnityHelper.Show(frame.highlight);

            Vector2 localPoint;
            FightSceneManager.Inst().ScreenToLocation(new Vector2(param.x, param.y), out localPoint);
            frame.highlight.transform.localPosition = localPoint;

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
        Vector2 m_pressAt;
        public override void OnClickHighLight(PointerEventData eventData)
        {
            m_pressAt = eventData.position;
            Leave();
        }
        public override void Process()
        {
            var script = GuiManager.Instance.GetCachedFrame(typeof(FightSingleFrame).Name);
            //script.FireAt(script.lastPress.position);
            //script.FireAt(m_pressAt);
            //GuideMaskFrame.GetClickHandler(clickTrans.gameObject).OnPointerClick(m_temp);
        }
    }    
}