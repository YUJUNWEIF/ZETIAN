//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.IO;

//namespace geniusbaby.LSharpScript
//{
//    public class Obj3DStep : GuideStep
//    {
//        protected class Param
//        {
//            public int type;
//            public int fishId;
//        }
//        public EntityId entityId { get; private set; }
//        IBaseObj m_fishId;
//        public override void Initialize(Guide guide)
//        {
//            base.Initialize(guide);
//            var param = (Param)geniusbaby.tab.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content);
//            entityId = new EntityId(Obj3DType.Fish, param.fishId);
//        }
//        public override void Process()
//        {
//            m_fishId.GetComponent<EventTrigger>().OnPointerClick(m_temp);
//        }
//        public override void Enter()
//        {
//            base.Enter();
//            m_fishId = SceneManager.Inst().GetAs<IBaseObj>(entityId);
//            var resetNode = SceneManager.Instance.Terrain<TerrainDesign>().GetObj3DResetLocation();
//            CameraControl.Inst().PlayTweener(resetNode, 0.5f);
//            frame.highlightCell.clickable = true;
//            Util.UnityHelper.Show(frame.highlightCell);

//            frame.Dialog(this);
//            Update();
//            frame.PlayArrowTip(direction);            
//        }
//        public override void Leave()
//        {
//            m_fishId = SceneManager.Instance.GetAs<IBaseObj>(entityId);
//            if (m_fishId)
//            {
//                var backNode = SceneManager.Instance.Terrain<TerrainDesign>().GetObj3DBackLocation();
//                CameraControl.Inst().Reset(backNode);
//            }
//        }
//        public override void Update()
//        {
//            float radius = 160f;
//            Set3D(frame.highlightCell, m_fishId.transform);

//            var rect = Display3DMask(m_fishId.transform, radius);
//            frame.maskImage.material.SetInt(@"_ShapeType", 2);
//            frame.maskImage.material.SetVector(@"_ShapePos", new Vector4(rect.x, rect.y));
//            frame.maskImage.material.SetVector(@"_ShapeSize", new Vector4(rect.z, rect.w));
//            var rc = (RectTransform)frame.highlightCell.transform;
//            rc.sizeDelta = Vector2.one * radius * 2f;
//        }
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using geniusbaby;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class Obj3DStep : GuideStep
    {
        protected class Param
        {
            public int type;
            public int fishId;
        }
        public EntityId entityId { get; private set; }
        IBaseObj m_fish;
        public override void Initialize(Guide guide)
        {
            base.Initialize(guide);
            int column = 0;
            var param = (Param)config.UserDefineFormatParser.ConvertToType(typeof(Param), stepCfg.content.Split('|'), ref column);
            entityId = new EntityId(Obj3DType.Fish, param.fishId);
        }
        public override void Process()
        {
            TcpNetwork.Inst().Send(
                new GSrvGunFireReport()
                {
                    castId = FightSceneManager.Me().sessionId,
                    targetType = (int)GunTargetType.Fish,
                    fireAt = entityId.uniqueId
                });
        }
        public override void Enter()
        {
            base.Enter();
            m_fish = FightSceneManager.Inst().Find<FishObj>(entityId);
            frame.highlight.clickable = true;
            Util.UnityHelper.Show(frame.highlight);

            frame.Dialog(this);
            Update();
            frame.PlayArrowTip(direction);
        }
        public override void Update()
        {
            float radius = 160f;
            Set3D(frame.highlight, m_fish.transform);

            var rect = Display3DMask(m_fish.transform, radius);
            frame.mask.material.SetInt(@"_ShapeType", 2);
            frame.mask.material.SetVector(@"_ShapePos", new Vector4(rect.x, rect.y));
            frame.mask.material.SetVector(@"_ShapeSize", new Vector4(rect.z, rect.w));
            var rc = (RectTransform)frame.highlight.transform;
            rc.sizeDelta = Vector2.one * radius * 2f;
        }
    }
}