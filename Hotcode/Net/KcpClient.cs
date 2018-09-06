using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using Net;
using Util;

namespace geniusbaby
{
    public class KcpClient<T> : INetwork, ISendImpl, INetListener
        where T : class, INetUserToken, IUKxp
    {
        T m_kcp;
        //KcpOnUdp m_kcp;
        Action m_onAbnormal;
        Udp m_udp;
        Util.CVector<object> m_protos = new Util.CVector<object>();
        Util.CVector<object> m_onceReceive = new Util.CVector<object>();
        byte[] m_buffer = Util.PoolByteArrayAlloc.alloc.Alloc(1024);
        public void Send(object proto)
        {
            var position = Net.NetHelper.Encode(proto, m_buffer, 0);
            m_kcp.Send(m_buffer, 0, position);
        }
        public void Ping()
        {
            Send(new pps.GMapPingRequest() { utcMs = TimerManager.Inst().RealTimeMS() });
        }
        void Receive(long currentTimeMs)
        {
            // 处理虚拟网络：检测是否有udp包从p1->p2
            var datas = m_udp.Receive();
            for (int j = 0; j < datas.Count; ++j)
            {
                m_kcp.Input(datas[j].content);
            }
            m_kcp.Update(currentTimeMs);
        }
        public IList<object> Swap(long currentTimeMs)
        {
            Receive(currentTimeMs);
            return m_protos;
        }
        public void Dispose() { Stop(); }
        // 测试用例

        //public void Start(uint conv, IPEndPoint endport, Action onAbnormal)
        //{
        //    //var remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        //    m_udp = new Udp();
        //    m_udp.Connect(endport);
        //    m_udp.Start();
        //    m_kcp = new KcpOnUdp(conv, ClientParamSetting.clientKeepAlive * 1000);
        //    m_kcp.Bind(null, endport, this, this);
        //    m_onAbnormal = onAbnormal;
        //}
        public void Start(T ukxp, IPEndPoint endport, Action onAbnormal)
        {
            m_udp = new Udp();
            m_udp.Connect(endport);
            m_udp.Start();
            m_kcp = ukxp;
            m_kcp.Bind(null, endport, this, this);
            m_onAbnormal = onAbnormal;
        }
        public void Stop()
        {
            if (m_udp != null)
            {
                m_udp.Stop();
                m_udp = null;
            }
            if (m_kcp != null)
            {
                m_kcp.CloseBoth();
                m_kcp = null;
            }
        }
        public void Send(byte[] data, int size, object user)
        {
            try
            {
                m_udp.Send(data, size);
            }
            catch (Exception ex)
            {
                Util.Logger.Instance.Debug(ex.Message, ex);
                m_onAbnormal();
            }
        }
        public void OnReceive(byte[] bytes, int offset, int size, NetSession session)
        {
            NetHelper.Decode(bytes, offset, size, m_onceReceive);
            m_protos.AddRange(m_onceReceive);
            m_onceReceive.Clear();
        }
        public void OnAbnormal(NetSession session)
        {
            m_onAbnormal();
        }
        public void OnClose(NetSession session) { }
    }
}
