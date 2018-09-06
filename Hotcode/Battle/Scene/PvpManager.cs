using System;
using System.Collections.Generic;
using Util;
using UnityEngine;

namespace geniusbaby
{
    public class GMapSrvProtoSender : IProtoSender, IResultReporter
    {
        LocalNetwork m_local;
        bool m_debugSync;
        CVector<object> m_onceReceive = new CVector<object>();
        public GMapSrvProtoSender(LocalNetwork local, bool debugSync)
        {
            m_local = local;
            m_debugSync = debugSync;
        }
        public bool IsLinkBroken(string uniqueId)
        {
            return false;
        }
        public void Send(string uniqueId, byte[] buffer, int offset, int count)
        {
            Net.NetHelper.Decode(buffer, offset, count, m_onceReceive);
            if (m_debugSync)
            {
                DebugSync(m_onceReceive);
            }
            else
            {
                m_local.StuffReceive(m_onceReceive);
            }
            m_onceReceive.Clear();
        }
        public void Report(string url, byte[] buffer, int length) { }
        void DebugSync(IList<object> protos)
        {
            for (int index = 0; index < protos.Count; ++index)
            {
                try
                {
                    var proto = protos[index];
                    var factory = Net.ProtoManager.manager.GetImpl(proto);
                    factory(proto, null).Process();
                    Net.ProtoManager.manager.Free(proto);
                }
                catch (System.Exception ex)
                {
                    Util.Logger.Instance.Error(ex.Message, ex);
                }
            }
        }
        public void Kickout(string uniqueId, int afterTimeMs) { }
    }
    public abstract class PvpManager<T> : Singleton<T>, IPvpManager
        where T : PvpManager<T>
    {
        protected pps.SrvFight m_ipAddr;
        protected object m_report;
        protected Action m_networkNotReachable;
        protected Refresher m_lockStepRefresher = new Refresher();
        protected GMapSrvProtoSender m_localSender = new GMapSrvProtoSender(TcpNetwork.Inst().NetAs<LocalNetwork>(), true);
        protected int m_combatType;

        public CombatState combatState;
        public bool pause;
        public IPvpListener listener;
        //public abstract ILockStepFight mod { get; }

        public abstract void OnNosession();
        protected virtual void OnLoop() { }
        protected virtual void OnFrameUpdate() { }

        public void Send(object proto)
        {
            var buffer = Util.PoolByteArrayAlloc.alloc.Alloc(1024);
            int position = Net.NetHelper.Encode(proto, buffer, 0);
            if (position > 0)
            {
                m_localSender.Send(null, buffer, 0, position);
            }
            Util.PoolByteArrayAlloc.alloc.Free(ref buffer);
        }
        public void PvpEnter(pps.SrvFight addr, object report, Action networkNotReachable)
        {
            this.m_ipAddr = addr;
            this.m_report = report;
            this.m_networkNotReachable = networkNotReachable;

            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
            FEModule.Inst().StartUp(m_localSender, m_localSender);
            ClientLockStep.Inst().onDisplayUpdate.Add(OnFrameUpdate);
            TcpNetwork.Inst().StartKcp(m_ipAddr.ip, m_ipAddr.combatPort, OnAbnormal);

            TcpNetwork.Inst().Send(m_report);
        }
        public void PvpLeave()
        {
            TcpNetwork.Instance.Stop();
            Util.TimerManager.Inst().onFrameUpdate.Rmv(OnUpdate);
            FEModule.Inst().Stop();
            ClientLockStep.Inst().onDisplayUpdate.Rmv(OnFrameUpdate);
            ClientLockStep.Inst().Leave();

            ChatLeave();
        }
        public void ChatEnter()
        {
            if (m_ipAddr != null && !string.IsNullOrEmpty(m_ipAddr.ip))
            {
                ChatManager.Inst().Enter(m_ipAddr, FightSceneManager.mod.roomId, PlayerModule.MyId(), null);
            }
        }
        public void ChatLeave()
        {
            ChatManager.Inst().Leave();
        }

        public void OnAbnormal()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable: m_networkNotReachable(); break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    TcpNetwork.Instance.Stop();
                    TcpNetwork.Inst().StartKcp(m_ipAddr.ip, m_ipAddr.combatPort, OnAbnormal);
                    if (combatState == CombatState.Loading || combatState == CombatState.Fighting)
                    {
                        var battle = FightSceneManager.mod;
                        TcpNetwork.Inst().Send(new pps.GMapPvpReconnectReport() { playerId = PlayerModule.MyId(), value = (battle != null) ? battle.stepId : 0 });
                    }
                    else
                    {
                        TcpNetwork.Inst().Send(m_report);
                    }
                    break;
            }
        }
        protected void OnUpdate()
        {
            //FightSceneManager.Inst().loader.Run();
            //listener.Run();
            OnLoop();

            if (pause) { return; }

            UnityEngine.Profiling.Profiler.BeginSample("OnUpdate");
            var deltaMs = Time.deltaTime * 1000;
            if (deltaMs > GlobalParam.game.lockStepInterval) { deltaMs = GlobalParam.game.lockStepInterval; }
            if (m_combatType == GlobalDefine.GTypeFightSingle)
            {
                if (m_lockStepRefresher.Update(FEMath.RoundToInt(deltaMs))) { FEModule.Inst().Update(m_lockStepRefresher.GetActiveMs()); }
                ClientLockStep.Inst().FrameUpdate(deltaMs);
            }
            else
            {
                ClientLockStep.Inst().FrameUpdate(deltaMs);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}