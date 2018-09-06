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
    public enum DialogType
    {
        Func_Building = 1,
        PVE = 2,
        Func_Button = 3,
        Guide = 4,
    }
    public enum GuideDirection
    {
        Unknown,
        Left,
        Right,
        Up,
        Down,
    }
    public enum PVEDialog
    {
        BeforeCombat = 1,
        AfterCombat = 2,
    }
    public abstract class IGuideStep
    {
        public Guide guide;
        public List<IGuideStep> restoreURL = new List<IGuideStep>();
    }

    public abstract class GuideStep : IGuideStep
    {
        public GuideDirection direction;
        //public GuideDirection dlgPos;
        public geniusbaby.cfg.guide._sub stepCfg;
        public GuideMaskFrame  frame;
        public static PointerEventData tped = new PointerEventData(null);

        public virtual void Initialize(Guide guide)
        {
            this.guide = guide;
            stepCfg = guide.guideCfg.subs[guide.stepId];
            direction = (GuideDirection)stepCfg.arrow;
            //if (!string.IsNullOrEmpty(guideCfg.resumeUrl))
            //{
            //    var stepsStr = guideCfg.resumeUrl.Split(',');
            //    for (int index = 0; index < stepsStr.Length; ++index)
            //    {
            //        var stepSplitStr = stepsStr[index].Split(',');
            //        try
            //        {
            //            var step = new ClickUIStep();
            //            step.frameName = stepSplitStr[0];
            //            for (int btnIndex = 1; btnIndex < stepSplitStr.Length; ++btnIndex)
            //            {
            //                step.buttonName.Add(stepSplitStr[index]);
            //            }
            //            restoreURL.Add(step);
            //        }
            //        catch (Exception) { }
            //    }
            //}
        }
        string tdm { get { return string.Format("G{0}:{1}", guide.guideCfg.ToString(), guide.stepId.ToString()); } }
        public virtual void Enter()
        {
            //TDGAMission.OnBegin(tdm);
            if (stepCfg.autoType == geniusbaby.cfg.guide.StepAutoNext) { Leave(); }
        }
        public virtual void Leave()
        {
            DialogFrame.NotifyWhenFinished(stepCfg.dialogAfter, () =>
            {
                Process();
                //TDGAMission.OnCompleted(tdm);
                frame.GotoNext();
            });
        }
        public abstract void Process();
        public abstract void Update();
        //public virtual void OnClickAll() { Leave(); }
        public virtual void OnClickHighLight(PointerEventData ped) { Leave(); }
        public virtual void OnSwapHighLight() { Leave(); }
        public virtual void OnPressHighLight(PointerEventData ped) { }
        public virtual void OnDragHighLight(PointerEventData ped) { }
        public virtual void OnDropHighLight(PointerEventData ped) { }
        Bounds GetMaskRootBounds()
        {
            var viewRect = frame.mask.rectTransform;
            var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            return viewBounds;
        }
        protected void SetUI(DragGridCell obj, Transform trans)
        {
            var uiRc = (RectTransform)trans;
            var offset = (Vector2.one * 0.5f - uiRc.pivot);
            var loc = trans.position;
            loc.z = obj.transform.position.z;
            loc.x += uiRc.sizeDelta.x * offset.x;
            loc.y += uiRc.sizeDelta.y * offset.y;
            obj.transform.position = loc;
        }
        protected void Set3D(DragGridCell obj, Transform trans)
        {
            Set3D(obj, trans.position);
        }
        protected void Set3D(DragGridCell obj, Vector3 position)
        {
            Vector2 loc;
            Framework.C3DWorldPosRectangleLocal(position, out loc);
            var tmp = obj.transform.position;
            tmp.x = loc.x;
            tmp.y = loc.y;
            obj.transform.position = tmp;
        }
        protected Vector4 DisplayUIMask(RectTransform uiRc)
        {
            var m_maskBounds = GetMaskRootBounds();

            Vector4 rect = Vector4.one;
            var targetBounds = Util.UnityHelper.GetTargetBounds(uiRc, frame.api.transform);
            rect.x = targetBounds.center.x / m_maskBounds.size.x + 0.5f;
            rect.y = targetBounds.center.y / m_maskBounds.size.y + 0.5f;
            rect.z = (targetBounds.size.x / m_maskBounds.size.x) * 0.5f;
            rect.w = (targetBounds.size.y / m_maskBounds.size.y) * 0.5f;
            return rect;
        }
        protected Vector4 Display3DMask(Transform target, float radius)
        {
            return Display3DMask(target.position, radius);
        }
        protected Vector4 Display3DMask(Vector3 position, float radius)
        {
            var m_maskBounds = GetMaskRootBounds();

            Vector4 rect = Vector4.one;
            Vector2 loc;
            Framework.C3DWorldPosRectangleLocal(position, out loc, frame.mask.rectTransform as RectTransform);
            rect.x = loc.x / m_maskBounds.size.x + 0.5f;
            rect.y = loc.y / m_maskBounds.size.y + 0.5f;
            rect.z = (1f / m_maskBounds.size.x) * radius;
            rect.w = (1f / m_maskBounds.size.y) * radius;
            return rect;
        }
    }
}