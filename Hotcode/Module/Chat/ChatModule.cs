using System;
using System.Collections.Generic;
using System.Net;
using Util;
using Net;

namespace geniusbaby
{
    public class ChatManager : Singleton<ChatManager>, IGameEvent
    {
        public List<IGMapChatData> chats = new List<IGMapChatData>();
        List<ChatData> fillings = new List<ChatData>();
        int roomId;
        public int mySId;
        string uId;
        pps.SrvFight addr;
        KcpClient<UxpOnUdp> kcp = new KcpClient<UxpOnUdp>();
        Refresher m_kcpRingRefresher = new Refresher(1000, 0);
        INetwork m_network;
        ushort SYNC = 0;
        public SpeechCachePlay soundPlayer;

        public Util.Param1Actions<IGMapChatData> onAdd = new Util.Param1Actions<IGMapChatData>();
        public Util.Param1Actions<IGMapChatData> onRmv = new Util.Param1Actions<IGMapChatData>();
        public void OnStartGame() { }
        public void OnStopGame() { }
        public void Enter(pps.SrvFight addr, int roomId, string uId, List<int> roomSIds)
        {
            this.addr = addr;
            this.roomId = roomId;
            this.uId = uId;
            SYNC = (ushort)(Framework.rand.NextUInt() & 0xFFFF);
            StartKcp(addr.ip, addr.chatPort, OnAbnormal);
        }
        public void Leave()
        {
            for (int index = 0; index < chats.Count; ++index)
            {
                chats[index].Clear();
            }
            chats.Clear();
            for (int index = 0; index < fillings.Count; ++index)
            {
                fillings[index].Clear();
            }
            fillings.Clear();
            Stop();
        }
        public void Online(int mySId)
        {
            this.mySId = mySId;
        }
        public bool isOnline { get { return this.mySId > 0; } }
        public void OnAbnormal()
        {
            Stop();
            StartKcp(addr.ip, addr.chatPort, OnAbnormal);
        }
        public void AudioRecv(int talkerId, byte[] buffer)
        {
            var exist = fillings.FindIndex(it => it.talkerSId == talkerId);
            if (buffer != null && buffer.Length > 0)
            {
                var filling = (exist >= 0) ? fillings[exist] : null;
                if (filling == null)
                {
                    fillings.Add(filling = new ChatData(talkerId));
                }
                filling.Fill(buffer);
            }
            else if (exist >= 0)
            {
                var filling = fillings[exist];
                fillings.RemoveAt(exist);
                chats.Add(filling);
                onAdd.Fire(filling);

                if (chats.Count > 50)
                {
                    var data = chats[0];
                    chats.RemoveAt(0);
                    data.Clear();
                    onRmv.Fire(data);
                }
            }
        } 
        public void TextRecv(int talkerId, string msg)
        {
            var data = new TextChatData(talkerId, msg);
            chats.Add(data);
            onAdd.Fire(data);
        }
        public void Send(object proto)
        {
            Logger.Instance.Info("TS " + proto.GetType().ToString());
            if (m_network != null) { m_network.Send(proto); }
        }
        void StartKcp(string ip, int port, Action onAbnormal)
        {
            var addr = new IPEndPoint(IPAddress.Parse(ip), port);
            var onUdp = new UxpOnUdp(++SYNC, GamePath.net.keepAlive * 1000, TimerManager.Inst().RealTimeMS());
            kcp.Start(onUdp, addr, onAbnormal);
            //kcp.Start(++SYNC, addr, onAbnormal);
            m_network = kcp;
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);

            Send(new pps.GMapChatConnectReport() { roomId = roomId, uId = uId });
        }
        void Stop()
        {
            this.mySId = -1;

            Util.TimerManager.Inst().onFrameUpdate.Rmv(OnUpdate);
            if (m_network != null)
            {
                m_network.Dispose();
                m_network = null;
            }
        }
        void OnUpdate()
        {
            if (m_network == null) { return; }
            var protos = m_network.Swap(TimerManager.Inst().RealTimeMS());
            if (protos.Count > 0)
            {
                for (int index = 0; index < protos.Count; ++index)
                {
                    try
                    {
                        var proto = protos[index];
                        var factory = ProtoManager.manager.GetImpl(proto);
                        factory(proto, null).Process();
                        Net.ProtoManager.manager.Free(proto);
                    }
                    catch (Exception ex)
                    {
                        Util.Logger.Instance.Error(ex.Message, ex);
                    }
                }
                protos.Clear();
            }

            if (m_kcpRingRefresher.Update(TimerManager.Inst().DeltaTimeMS()))
            {
                m_network.Ping();
            }
        }
    }
}
