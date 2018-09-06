using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public abstract class FishObj : IGameObj, IBfGetter
    {
        public static FEQuaternion EulerReverse = FEQuaternion.Euler(0, 180, 0);
        GameObject shieldGo;
        FXControl m_fx;
        StringBuilder sb = new StringBuilder(256);
   
        BuffDisplayer m_displayer;
        FEFixed displayPos;
        protected Transform cachedTrans;
        protected cfg.objRes m_resCfg;
        protected GameObject obj;
        protected Transform model;
        protected Shader customDiffuse;
        protected Shader alphaDiffuse;

        public int UId() { return entityId.uniqueId; }
        public IFEMonster attachValue { get; private set; }

        Renderer render;
        protected Collider[] colls;
        void Awake()
        {
            m_displayer = new BuffDisplayer(transform, this);
            colls = GetComponentsInChildren<Collider>(true);
        }
        public bool ShouldProc()
        {
            return true;
        }
        public string Get(int skiId)
        {
            var fishSkiCfg = tab.monsterSkill.Inst().Find(skiId);
            //return fishSkiCfg != null ? fishSkiCfg.buff : null;
            return string.Empty;
        }
        public virtual void Initialize(IFEMonster fishObj)
        {
            this.cachedTrans = transform;
            this.attachValue = fishObj;
            transform.rotation = Quaternion.identity;

            IFEBattle.onFishUpdate.Add(OnFishUpdate);
            IFEBattle.onFishAttackMiss.Add(OnFishAttackMiss);
            this.attachValue.GetBuffs().listeners.Add(m_displayer);

            MonsterFight entity = fishObj.GetData();
            this.entityId = new EntityId(Obj3DType.Fish, entity.uniqueId);
            this.moduleId = entity.moduleId;

            var monsterCfg = tab.monster.Inst().Find(moduleId);
            m_resCfg = tab.objRes.Instance.Find(monsterCfg.resId);
            //obj = GameObjPool.Instance.CreateObjSync(GamePath.asset.fish.path + GamePath.asset.fish.prefab, m_resCfg.res);
            obj = gameObject;
            if (!string.IsNullOrEmpty(m_resCfg.tex))
            {
                if (sb.Length > 0) { sb.Remove(0, sb.Length); }
                var path = sb.Append(GamePath.asset.fish.model).Append('/');
                var tex = BundleManager.Inst().LoadSync<Texture>(path.ToString(), m_resCfg.tex);
                render = obj.GetComponentInChildren<SkinnedMeshRenderer>();
                if (render && render.material)
                {
                    render.enabled = true;
                    render.material.mainTexture = tex;
                }
            }
            //Util.UnityHelper.ShowAsChild(obj.transform, cachedTrans);
            obj.transform.localScale = Vector3.one;
            //Util.UnityHelper.SetLayerRecursively(obj.transform, Util.TagLayers.Fish);
            //string absolutePath = GamePath.asset.fish.anim + m_resCfg.res;
            //var clips = GameObjPool.Instance.LoadAnim(absolutePath);
            //if (clips == null)
            //{
            //    var tmp = Resources.LoadAll(absolutePath, typeof(AnimationClip));
            //    clips = System.Array.ConvertAll(tmp, it => it as AnimationClip);
            //    if (clips != null) { GameObjPool.Instance.AddClip(absolutePath, clips); }
            //}
            model = obj.transform.Find("model");
            if (model == null) { model = obj.transform; }
            //m_skinAnim.SetVisibleData(model ? model : obj.transform, clips);
            m_skinAnim.SetVisibleData(model, null);
            //if (!string.IsNullOrEmpty(entity.fishCfg.effectFx))
            //{
            //    m_fx = FXControl.Create(GamePath.asset.fx3D, entity.fishCfg.effectFx);
            //    m_fx.Play(transform);
            //}
            //m_skinAnim.Play(m_resCfg.swim, 1f);
            SummonShield(entity.fight.shield);

            displayPos = fishObj.moveAI.path.passedLength;
            transform.position = attachValue.moveAI.GetPosition().V3();
            transform.forward = attachValue.moveAI.GetForward().V3();
            _Update();

            var line = gameObject.GetComponent<LineRenderer>();
            if (!line)
            {
                line = gameObject.AddComponent<LineRenderer>();
            }
            
            var vs = new Vector3[RVOSysPath.test.Count];
            for (int index = 0; index < RVOSysPath.test.Count; ++index)
            {
                var v = RVOSysPath.test[index];
                vs[index] = new Vector3(v.x, 0, v.z) * PathCreator.CellSize;
            }
            line.positionCount = vs.Length;
            line.SetPositions(vs);
        }

        public override void UnInitialize()
        {
            IFEBattle.onFishUpdate.Rmv(OnFishUpdate);
            IFEBattle.onFishAttackMiss.Add(OnFishAttackMiss);
            this.attachValue.GetBuffs().listeners.Rmv(m_displayer);
            m_displayer.OnSync();
            if (m_fx != null)
            {
                FXControl.Destroy(m_fx);
                m_fx = null;
            }
            if(shieldGo)
            {
                GameObjPool.Inst().DestroyObj(shieldGo);
                shieldGo = null;
            }
            base.UnInitialize();
        }

        protected virtual void OnFishUpdate(IFEMonster now)
        {
            if (now.UId() != entityId.uniqueId) { return; }

            var data = now.GetData();
            SummonShield(data.fight.shield);
            bool sneak = data.AnyExist(FishState.Sneak);
            if (render)
            {
                render.enabled = !sneak;
            }
            for (int index = 0; index < colls.Length; ++index)
            {
                colls[index].enabled = !sneak && !data.dieRunaway;
            }
            if ((data.state & FishState.Dead) != 0)
            {
                //m_skinAnim.Play(m_resCfg.die, 1f);
                Recycle(1f);
            }
            else
            {
                bool freeze = data.AnyExist(FishState.Freeze);
                if (freeze) { m_skinAnim.FreezeAnimation(); }
                else { m_skinAnim.UnfreezeAnimation(); }
            }
            OnFishUpdate();
        }
        protected void OnFishAttackMiss(IFEMonster now)
        {
            if (now.UId() != entityId.uniqueId) { return; }
            var fx = FXControl.Create(GamePath.asset.fx3D, "miss");
            FightSceneManager.Inst().scene2d.Play2dFx(fx, transform.position, true);
        }
        public void DisplayReply(int sessionId, EnglishDisplayParam ep)
        {
            FightSceneManager.OnAlphabetFly(sessionId, transform, ep);
            if (ep.rightWrong)
            {
                DisplayKill(sessionId);
            }
        }
        public void DisplayKill(int sessionId)
        {
            FightSceneManager.OnCoinFly(sessionId, transform);
            
            var fishCfg = tab.monster.Inst().Find(moduleId);
            if (fishCfg.dieAudios.Length > 0)
            {
                var index = Framework.rand.Range(0, fishCfg.dieAudios.Length);
                var clip = GameObjPool.Inst().LoadAudioSync(GamePath.asset.audio, fishCfg.dieAudios[index]);
                SoundManager.Inst().PlaySound(clip);
            }
            if (!string.IsNullOrEmpty(fishCfg.dieFx.res))
            {
                var diefx = FXControl.Create(GamePath.asset.fx3D, fishCfg.dieFx.res, false);
                diefx.duration = fishCfg.dieFx.lifeMs * 0.001f;
                if (fishCfg.dieFx.fullscreen)
                {
                    FightSceneManager.Inst().scene2d.Play2dFx(diefx, transform.position, true);
                }
                else
                {
                    FightSceneManager.Inst().scene2d.Play3dFx(diefx, transform, transform.position, true);
                }
            }
            OnKilled(sessionId);
        }
        //public abstract void GetAttackedBox(int sessionId, FEBox box);
        public abstract void Hit(int sessionId, RaycastHit hitInfo);
        public abstract void Hit(int sessionId, Vector3 hitPos);


        protected virtual void OnKilled(int sessionId) { }
        protected virtual void OnFishUpdate() { }

        public void Recycle(float delaySec = 0f)
        {
            coroutineHelper.StopAll();
            if (delaySec < 0.1f)
            {
                SceneManager.Inst().RmvObj(entityId);
            }
            else
            {
                coroutineHelper.StartCoroutineImmediate(WaitSeconds.Delay(() => SceneManager.Inst().RmvObj(entityId), delaySec));
            }
        }
        public void OnLockStepUpdate(int deltaMs)
        {
        }
        void SummonShield(int shield)
        {
            if (shield > 0)
            {
                if (!shieldGo)
                {
                    shieldGo = GameObjPool.Inst().CreateObjSync(GamePath.asset.bullet.prefab, "hudun");
                    Util.UnityHelper.SetLayerRecursively(shieldGo.transform, Util.TagLayers.Fish);
                    Util.UnityHelper.Show(shieldGo.transform, transform);
                    shieldGo.transform.localScale = Vector3.one * m_resCfg.radius;
                }
                shieldGo.SetActive(true);
            }
            else if (shieldGo)
            {
                shieldGo.SetActive(false);
            }
        }
        float deltaMs;
        public void OnDisplayUpdate(float deltaMs)
        {
            //if (!attachValue.DieRunaway() && attachValue.speed > 0)
            {
                displayPos += (this.deltaMs = deltaMs);
                _Update();
            }
        }
        void _Update()
        {
            if (!attachValue.DieRunaway())
            {
                if (FightSceneManager.mod.state != FightState.Sweeping)
                {
                    var p = transform.position;
                    FEVector2D worldPos = new FEVector2D(p.x, p.z);
                    FEVector2D look = FEVector2D.zero;
                    attachValue.moveAI.path.GetTrans(displayPos, deltaMs, ref worldPos, ref look);
                    Land(new Vector3(worldPos.x, 0f, worldPos.y));
                    var now = new Vector3(look.x, 0f, look.y) * (attachValue.ahead ? 1f : -1f);
                    now.Normalize();
                    var old = transform.forward;
                    old.y = 0f;
                    old.Normalize();

                    const float rotateSpeed = 360;
                    float param = rotateSpeed * deltaMs * 0.001f * Mathf.Deg2Rad;
                    Vector3 moveDir = Vector3.Lerp(old, now, param).normalized;                    
                    transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                    var pos = transform.position;
                    pos.x += attachValue.speed * 0.01f * Time.deltaTime * 5;
                    Land(pos);
                }
            }
        }

        void Land(Vector3 pos)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Vector3(pos.x, 20f, pos.z), Vector3.down, out hitInfo, 40f, 1 << Util.TagLayers.Terrain))
            {
                transform.position = hitInfo.point;
            }
            else
            {
                transform.position = pos;
            }
        }
        public void Ski2Fish(cfg.monsterSkill fishSkiCfg, IList<IFEMonster> targets)
        {
            DisplayCastSkill(fishSkiCfg);
            for (int index = 0; index < targets.Count; ++index)
            {
                switch (fishSkiCfg.target.type)
                {
                    case cfg.monsterSkill.TargetSelf:
                    case cfg.monsterSkill.TargetScopeExceptSelf:
                    case cfg.monsterSkill.TargetScopeAll:
                        var targetObj = FightSceneManager.Inst().Find<FishObj>(Obj3DType.Fish, targets[index].UId());
                        if (targetObj)
                        {
                            targetObj.DisplaySkillAttacked(fishSkiCfg);
                        }
                        break;
                }
            }
        }
        public void Ski2Player(cfg.monsterSkill skiCfg, IList<CombatPlayer<IFEBattle>> targets)
        {

        }
        void DisplayCastSkill(cfg.monsterSkill fishSkiCfg)
        {
            if (!string.IsNullOrEmpty(fishSkiCfg.castFx.res))
            {
                var castFx = FXControl.Create(GamePath.asset.fx3D, fishSkiCfg.castFx.res, false);
                castFx.duration = fishSkiCfg.castFx.lifeMs * 0.001f;
                if (fishSkiCfg.castFx.fullscreen)
                {
                    FightSceneManager.Inst().scene2d.Play2dFx(castFx, transform.position, true);
                }
                else
                {
                    FightSceneManager.Inst().scene2d.Play3dFx(castFx, null, transform.position, true);
                }
            }
        }
        void DisplaySkillAttacked(cfg.monsterSkill fishSkiCfg)
        {
            if (!string.IsNullOrEmpty(fishSkiCfg.hitFx.res))
            {
                var hitfx = FXControl.Create(GamePath.asset.fx3D, fishSkiCfg.hitFx.res, false);
                hitfx.duration = fishSkiCfg.hitFx.lifeMs * 0.001f;
                if (fishSkiCfg.hitFx.fullscreen)
                {
                    FightSceneManager.Inst().scene2d.Play2dFx(hitfx, transform.position, true);
                }
                else
                {
                    FightSceneManager.Inst().scene2d.Play3dFx(hitfx, transform, transform.position, true);
                }
            }
        }
    }
}