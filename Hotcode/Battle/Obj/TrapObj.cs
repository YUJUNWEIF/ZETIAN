using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public class TrapObj : IBaseObj
    {
        FETrap m_trap;
        FXControl m_keepFx;
        FXControl m_activeFx;
        public void Initialize(FETrap trap)
        {
            m_trap = trap;
            entityId = new EntityId(Obj3DType.Trap, trap.uniqueId);
            IFEBattle.onTrapMove.Add(OnTrapMove);
            IFEBattle.onTrapActive.Add(OnTrapActive);
            switch (m_trap.skillCfg.effect.type)
            {
                case EffectType.TrapLine:
                case EffectType.TrapLineMovale:
                case EffectType.TrapCircle:
                case EffectType.TrapCircleMovable:
                    if (!string.IsNullOrEmpty(m_trap.skillCfg.castFx))
                    {
                        m_keepFx = FXControl.Create(GamePath.asset.fx3D, m_trap.skillCfg.castFx, false);
                        m_keepFx.Play(transform);
                        m_keepFx.transform.localScale = Vector3.one * (m_trap.distance / 100f);
                        Util.UnityHelper.SetLayerRecursively(m_keepFx.transform, Util.TagLayers.Default);
                    }
                    break;
                case EffectType.TrapFan:
                    if (!string.IsNullOrEmpty(m_trap.skillCfg.castFx))
                    {
                        m_keepFx = FXControl.Create(GamePath.asset.fx3D, m_trap.skillCfg.castFx, false);
                        m_keepFx.Play(transform);
                        m_keepFx.transform.localScale = Vector3.one * (m_trap.distance / m_trap.skillCfg.effect.value1);
                        Util.UnityHelper.SetLayerRecursively(m_keepFx.transform, Util.TagLayers.Default);
                    }
                    break;
                case EffectType.Blomb: break;
            }
        }
        public override void UnInitialize()
        {
            //FXControl.Destroy(m_keepFx);
            if (m_keepFx) Util.UnityHelper.DestroyGameObjectNoUnInit(m_keepFx.gameObject);
            IFEBattle.onTrapMove.Rmv(OnTrapMove);
            IFEBattle.onTrapActive.Rmv(OnTrapActive);
            base.UnInitialize();
        }
        public void SetPos(FEVector3D targetPos)
        {
            PetFightObj caster = null;
            if (m_trap.ctrl)
            {
                caster = FightSceneManager.Inst().FindPlayer(m_trap.owner.sessionId);
            }
            //if (caster != null)
            //{
            //    var gunPos = FEScene2d.GetGunPos(FightSceneManager.Inst().loader.combatType, m_trap.owner.sessionId);
            //    var targetPos = FEScene2d.Convert(coord);
            //    caster.GetGun().transform.up = (targetPos - gunPos).normalized.V3();
            //}
            switch (m_trap.skillCfg.effect.type)
            {
                case EffectType.TrapLine:
                case EffectType.TrapLineMovale:
                case EffectType.TrapFan:
                    if (caster != null)
                    {
                        var mapCfg = tab.map.Inst().Find(m_trap.owner.room.mapId);
                        var gunPos = FEScene2d.GetGunPos(mapCfg.res, m_trap.owner.sessionId);
                        transform.position = gunPos.V3();
                        transform.up = (targetPos - gunPos).normalized.V3();
                    }
                    break;
                case EffectType.TrapCircle:
                case EffectType.TrapCircleMovable:
                case EffectType.Blomb:
                    transform.localPosition = targetPos.V3();
                    break;
            }
        }        
        public void Rmv()
        {
            if (m_trap.skillCfg.delayRmvMs > 100)
            {
                coroutineHelper.StartCoroutineImmediate(WaitSeconds.Delay(m_trap.skillCfg.delayRmvMs * 0.001f), RmvTrap);
            }
            else
            {
                RmvTrap();
            }
        }
        void RmvTrap()
        {
            FightSceneManager.Inst().RmvTrap(this);
        }
        void OnTrapActive(FETrap trap)
        {
            if (m_trap == trap)
            {
                switch (m_trap.skillCfg.effect.type)
                {
                    case EffectType.Blomb:
                        m_activeFx = FXControl.Create(GamePath.asset.fx3D, m_trap.skillCfg.castFx, false);
                        m_activeFx.PlayAutoDestroy(null, true);
                        m_activeFx.transform.position = transform.position;
                        m_activeFx.transform.localScale = Vector3.one * (m_trap.distance / 100f);
                        Util.UnityHelper.SetLayerRecursively(m_activeFx.transform, Util.TagLayers.UI);
                        break;
                }
            }
        }
        void OnTrapMove(FETrap target)
        {
            if (m_trap == target)
            {
                SetPos(target.targetAt);
            }
        }
    }
}