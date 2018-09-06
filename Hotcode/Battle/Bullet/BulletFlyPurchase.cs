using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Util;

namespace geniusbaby
{
    public class BulletFlyPurchase : IBullet
    {
        EntityId targetId;
        Vector3 m_dir;
        bool m_valid;
        float m_time;
        bool m_hideStart;
        public void Initialize(int casterId, EntityId targetId, float speed, float delay)
        {
            this.casterId = casterId;
            this.speed = speed;
            m_time = -delay;
            m_hideStart = delay > 0;
            if (m_hideStart) { gameObject.SetActive(false); }
            m_valid = false;
            var target = SceneManager.Instance.GetAs<IBaseObj>(targetId);
            if (target)
            {
                this.targetId = targetId;
                m_dir = (target.transform.position - transform.position);
                m_dir.y = 0f;
                m_dir.Normalize();
                transform.up = m_dir;
            }
        }
        public override bool FrameUpdate(float deltaTimeMs)
        {            
            if (!m_valid) { return false; }

            var deltaTime = deltaTimeMs * 0.001f;
            if (m_time < 0)
            {
                m_time += deltaTime;
                if (m_time <= 0) { return true; }
                deltaTime = m_time;
            }

            if (m_hideStart) { m_hideStart = false; gameObject.SetActive(true); }

            var target = SceneManager.Instance.GetAs<IBaseObj>(targetId);
            if (target)
            {
                m_dir = (target.transform.position - transform.position);
                m_dir.y = 0f;
                m_dir.Normalize();
                transform.up = m_dir;
            }
            var pos = transform.position + m_dir * speed * deltaTime;
            transform.position = pos;

            if (pos.x < -64 || pos.x > 64 || pos.z < -64 || pos.z > 64) { return false; }

            var fish = FightSceneManager.Inst().Raycast(pos);
            if (fish && fish == target)
            {
                fish.Hit(casterId, pos);
                return false;
            }
            return true;
        }
    }
}