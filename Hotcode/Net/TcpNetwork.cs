using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Util;
using Net;

namespace geniusbaby
{
    public interface INetwork : IDisposable
    {
        void Send(object proto);
        void Ping();
        IList<object> Swap(long currentTimeMs);
    }
    public class TcpNetwork : Singleton<TcpNetwork>
    {
        enum State
        {
            Unknown = 0,
            Running = 1,
            Stoped = 2,
            WillStop = 3,
        }
        State m_state;
        long m_stopAt;
        LocalNetwork local = new LocalNetwork();
        PhysicNetwork physic = new PhysicNetwork();
        KcpClient<UxpOnUdp> kcp = new KcpClient<UxpOnUdp>();
        Refresher m_kcpRingRefresher = new Refresher(1000, 0);
        INetwork m_network;
        NetSession m_session;
        ushort SYNC = 0;
        public float delay { get; set; }
        public T NetAs<T>() where T : INetwork
        {
            return (T)m_network;
        }
        public TcpNetwork()
        {
            SYNC = (ushort)(Framework.rand.NextUInt() & 0xFFFF);
        }
        public void StartLocal(NetSession session)
        {
            Stop();
            m_state = State.Running;
            local.StartUp(Exe);
            m_network = local;
            m_session = session;
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
        }
        public void StartTcp(string ip, int port, Action onShutDown)
        {
            Stop();
            m_state = State.Running;
            physic.StartUp(ip, port, onShutDown);
            m_network = physic;
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
        }
        public void StartKcp(string ip, int port, Action onAbnormal)
        {
            Stop();
            m_state = State.Running;
            var addr = new IPEndPoint(IPAddress.Parse(ip), port);
            var onUdp = new UxpOnUdp(++SYNC, GamePath.net.keepAlive * 1000, TimerManager.Inst().RealTimeMS());
            kcp.Start(onUdp, addr, onAbnormal);
            //kcp.Start(++SYNC, addr, onAbnormal);
            m_network = kcp;
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
        }
        public void Stop()
        {
            Util.TimerManager.Inst().onFrameUpdate.Rmv(OnUpdate);
            if (m_network != null)
            {
                m_network.Dispose();
                m_network = null;
            }
            m_state = State.Stoped;
        }
        public void Send(object proto)
        {
            Logger.Instance.Info("TS " + proto.GetType().ToString());
            if (m_network != null) { m_network.Send(proto); }
        }
        public void OnUpdate()
        {
            if (m_state == State.WillStop && TimerManager.Inst().RealTimeMS() >= m_stopAt)
            {
                Stop();
            }
            if (m_network == null) { return; }
            var protos = m_network.Swap(TimerManager.Inst().RealTimeMS());
            if (protos.Count > 0)
            {
                for (int index = 0; index < protos.Count; ++index)
                {
                    try
                    {
                        Exe(protos[index]);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error(ex.Message, ex);
                    }
                }
                protos.Clear();
            }

            if (m_state == State.Running && m_kcpRingRefresher.Update(TimerManager.Inst().DeltaTimeMS()))
            {
                m_network.Ping();
            }
        }
        public void DelayStop(int delaySec)
        {
            m_state = State.WillStop;
            m_stopAt = TimerManager.Inst().RealTimeMS() + delaySec * 1000;
        }
        void Exe(object proto)
        {
            var factory = ProtoManager.manager.GetImpl(proto);
            factory(proto, m_session).Process();
            Net.ProtoManager.manager.Free(proto);
        }
    }
}
