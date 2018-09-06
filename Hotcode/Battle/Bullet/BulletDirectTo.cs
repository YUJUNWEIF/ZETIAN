using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class BulletDirectTo : IBullet
    {
        float m_life;
        float m_time;
        bool m_hideStart;
        Vector3 m_start;
        Vector3 m_targetPos;
        public void Initialize(int casterId, Vector3 targetPos, float speed, float delay)
        {
            this.casterId = casterId;
            this.speed = speed;
            m_start = (Vector2)transform.position;
            m_targetPos = targetPos;
            m_life = (targetPos - m_start).magnitude / speed;
            m_time = -delay;
            m_hideStart = delay > 0;
            if (m_hideStart) { gameObject.SetActive(false); }
        }
        public override bool FrameUpdate(float deltaTimeMs)
        {
            var deltaTime = deltaTimeMs * 0.001f;
            m_time += deltaTime;
            if (m_time < 0)
            {
                if (m_time <= 0) { return true; }
            }

            if (m_hideStart) { m_hideStart = false; gameObject.SetActive(true); }
            if (m_time <= m_life)
            {                
                transform.position = Vector3.Lerp(m_start, m_targetPos, m_time / m_life);
                return true;
            }

            var fish = FightSceneManager.Inst().Raycast(transform.position);
            if (fish)
            {
                fish.Hit(casterId, transform.position);
                return false;
            }
            var fx = FXControl.Create(GamePath.asset.bullet.prefab, "wang");
            FightSceneManager.Inst().scene2d.Play2dFx(fx, m_targetPos, true);
            transform.position = m_targetPos;
            return false;
            //var screenPos = GameManager.C2DWorldToScreen(m_targetPos);
            //var ray = CameraControl.Inst().ScreenPointToRay(screenPos);
            //RaycastHit hitInfo;
            //if (Physics.Raycast(ray, out hitInfo, 300, 1 << Util.TagLayers.Fish))
            //{
            //    var fish = hitInfo.transform.GetComponentInParent<FishObj>();
            //    if (fish) { fish.Hit(casterId, hitInfo); }
            //}
            //else
            //{
            //    var fx = FXControl.Create(GamePath.asset.bullet.prefab, "wang");
            //    FightSceneManager.Inst().scene2d.Play2dFx(fx, m_targetPos, true);
            //}
            //return false;
            
        }
    }
}