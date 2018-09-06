using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public class FishNormObj : FishObj
    {
        Transform text;
        GameObject alphabetObj;
        public override void Initialize(IFEMonster fishObj)
        {
            base.Initialize(fishObj);
            for (int index = 0; index < colls.Length; ++index) { colls[index].enabled = true; }
            text = cachedTrans.Find("model/Text");
            if (!text) { text = transform; } 
            OnFishUpdate();
        }
        public override void UnInitialize()
        {
            if (alphabetObj) { GameObjPool.Inst().DestroyObj(alphabetObj); alphabetObj = null; }
            base.UnInitialize();
        }
        public override void Hit(int sessionId, RaycastHit hitInfo)
        {
            var position = hitInfo.transform.position;

            var me = sessionId == FightSceneManager.MySId();
            var fx = FXControl.Create(GamePath.asset.bullet.prefab, me ? "wang" : "wang_01");
            FightSceneManager.Inst().scene2d.Play2dFx(fx, position, true);

            if (me)
            {
                FightSceneManager.Inst().target = entityId;

                var physicAttack = new pps.GSrvPhysicAttackReport();
                physicAttack.uIds.Add(entityId.uniqueId);
                TcpNetwork.Inst().Send(physicAttack);
            }
        }
        public override void Hit(int sessionId, Vector3 position)
        {
            var me = sessionId == FightSceneManager.MySId();
            var fx = FXControl.Create(GamePath.asset.bullet.prefab, me ? "wang" : "wang_01");
            FightSceneManager.Inst().scene2d.Play2dFx(fx, position, true);
            if (me)
            {
                FightSceneManager.Inst().target = entityId;

                var physicAttack = new pps.GSrvPhysicAttackReport();
                physicAttack.uIds.Add(entityId.uniqueId);
                TcpNetwork.Inst().Send(physicAttack);
            }
        }        
        protected override void OnKilled(int sessionId)
        {
            if (sessionId == FightSceneManager.MySId()) { FightSceneManager.Inst().statistic.KillNormalFish(moduleId); }
            if (alphabetObj) { GameObjPool.Inst().DestroyObj(alphabetObj); alphabetObj = null; }
            for (int index = 0; index < colls.Length; ++index) { colls[index].enabled = false; }
        }
        protected virtual void LateUpdate()
        {
            UnityEngine.Profiling.Profiler.BeginSample("Alphabet");
            if (alphabetObj)
            {
                var defender = FightSceneManager.Me() as FEDefenderObj;
                if (defender != null)
                {
                    bool canSee = !defender.AnyExist(PlayerState.Shadow);
                    if (alphabetObj.activeSelf != canSee) { alphabetObj.SetActive(canSee); }
                }
            }
            if (text)
            {
                //CameraControl.Inst().Billboard(text.transform, true, true);
                text.transform.Rotate(0f, Time.deltaTime * 180f, 0f, Space.Self);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        protected override void OnFishUpdate()
        {
            if (text)
            {
                if (alphabetObj) { GameObjPool.Inst().DestroyObj(alphabetObj); alphabetObj = null; }

                var directVisit = attachValue.GetData();
                if (directVisit.alphabet)
                {
                    var ch = (directVisit.alphabet.ch == Alphabet.Wildchar) ? "wildchar" : directVisit.alphabet.ToString();
                    alphabetObj = GameObjPool.Inst().CreateObjSync(GamePath.asset.alphabet.prefab, ch);
                    Util.UnityHelper.ShowAsChild(alphabetObj.transform, text);
                }
            }
        }
    }
}