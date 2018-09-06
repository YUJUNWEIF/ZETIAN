using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public struct EnglishDisplayParam
    {
        public IFEMonster fish;
        public bool rightWrong;
        public int replyIndex;
        public EnglishDisplayParam(IFEMonster fish, bool rightWrong, int replyIndex) : this()
        {
            this.fish = fish;
            this.rightWrong = rightWrong;
            this.replyIndex = replyIndex;
        }
    }
    public interface IPvpManager
    {
        void PvpEnter(pps.SrvFight addr, object report, Action networkNotReachable);
        void PvpLeave();
        void OnNosession();
        void OnAbnormal();
    }
    public interface IPvpListener
    {
        void Run();
        void OnDisplayUpdate();
        void Nosession();
    }


    public class FightSceneManager : PvpManager<FightSceneManager>, IGameEvent, ISceneManager, ICastSkillListener
    {
        public int classId;
        public List<pps.ProtoKnowledge> knowledges;
        public Action afterFight;

        public int roomId;
        public int mapId;
        public List<pps.ProtoDetailPlayer> players;
        public Dictionary<string, int> progress = new Dictionary<string, int>();
        bool m_loading;
        Refresher m_loadingRefresher = new Refresher();

        TerrainDesign m_td = new TerrainDesign();
        List<PetFightObj> m_players = new List<PetFightObj>();
        List<FishObj> m_fishes = new List<FishObj>();
        List<IBullet> m_bullets = new List<IBullet>();
        List<TrapObj> m_traps = new List<TrapObj>();
        EntityId m_targetFish;
        IFEState m_sweeper;
        Util.CoroutineHelper m_cor = new Util.CoroutineHelper();

        public Scene2D scene2d = new Scene2D();
        public Statistic statistic = new Statistic();

        public Util.Param1Actions<EntityId> onTargetSelect = new Util.Param1Actions<EntityId>();
        public Util.Param1Actions<int> onResult = new Util.Param1Actions<int>();

        public static IFEBattle mod { get; private set; }
        public static Action<int, Transform> OnCoinFly;
        public static Action<int, Transform, EnglishDisplayParam> OnAlphabetFly;


        public EntityId target
        {
            get { return m_targetFish; }
            set { onTargetSelect.Fire(m_targetFish = value); }
        }
        public void OnStartGame()
        {
            IFEBattle.onCastSkill = this;
            IFEBattle.onPlayerSync.Add(OnSyncPlayer);
        }
        public void OnStopGame()
        {
            IFEBattle.onPlayerSync.Rmv(OnSyncPlayer);
        }
        class Sweep : IFEState
        {
            long m_sweepTimeMs;
            public Sweep(int sweepTimeMs) { m_sweepTimeMs = sweepTimeMs; }
            public void Enter() { }
            public void Leave()
            {
                FightSceneManager.Inst().ClearObjs(FightSceneManager.Inst().m_fishes);
            }
            public void Update(int deltaMs)
            {
                if (m_sweepTimeMs <= 0) { return; }
                if ((m_sweepTimeMs -= FightSceneManager.mod.deltaMs) <= 0) { Leave(); }
            }
        }
        public void Initialize(IArea2d area2d) { scene2d.area2d = area2d; }
        public void UnInitialize() { scene2d.area2d = null; }
        public bool InScreenView(Vector2 screenPos) { return scene2d.InScreenView(screenPos); }
        public bool ScreenToLocation(Vector2 screenPos, out Vector2 localPoint) { return scene2d.ScreenToLocation(screenPos, out localPoint); }
        public bool LocationToScreen(Vector2 localPoint, out Vector2 screenPos) { return scene2d.LocationToScreen(localPoint, out screenPos); }

        public bool ScreenToWorld(Vector2 screenPos, out RaycastHit hitInfo)
        {
            var ray = Framework.Instance.camera3d.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out hitInfo, 500, (1 << Util.TagLayers.Fish) | (1 << Util.TagLayers.Terrain));
        }

        public T GetRayObj<T>(Vector2 screenPos, int mask) where T : IBaseObj { return scene2d.GetRayObj<T>(screenPos, mask); }

        public void LoadStart(int roomId, int combatType, int mapId, List<pps.ProtoDetailPlayer> players)
        {
            this.roomId = roomId;
            this.m_combatType = combatType;
            this.mapId = mapId;
            this.players = players;
            this.m_loading = true;
            m_loadingRefresher.Reset(1000, 0);
        }
        public void LoadFinish(int randSeed)
        {
            this.m_loading = false;
            switch (m_combatType)
            {
                case GlobalDefine.GTypeFightSingle: mod = new FEBattlePVESingle(); break;
                case GlobalDefine.GTypeFightMulti_1S1: mod = new FEBattlePvpMulti(false); break;
            }
            FightSceneManager.mod = mod;
            mod.Initialize(roomId);
            FEModule.Inst().AddBattle(mod);
            var gplayers = new List<CombatPlayer<IFEBattle>>();
            for (int index = 0; index < players.Count; ++index)
            {
                gplayers.Add(new FEDefenderObj(mod, players[index], false));
            }
            mod.Enter(m_combatType, mapId, randSeed, gplayers);
            mod.StartFight();
        }
        public override void OnNosession()
        {
            if (combatState == CombatState.Loading || combatState == CombatState.Fighting)
            {
                PvpLeave();
                MainState.Instance.PopState();
            }
        }
        protected override void OnLoop()
        {
            if (m_loading)
            {
                if (m_loadingRefresher.Update(Mathf.RoundToInt(Time.deltaTime * 1000)))
                {
                    TcpNetwork.Inst().Send(new pps.FightMapLoadProgressReport() { value = SceneManager.Inst().LoadingProgress() });
                }
            }
        }
        protected override void OnFrameUpdate()
        {
            UnityEngine.Profiling.Profiler.BeginSample("FrameUpdate");
            var deltaMs = ClientLockStep.displayDeltaMs;
            for (int index = m_fishes.Count - 1; index >= 0; --index)
            {
                var fish = m_fishes[index];
                if (fish)
                {
                    fish.OnDisplayUpdate(deltaMs);
                }
            }
            for (int index = m_bullets.Count - 1; index >= 0; --index)
            {
                var bullet = m_bullets[index];
                if (bullet)
                {
                    if (!bullet.FrameUpdate(deltaMs))
                    {
                        m_bullets.RemoveAt(index);
                        bullet.UnInitialize();
                        GameObjPool.Instance.DestroyObj(bullet.gameObject);
                    }
                }
                else
                {
                    m_bullets.RemoveAt(index);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void Refresh(string id, int value)
        {
            if (progress.ContainsKey(id))
            {
                progress[id] = value;
            }
            else
            {
                progress.Add(id, value);
            }
            if (m_combatType == GlobalDefine.GTypeFightSingle && value >= 100)
            {
                FightSceneManager.Inst().Send(new pps.GMapGameStartNotify() { randSeed = 0 });
            }
        }

        public void Enter()
        {
            var mapCfg = tab.map.Inst().Find(mapId);
            SceneManager.Instance.EnterMap(mapCfg.res, null, PreLoad);

            IFEBattle.onNotifyFightResult.Add(OnNotifyFightResult);
            IFEBattle.onFishAdd.Add(OnAddFish);
            IFEBattle.onFishRmv.Add(OnRmvFish);
            IFEBattle.onTrapAdd.Add(OnAddTrap);
            IFEBattle.onTrapRmv.Add(OnRmvTrap);
            statistic.Reset();

            combatState = CombatState.Loading;
        }
        public void Leave()
        {
            combatState = CombatState.Matching;
            IFEBattle.onNotifyFightResult.Rmv(OnNotifyFightResult);
            IFEBattle.onFishAdd.Rmv(OnAddFish);
            IFEBattle.onFishRmv.Rmv(OnRmvFish);            
            IFEBattle.onTrapAdd.Rmv(OnAddTrap);
            IFEBattle.onTrapRmv.Rmv(OnRmvTrap);

            m_targetFish = EntityId.Empty;
            ClearObjs(m_players);
            ClearObjs(m_fishes);
            ClearObjs(m_bullets);
            ClearObjs(m_traps);
            SceneManager.Instance.LeaveMap();
        }
        public void CompleteEnter()
        {
            combatState = CombatState.Fighting;
        }
        public void PVEDemoEnter(int mapId, int classId, List<pps.ProtoKnowledge> knows, int petMId)
        {
            var pet = new PetBase() { uniqueId = 1, mId = petMId, lv = 0, randSeed = 0 };
            PVELevelEnter(pet, mapId, classId, knows);
        }
        public void PVELevelEnter(int mapId, int classId, List<pps.ProtoKnowledge> knows)
        {
            var pet = PetModule.Inst().GetEquiped();
            PVELevelEnter(pet, mapId, classId, knows);
        }
        public void PVERetry() { PVELevelEnter(mapId, classId, knowledges); }
        public void PVEReborn() { (FightSceneManager.mod as FEBattlePVESingle).Reborn(); }
        void PVELevelEnter(PetBase pet, int mapId, int classId, List<pps.ProtoKnowledge> knows)
        {
            this.mapId = mapId;
            this.classId = classId;
            this.knowledges = knows;

            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
            ClientLockStep.Inst().onDisplayUpdate.Add(OnFrameUpdate);
            m_lockStepRefresher.Reset(GlobalParam.game.lockStepInterval, 0);
            TcpNetwork.Instance.StartLocal(new Net.NetSession() { roomId = 0, playerId = PlayerModule.MyId() });

            FEModule.Inst().StartUp(m_localSender, m_localSender);
            var resourceNo = new pps.GMapResourceLoadNotify() { roomId = 0, combatType = GlobalDefine.GTypeFightSingle, mapId = mapId };
            var combatPlayer = new pps.ProtoDetailPlayer()
            {
                uniqueId = PlayerModule.MyId(),
                sessionId = 1,
                name = PlayerModule.Inst().player.name,
                lv = 1,
                packageId = KnowledgeModule.Instance.lgapId,
                pet = pet.ToProto(),
            };
            combatPlayer.srvLogic = new pps.SrvLogic() { publicAddr = string.Empty, privateAddr = string.Empty };
            knowledges.ForEach(it => combatPlayer.knows.Add(it));
            resourceNo.players.Add(combatPlayer);
            Send(resourceNo);
        }       
        
        void OnSyncPlayer()
        {
            var players = mod.players;
            m_me = players.Find(it => it.uniqueId == PlayerModule.MyId());

            m_td.Init(SceneManager.Inst().terrain);
            for (int index = 0; index < players.Count; ++index)
            {
                var script = new GameObject("heroDisplay").AddComponent<PetFightObj>();
                script.Initialize(players[index] as FEDefenderObj);
                var p = m_td.Find((players[index].uniqueId == m_me.uniqueId) ? "dynamic/a" : "dynamic/b");
                Util.UnityHelper.Show(script, p, true);
                m_players.Add(script);
                SceneManager.Instance.AddObj(script);
            }
            ChatEnter();
        }
        void OnAddFish(IFEMonster fishObj)
        {
            var monsterCfg = tab.monster.Inst().Find(fishObj.GetData().moduleId);
            var resCfg = tab.objRes.Instance.Find(monsterCfg.resId);
            var obj = GameObjPool.Instance.CreateObjSync(GamePath.asset.fish.prefab, resCfg.name);
            var script = obj.GetComponent<FishNormObj>();
            if (!script) { script = obj.AddComponent<FishNormObj>(); }
            script.Initialize(fishObj);
            m_fishes.Add(script);
            SceneManager.Instance.AddObj(script);
        }
        void OnRmvFish(int uniqueId)
        {
            RmvObj(m_fishes, uniqueId);
        }
        void OnAddTrap(FETrap trap)
        {
            var skillCfg = tab.skill.Inst().Find(trap.skillCfg.id);
            var script = new GameObject(typeof(TrapObj).Name).AddComponent<TrapObj>();
            script.Initialize(trap);
            Util.UnityHelper.ShowAsChild(script.transform, null);
            script.SetPos(trap.targetAt);
            m_traps.Add(script);
            SceneManager.Inst().AddObj(script);
        }
        void OnRmvTrap(int uniqueId)
        {
            var obj = m_traps.Find(it => it.entityId.uniqueId == uniqueId);
            if (obj)
            {
                obj.Rmv();
            }
        }
        public void RmvTrap(TrapObj trap)
        {
            RmvObj(m_traps, trap.entityId.uniqueId);
        }
        public T AddBullet<T>(string res, Vector3 pos) where T : IBullet
        {
            var obj = GameObjPool.Instance.CreateObjSync(GamePath.asset.bullet.prefab, res);
            var script = obj.GetComponent<T>();
            if (!script) { script = obj.AddComponent<T>(); }
            //script.transform.parent = scene2d.area2d.area;
            script.transform.position = pos;
            m_bullets.Add(script);
            return script;
        }
        public void LogicUpdate()
        {
            UnityEngine.Profiling.Profiler.BeginSample("FrameUpdate");      
            if (m_sweeper != null)
            {
                m_sweeper.Update(FightSceneManager.mod.deltaMs);
            }
            for (int index = m_fishes.Count - 1; index >= 0; --index)
            {
                m_fishes[index].OnLockStepUpdate(FightSceneManager.mod.deltaMs);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public ISpline GetSpline(int splineId)
        {
            var splines = FEScene2d.Inst().FindSpline("");
            for (int index = 0; index < splines.splines.Count; ++index)
            {
                var spline = splines.splines[index];
                if (spline.Id == splineId) { return spline; }
            }
            return null;
        }
        public IEnumerator PreLoad()
        {
            var monster = tab.monster.Inst().RecordArray;
            for (int index = 0; index < monster.Count; ++index)
            {
                var resCfg = tab.objRes.Inst().Find(monster[index].resId);
                yield return GameObjPool.Instance.CreatePoolAsync(GamePath.asset.fish.prefab, resCfg.name);
            }
            
            for (int index = 0;index < players.Count; ++index)
            {
                var player = players[index];
                var petCfg = tab.hero.Inst().Find(player.pet.mId);
                var resCfg = tab.objRes.Inst().Find(petCfg.star.resId);
                yield return GameObjPool.Instance.CreatePoolAsync(GamePath.asset.pet.prefab, resCfg.name);                
            }
        }
        static CombatPlayer<IFEBattle> m_me;
        public static CombatPlayer<IFEBattle> Me() { return m_me; }
        public static int MySId() { return m_me.sessionId; }
        public static PetFightObj MyObj() { return Inst().Find<PetFightObj>(Obj3DType.Pet, m_me.sessionId); }
        public PetFightObj FindPlayer(int sessionId) { return Find<PetFightObj>(Obj3DType.Pet, sessionId); }
        public T Find<T>(EntityId eId) where T : IBaseObj
        {
            return SceneManager.Inst().GetAs<T>(eId);
        }
        public T Find<T>(int objType, int uniqueId) where T : IBaseObj
        {
            return Find<T>(new EntityId(objType, uniqueId));
        }
        void RmvObj<T>(List<T> objs, int uniqueId) where T : IBaseObj, IFinalize
        {
            var exist = objs.FindIndex(it => it.entityId.uniqueId == uniqueId);
            if (exist >= 0)
            {
                var obj = objs[exist];
                SceneManager.Inst().RmvObj(obj.entityId);
                objs.RemoveAt(exist);
                if (obj)
                {
                    obj.UnInitialize();
                    GameObjPool.Instance.DestroyObj(obj.gameObject);
                }
            }
        }
        void ClearObjs<T>(List<T> objs) where T : IFinalize
        {
            for (int index = 0; index < objs.Count; ++index)
            {
                var obj = objs[index];
                obj.UnInitialize();
                var script = obj as IBaseObj;
                if (script)
                {
                    SceneManager.Instance.RmvObj(script.entityId);
                    GameObjPool.Instance.DestroyObj(script.gameObject);
                }
            }
            objs.Clear();
        }
        
        public void OnNotifyFightResult()
        {
            onResult.Fire(MySId());
            //controller.NotifyFightResult(MySId());
        }

        public void Ski2Fish(CombatPlayer<IFEBattle> caster, cfg.skill skillCfg, FEVector3D targetPos, IList<IFEMonster> targets)
        {
            var casterObj = FightSceneManager.Inst().FindPlayer(caster.sessionId);
            casterObj.CastSkill(skillCfg.id);

            switch (skillCfg.effect.type)
            {
                case EffectType.AOERandom:
                    PlayChainFx(skillCfg, targetPos.V3(), targets); break;
                default: break;
            }
        }
        public void Ski2Player(CombatPlayer<IFEBattle> caster, cfg.skill skillCfg, IList<CombatPlayer<IFEBattle>> targets)
        {
            var casterObj = FindPlayer(caster.sessionId);
            casterObj.CastSkill(skillCfg.id);

            if (skillCfg.castType == cfg.skill.DirectFlyTo)
            {
                for (int index = 0; index < targets.Count; ++index)
                {
                    var targetObj = FindPlayer(targets[index].sessionId);
                    m_cor.StartCoroutine(FxFlyTo(casterObj.transform.position, targetObj.transform.position, skillCfg, skillCfg.delayRmvMs * 0.001f));
                }
            }
        }
        public void Ski2Other(CombatPlayer<IFEBattle> caster, cfg.skill skillCfg)
        {
            var casterObj = FindPlayer(caster.sessionId);
            casterObj.CastSkill(skillCfg.id);
        }
        public void FishSki2Fish(IFEMonster caster, cfg.skill skillCfg, IList<IFEMonster> targets)
        {
            switch (skillCfg.effect.type)
            {
                case EffectType.AOERandom:
                    PlayChainFx(skillCfg, caster.GetPosition().V3(), targets);
                    break;
                default: break;
            }
        }
        public void FishSki2Player(IFEMonster caster, cfg.skill skillCfg, IList<CombatPlayer<IFEBattle>> targets)
        {
            if (skillCfg.castType == cfg.skill.DirectFlyTo)
            {
                var casterObj = Find<FishObj>(Obj3DType.Fish, caster.UId());
                for (int index = 0; index < targets.Count; ++index)
                {
                    var targetObj = FightSceneManager.Inst().FindPlayer(targets[index].sessionId);
                    m_cor.StartCoroutine(FxFlyTo(casterObj.transform.position, targetObj.transform.position, skillCfg, skillCfg.delayRmvMs * 0.001f));
                }
            }
        }
        public void FishSki2Fish(IFEMonster caster, cfg.monsterSkill skiCfg, IList<IFEMonster> targets)
        {
            var casterObj = Find<FishObj>(Obj3DType.Fish, caster.UId());
            casterObj.Ski2Fish(skiCfg, targets);
        }
        public void FishSki2Player(IFEMonster caster, cfg.monsterSkill skiCfg, IList<CombatPlayer<IFEBattle>> targets)
        {
            var casterObj = Find<FishObj>(Obj3DType.Fish, caster.UId());
            casterObj.Ski2Player(skiCfg, targets);
        }
        public IEnumerator FxFlyTo(Vector3 from, Vector3 to, cfg.skill skillCfg, float time)
        {
            var fx = FXControl.Create(GamePath.asset.fx3D, skillCfg.castFx, false);
            if (fx)
            {
                fx.Play(null);
                fx.transform.position = from;
                fx.transform.rotation = Quaternion.identity;
                yield return fx.FlyWithTime(from, to, skillCfg.delayRmvMs * 0.001f);
                fx.Stop();
                FXControl.Destroy(fx);
            }
        }
        public void PlayChainFx(cfg.skill skiCfg, Vector3 center, IList<IFEMonster> targets)
        {
            for (int index = 0; index < targets.Count; ++index)
            {
                var obj = FightSceneManager.Inst().Find<IBaseObj>(Obj3DType.Fish, targets[index].UId());
                if (!obj) { continue; }

                var pos = obj.transform.position;
                var fx = FXControl.Create(GamePath.asset.fx3D, skiCfg.castFx, false);
                fx.duration = skiCfg.delayRmvMs * 0.001f;
                fx.PlayAutoDestroy(null, pos, true);
                var scale = Vector3.one;
                var distance = (center - pos);
                scale.y = distance.magnitude / 100f;
                fx.transform.localScale = scale;
                fx.transform.up = distance;
            }
        }
        public FishObj Raycast(Vector3 position)
        {
            for (int index = 0; index < m_fishes.Count; ++index)
            {
                var dist = m_fishes[index].transform.position - position;
                dist.y = 0f;
                if (dist.sqrMagnitude < m_fishes[index].attachValue.radius * m_fishes[index].attachValue.radius)
                {
                    return m_fishes[index];
                }
            }
            return null;
        }
    }
}