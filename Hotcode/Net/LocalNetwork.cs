using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Util;

namespace geniusbaby
{
    public class LocalNetwork : INetwork
    {
        Util.CVector<object> m_totalReceive = new Util.CVector<object>();
        Util.CVector<object> m_onceReceive = new Util.CVector<object>();
        Action<object> m_protoImplCaller;
        public void StartUp(Action<object> protoImplCaller)
        {
            m_protoImplCaller = protoImplCaller;
        }
        public void Dispose() { }
        public void Send(object proto)
        {
            m_protoImplCaller(proto);
        }
        public void Ping() { }
        void Receive()
        {
            m_totalReceive.AddRange(m_onceReceive);
            m_onceReceive.Clear();
        }
        public IList<object> Swap(long currentTimeMs)
        {
            Receive();
            return m_totalReceive;
        }
        public void StuffReceive(IList<object> protos)
        {
            m_onceReceive.AddRange(protos);
        }
    }
}