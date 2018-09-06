using UnityEngine;
using System.Collections;

namespace geniusbaby
{
    enum GunTargetType
    {
        Coord = 1,
        Fish = 2,
    }
    public class PetFightObj : IPetObj
    {
        public AudioClip Snd_Fire;
        cfg.weapon m_gunCfg;
        bool m_fireAtCoordOrFish;
        bool m_downUp;
        long gunCoolDownAt;
        int m_gunCd;
        Vector3 gunPos;
        Vector2 m_screenPos;
        int preparedSki;
        FEPet m_pet;
        public FEDefenderObj owner { get; private set; }
        public void Initialize(FEDefenderObj owner)
        {
            this.owner = owner;
            this.entityId = new EntityId(Obj3DType.Pet, owner.pdp.sessionId);
            this.gunCoolDownAt = 0;
            this.moduleId = -1;
            var mapCfg = tab.map.Inst().Find(owner.room.mapId);
            gunPos = FEScene2d.GetGunPos(mapCfg.res, owner.sessionId).V3();
            //GameManager.C2DWorldPosRectangleLocal(transform.position, out gunPos);
            _Change(owner.pet);
        }
        public void FireAt(Vector2 screenPos, bool downUp)
        {
            if (m_downUp = downUp)
            {
                m_screenPos = screenPos;
                if (owner.IsCasting())
                {
                    if (!owner.TrapCanProc()) { return; }
                    //Vector2 localPoint;
                    //if (FightSceneManager.Inst().ScreenToLocation(m_screenPos, out localPoint))
                    //{
                    //    TcpNetwork.Inst().Send(new pps.GSrvTrapProcReport() { coord = FEScene2d.Convert(localPoint.V2()) });
                    //}
                    RaycastHit hitInfo;
                    if(FightSceneManager.Inst().ScreenToWorld(m_screenPos, out hitInfo))
                    {
                        TcpNetwork.Inst().Send(new pps.GSrvTrapProcReport() { coord = FEScene2d.Convert(hitInfo.point.V3()) });
                    }
                }
                else
                {
                    GunUpdate();
                }
            }
        }
        public void _Change(FEPet gun)
        {
            m_pet = gun;
            Change(gun.data.com.mId);
            //if (gun.pet.mId == moduleId) { return; }
            //UnInitialize();
            //GameObjPool.Instance.DestroyObj(obj);
            //this.moduleId = gun.gunId;
            //m_gunCfg = tab.tower.Instance.Find(moduleId);
            //m_fireAtCoordOrFish = (m_gunCfg.effect.Length <= 0 || m_gunCfg.effect[0].type != 3);
            //obj = GameObjPool.Inst().CreateObjSync(GamePath.asset.gun.prefab, m_gunCfg.res);
            //Util.UnityHelper.Show(obj.transform, transform);
            //m_skinAnim.SetVisibleData(obj.transform, null, AnimationCullingType.AlwaysAnimate);
            var heroCfg = tab.hero.Inst().Find(moduleId);
            m_gunCfg = tab.weapon.Inst().Find(heroCfg.star.weapon);
            m_fireAtCoordOrFish = (m_gunCfg.effect.Length <= 0 || m_gunCfg.effect[0].type != 3);
            m_gunCd = m_pet.data.fight.fireCd;
        }
        public void NotifyFireAt(int tt, int fireAt, bool fly = false)
        {
            //m_skinAnim.Play(m_gunCfg.fireAnim, 1f);
            //SoundManager.Inst().PlaySound(Snd_Fire, true);

            var targetType = (GunTargetType)tt;
            Vector3 targetPos = Vector3.zero;
            switch (targetType)
            {
                case GunTargetType.Coord: targetPos = FEScene2d.Convert(fireAt).V3(); break;
                case GunTargetType.Fish:
                    var fishObj = SceneManager.Inst().GetAs<IBaseObj>(new EntityId(Obj3DType.Fish, fireAt));
                    //if (fishObj) { GameManager.C3DWorldPosRectangleLocal(fishObj.transform.position, out targetPos); }
                    if (fishObj) { targetPos = fishObj.transform.position; }
                    break;
            }
            targetPos.y = fly ? 2f : 0f;
            var dir = (targetPos - gunPos).normalized;
            //transform.up = dir;

            if (m_gunCfg.effect.Length > 0)
            {
                var effect = m_gunCfg.effect[0];
                switch (effect.type)
                {
                    case 1: BulletDirect(targetPos, dir, effect.param1, effect.param2); break;
                    case 2: BulletFly(dir, effect.param1, effect.param2); break;
                    case 3:
                        if (targetType == GunTargetType.Fish)
                        {
                            for (int index = 0; index < effect.param1; ++index)
                            {
                                var script = FightSceneManager.Inst().AddBullet<BulletFlyPurchase>(m_gunCfg.bullet, gunPos);
                                script.Initialize(owner.sessionId, new EntityId(Obj3DType.Fish, fireAt), m_pet.data.fight.bulletSpeed,  index * effect.param2 * 0.001f);
                            }
                        }
                        else
                        {
                            BulletFly(dir, effect.param1, effect.param2);
                        }
                        break;
                }
            }
            else
            {
                BulletFly(dir, 1, 0);
            }
        }
        public void PrepareSkill(int skiId)
        {
            preparedSki = skiId;
            m_skinAnim.Play("prepare", 1f);
        }
        public void CastSkill(int skiId)
        {
            m_skinAnim.Play("idle", 1f);
        }
        void BulletDirect(Vector3 targetPos, Vector3 dir, int bulletCount, int bulletInterval)
        {
            for (int index = 0; index < bulletCount; ++index)
            {
                var script = FightSceneManager.Inst().AddBullet<BulletDirectTo>(m_gunCfg.bullet, gunPos);
                script.Initialize(owner.sessionId, targetPos, m_pet.data.fight.bulletSpeed, index * bulletInterval * 0.001f);
                script.transform.up = dir;
            }
        }
        void BulletFly(Vector3 dir, int bulletCount, int bulletInterval)
        {
            for (int index = 0; index < bulletCount; ++index)
            {
                var script = FightSceneManager.Inst().AddBullet<BulletFlyLine>(m_gunCfg.bullet, gunPos);
                script.Initialize(owner.sessionId, dir, m_pet.data.fight.bulletSpeed, index * bulletInterval * 0.001f);
                script.transform.up = dir;
            }
        }
        public void GunUpdate()
        {
            if (owner.IsCasting() || owner.AnyExist(PlayerState.Seal)) { return; }

            if (m_downUp)
            {
                long curTimeMs = FightSceneManager.mod.timeMs;
                if (curTimeMs - gunCoolDownAt >= m_gunCd)
                {
                    gunCoolDownAt = curTimeMs;
                    if (m_fireAtCoordOrFish)
                    {
                        FireAt();
                    }
                    else
                    {
                        var fish = FightSceneManager.Inst().GetRayObj<FishObj>(m_screenPos, Util.TagLayers.Fish);
                        if (fish)
                        {
                            TcpNetwork.Inst().Send(
                                new pps.GSrvGunFireReport()
                                {
                                    castId = FightSceneManager.Me().sessionId,
                                    targetType = (int)GunTargetType.Fish,
                                    fireAt = fish.entityId.uniqueId
                                });
                        }
                        else
                        {
                            FireAt();
                        }
                    }
                }
            }
        }
        void FireAt()
        {
            //Vector2 localPoint;
            //if (FightSceneManager.Inst().ScreenToLocation(m_screenPos, out localPoint))
            //{
            //    if (preparedSki > 0)
            //    {
            //        TcpNetwork.Inst().Send(new pps.GSrvSkillCastReport() { skiId = preparedSki, coord = FEScene2d.Convert(localPoint.V2()) });
            //        preparedSki = 0;
            //    }
            //    else
            //    {
            //        TcpNetwork.Inst().Send(
            //            new pps.GSrvGunFireReport()
            //            {
            //                castId = FightSceneManager.Me().sessionId,
            //                targetType = (int)GunTargetType.Coord,
            //                fireAt = FEScene2d.Convert(localPoint.V2())
            //            });
            //    }
            //}
            RaycastHit hitInfo;
            if (FightSceneManager.Inst().ScreenToWorld(m_screenPos, out hitInfo))
            {
                if (preparedSki > 0)
                {
                    TcpNetwork.Inst().Send(new pps.GSrvSkillCastReport() { skiId = preparedSki, coord = FEScene2d.Convert(hitInfo.point.V3()) });
                    preparedSki = 0;
                }
                else
                {
                    TcpNetwork.Inst().Send(
                        new pps.GSrvGunFireReport()
                        {
                            castId = FightSceneManager.Me().sessionId,
                            targetType = (int)GunTargetType.Coord,
                            fireAt = FEScene2d.Convert(hitInfo.point.V3())
                        });
                }
            }
        }
        public bool isCoolDown()
        {
            return FightSceneManager.mod.timeMs >= gunCoolDownAt;
        }
    }
}