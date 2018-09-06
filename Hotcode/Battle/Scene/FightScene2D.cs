using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public interface IArea2d
    {
        RectTransform area { get; }
        IPlayer2DObj GetPlayerObj(int sessionId);
    }
    public class Scene2D
    {
        public IArea2d area2d;
        public IPlayer2DObj FindPlayer(int sessionId) { return (area2d != null) ? area2d.GetPlayerObj(sessionId) : null; }

        public bool InScreenView(Vector2 location)
        {
            var areaRc = area2d.area.rect;
            return location.x > areaRc.xMin && location.x < areaRc.xMax && location.y > areaRc.yMin && location.y < areaRc.yMax;
        }
        public bool ScreenToLocation(Vector2 screenPosition, out Vector2 localPoint)
        {
            var cam2d = Framework.Instance.camera2d;
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(area2d.area, screenPosition, cam2d, out localPoint);
        }
        public bool LocationToScreen(Vector2 localPoint, out Vector2 screenPosition)
        {
            var worldPos = area2d.area.localToWorldMatrix.MultiplyPoint3x4(localPoint);
            var cam2d = Framework.Instance.camera2d;
            screenPosition = cam2d.WorldToScreenPoint(worldPos);
            return true;
        }
        public T GetRayObj<T>(Vector2 screenPos, int mask) where T : IBaseObj
        {
            var ray = CameraControl.Inst().ScreenPointToRay(screenPos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 300, 1 << mask)) { return hitInfo.transform.GetComponentInParent<T>(); }
            return null;
        }
        public void Play3dFx(FXControl fx, Transform parent, Vector3 worldPos, bool autoDestroy)
        {
            if (autoDestroy) { fx.PlayAutoDestroy(parent, true, TimeType.SyncTime); }
            else
            {
                fx.Play(parent);
            }
            fx.transform.position = worldPos;
        }
        public void Play2dFx(FXControl fx, Vector3 worldPos, bool autoDestroy)
        {
            Vector2 localPos;
            Framework.C3DWorldPosRectangleLocal(worldPos, out localPos);
            Play2dFx(fx, localPos, autoDestroy);
        }
        public void Play2dFx(FXControl fx, Vector2 localPoint, bool autoDestroy)
        {
            if (autoDestroy) { fx.PlayAutoDestroy(area2d.area, true, TimeType.SyncTime); }
            else
            {
                fx.Play(area2d.area);
            }
            fx.transform.position = localPoint;
        }
        public void PlayChainFx(cfg.skill skiCfg, Vector2 center, IList<IFEMonster> targets)
        {
            for (int index = 0; index < targets.Count; ++index)
            {
                var obj = FightSceneManager.Inst().Find<IBaseObj>(Obj3DType.Fish, targets[index].UId());
                if (!obj) { continue; }

                Vector2 localPos;
                Framework.C3DWorldPosRectangleLocal(obj.transform.position, out localPos);
                var fx = FXControl.Create(GamePath.asset.fx3D, skiCfg.castFx, false);
                fx.duration = skiCfg.delayRmvMs * 0.001f;
                fx.PlayAutoDestroy(area2d.area, localPos, true);
                var scale = Vector3.one;
                var distance = (center - localPos);
                scale.y = distance.magnitude / 100f;
                fx.transform.localScale = scale;
                fx.transform.up = distance;
            }
        }        
    }
}