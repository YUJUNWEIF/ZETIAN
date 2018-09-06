using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Util;
using Net;

namespace geniusbaby
{
    public class PhysicNetwork : INetwork, IDisposable
    {
        Socket m_socket;
        CVector<object> m_totalReceive = new CVector<object>();
        CVector<object> m_onceReceive = new CVector<object>();

        List<object> m_sending = new List<object>();
        Action m_onShutDown;
        public void StartUp(string ip, int port, Action onShutDown)
        {
            var ipAddr = IPAddress.Parse(ip);
            m_socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Connect(ipAddr, port);
            m_onShutDown = onShutDown;
        }
        public void Dispose()
        {
            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
            m_onShutDown();
        }
        public void Receive()
        {
            if (m_socket == null) { return; }
            try
            {
                var readset = new List<Socket>();
                readset.Add(m_socket);
                Socket.Select(readset, null, null, 30);
                if (readset.Count > 0)
                {
                    var m_readBuffer = Util.PoolByteArrayAlloc.alloc.Alloc(4 * 1024);
                    int len = m_socket.Receive(m_readBuffer);
                    if (len > 0)
                    {
                        NetHelper.Decode(m_readBuffer, 0, len, m_onceReceive);
                        m_totalReceive.AddRange(m_onceReceive);
                        m_onceReceive.Clear();
                    }
                    else
                    {
                        Dispose();
                    }
                }
            }
            catch (Exception)
            {
                Dispose();
            }
        }
        byte[] m_buffer = Util.PoolByteArrayAlloc.alloc.Alloc(256);
        public void Send(object proto)
        {
            try
            {
                var position = NetHelper.Encode(proto, m_buffer, 0);
                m_socket.Send(m_buffer, 0, position, SocketFlags.None);
            }
            catch (Exception)
            {
                Dispose();
            }
        }
        public void Ping() { }
        public IList<object> Swap(long currentTimeMs)
        {
            Receive();
            return m_totalReceive;
        }
    }
}