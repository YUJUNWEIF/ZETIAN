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
    public class TeachCastXpStep : GuideStep
    {
        public RectTransform clickTrans;
        public override void OnPressHighLight(PointerEventData ped)
        {
            //var script = GuiManager.Instance.GetCachedFrame<FightFrame>();
            //if (script) { script.WantCastXp(ped.position); }
        }
        public override void OnDragHighLight(PointerEventData ped)
        {
            //var script = GuiManager.Instance.GetCachedFrame<FightFrame>();
            //if (script) { script.PlaceFxAtTerrain(ped.position); }
        }
        public override void OnDropHighLight(PointerEventData ped)
        {
            //var script = GuiManager.Instance.GetCachedFrame<FightFrame>();
            //if (script && script.CastXpAtTerrain(ped.position))
            //{
            //    GuiManager.Instance.HideFrame(frame);
            //    DialogFrame.NotifyWhenFinished(dialogAfter, ()=>
            //    {
            //        Process();
            //        frame.GotoNext();
            //    });
            //}
        }
        public override void Process()
        {
            //FightSceneManager.Instance.pause = false;
        }
        public override void Enter()
        {
            base.Enter();
            //FightSceneManager.Instance.pause = true;

            frame.highlight.clickable = true;
            //var rc = frame.line2d.uvOffset;
            //rc.width = 12f;
            //frame.line2d.uvOffset = rc;
            Util.UnityHelper.Show(frame.highlight);
            Util.UnityHelper.Show(frame.line);

            frame.Dialog(this);
            Update();
            frame.PlayArrowTip(GuideDirection.Unknown);


        }
        public override void Update()
        {
            //float radius = 160f;
            //Set3D(frame.highlightCell, building.transform);

            //var rect = Display3DMask(building.transform, radius);

            //frame.maskImage.material.SetInt(@"_ShapeType", 2);
            //frame.maskImage.material.SetVector(@"_ShapePos", new Vector4(rect.x, rect.y));
            //frame.maskImage.material.SetVector(@"_ShapeSize", new Vector4(rect.z, rect.w));

            //var rc = (RectTransform)frame.highlightCell.transform;
            //rc.sizeDelta = Vector2.one * radius * 2f;
        }
    }
}