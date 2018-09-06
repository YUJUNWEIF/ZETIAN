using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Net;
using Util;

namespace geniusbaby
{
    public interface ILockStep
    {
        void Run();
    }
    public class ClientLockStep : Singleton<ClientLockStep>
    {
        LockStep empty = new LockStep();
        DList<LockStep> m_lockStepActions = new DList<LockStep>(PoolObjAlloc<DListNode>.alloc);
        public Util.ParamActions onDisplayUpdate = new Util.ParamActions();
        public static float displayTimeMs { get; private set; }
        public static float displayDeltaMs { get; private set; }
        const int SlowDownLagging = 5;
        const int SpeedUpLagging = 3;
        bool m_running = false;
        ILockStepFight m_battle;
        public void Enter(ILockStepFight battle)
        {
            m_battle = battle;
            m_running = true;
            displayTimeMs = m_battle.timeMs;
            m_lockStepActions.Clear();
        }
        public void Leave()
        {
            m_lockStepActions.Clear();
            displayTimeMs = int.MaxValue;
            m_running = false;
        }
        public void Receive(LockStep step)
        {
            m_lockStepActions.AddLast(step);
        }
        public void Reconnecting(IList<LockStep> steps)
        {
            int maxStepId = -1;
            var no = m_lockStepActions.Last;
            if (no != null) { maxStepId = no.V<LockStep>().stepId; }
            else
            {
                maxStepId = m_battle.stepId;
            }
            for (int index = 0; index < steps.Count; ++index)
            {
                var step = steps[index];
                if (step.stepId > maxStepId) { m_lockStepActions.AddLast(step); }
            }
        }
        public void SingleUpdate(float deltaMs)
        {
            if (!m_running) { return; }
            CallOnce(deltaMs);
        }
        public void FrameUpdate(float deltaMs)
        {
            if (!m_running) { return; }
            var deltaLockStepMs = m_battle.deltaMs;
            var tempRealTimeMs = m_battle.timeMs;
            if (m_lockStepActions.Count > 0)
            {
                tempRealTimeMs = m_lockStepActions.Last.V<LockStep>().stepId * deltaLockStepMs;
            }
            if (displayTimeMs >= tempRealTimeMs)//网络卡，lockstep包大延迟接收，导致客户端时间比服务器同步包快，需要减慢客户端
            {
                var delay = (displayTimeMs - tempRealTimeMs) / deltaLockStepMs;
                var n = FEMath.Clamp(delay, 0f, 20f);
                if (n > SlowDownLagging) { deltaMs = 0f; }
                else
                {
                    var slowdown = (n <= 0) ? 1f : 1f / (FEMath.GetFibonacciLerp(n) + 0.25f);
                    deltaMs = deltaMs * slowdown;
                }
                CallOnce(deltaMs);
            }
            else//网络卡，导致客户端时间比服务器同步包慢，需要加速客户端
            {
                var delay = (tempRealTimeMs - displayTimeMs) / deltaLockStepMs;
                var n = FEMath.Clamp(delay, 0f, 20f);
                var multi = (n <= 0) ? 0f : FEMath.GetFibonacciLerp(n);
                deltaMs = deltaMs * (1 + multi * 0.25f);
                if (deltaMs + displayTimeMs >= tempRealTimeMs)
                {
                    deltaMs = tempRealTimeMs - displayTimeMs;
                }
                while (deltaMs > SpeedUpLagging * deltaLockStepMs)
                {
                    deltaMs -= deltaLockStepMs;
                    CallOnce(deltaLockStepMs);
                }
                if (deltaMs > deltaLockStepMs) { deltaMs = deltaLockStepMs; }
                CallOnce(deltaMs);
            }
        }
        void CallOnce(float deltaMs)
        {
            displayDeltaMs = deltaMs;
            displayTimeMs += displayDeltaMs;
            while (m_lockStepActions.Count > 0)
            {
                var stepId = m_battle.stepId + 1;
                LockStep curStep = m_lockStepActions.First.V<LockStep>();
                var tmpRealTimeMs = stepId * m_battle.deltaMs;
                if (displayTimeMs >= tmpRealTimeMs)
                {
                    //if (m_battle.state == FightState.Unknown)
                    //{
                    //}
                    //else
                    if (curStep.stepId == stepId)
                    {
                        m_lockStepActions.RemoveFirst();
                        //if (curStep.debugRandSeed != m_battle.DebugGetRSeed() ||
                        //    curStep.debugIdMaker != (uint)m_battle.DebugGetUniqueId())
                        //{
                        //    Util.Logger.Instance.Error("not sync");
                        //}
                        m_battle.Process(curStep);
                        Net.ProtoManager.manager.Free(curStep);
                    }
                    else
                    {
                        empty.stepId = stepId;
                        m_battle.Process(empty);
                    }
                    FEModule.Inst().Update(m_battle.deltaMs);
                }
                else
                {
                    break;
                }
            }
            if (!FEMath.RealEqual(displayDeltaMs, 0, 0.005f)) { onDisplayUpdate.Fire(); }
        }
    }
}