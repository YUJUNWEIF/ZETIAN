using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class BulletFlyLine : IBullet
    {
        Vector3 m_dir;
        float m_time;
        bool m_hideStart;
        public void Initialize(int casterId, Vector3 dir, float speed, float delay)
        {
            this.casterId = casterId;
            this.speed = speed;
            m_dir = dir.normalized;
            m_time = -delay;
            m_hideStart = delay > 0;
            if (m_hideStart) { gameObject.SetActive(false); }
        }
        public override bool FrameUpdate(float deltaTimeMs)
        {
            var deltaTime = deltaTimeMs * 0.001f;
            if (m_time < 0)
            {
                m_time += deltaTime;
                if (m_time <= 0) { return true; }
                deltaTime = m_time;
            }

            if (m_hideStart) { m_hideStart = false; gameObject.SetActive(true); }

            var pos = transform.position + m_dir * speed * deltaTime;
            pos.y = 5f;
            transform.position = pos;
            //if (!FightSceneManager.Inst().InScreenView(pos)) { return false; }

            //var screenPos = GameManager.C2DWorldToScreen(transform.position);
            //var ray = CameraControl.Inst().ScreenPointToRay(screenPos);
            //RaycastHit hitInfo;
            //if (Physics.Raycast(ray, out hitInfo, 300, 1 << Util.TagLayers.Fish))
            //{
            //    var fish = hitInfo.transform.GetComponentInParent<FishObj>();
            //    if (fish)
            //    {
            //        fish.Hit(casterId, hitInfo);
            //        return false;
            //    }
            //}
            //return true;

            if (pos.x < -64 || pos.x > 64 || pos.z < -64 || pos.z > 64) { return false; }

            var fish = FightSceneManager.Inst().Raycast(pos);
            if (fish)
            {
                fish.Hit(casterId, pos);
                return false;
            }
            return true;
        }
    }
}