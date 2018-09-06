using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using Net;

namespace geniusbaby
{
    public class HttpNetwork : Singleton<HttpNetwork>
    {
        public class WaitHttpComplete : Util.IYieldInstruction
        {
            public bool done { get { return !HttpNetwork.Instance.IsSending(); } }
        }
        public bool IsSending() { return HttpNetwork.Instance.m_protoSending != null; }
        private object m_protoSending;
        public string session { get; private set; }
        public object Rpc { get; private set; }
        public static object patchParam { get; set; }//patch for video
        private LinkedList<object> m_protoWaiting = new LinkedList<object>();
        public static Util.ParamActions onCommunicateDoing = new Util.ParamActions();
        public static Util.ParamActions onCommunicateSucceed = new Util.ParamActions();
        public static Util.Param1Actions<string> onCommunicateFailed = new Util.Param1Actions<string>();
        public static Util.ParamActions onSessionInvalid = new Util.ParamActions();
        public Action<int, ProtoBuf.IExtensible> doSync { get; set; }
        Util.CoroutineHelper coroutineHelper = new Util.CoroutineHelper();
        Dictionary<string, string> m_headers = new Dictionary<string, string>();
        int m_date;
        LinkedList<Type> m_delayProtos = new LinkedList<Type>();
        LinkedList<Type> m_filterProtos = new LinkedList<Type>();
        public void Delay(Type type) { m_delayProtos.AddLast(type); }
        public void Filter(Type type) { m_filterProtos.AddLast(type); }
        public bool IsDelay(Type type) { return m_delayProtos.Contains(type); }
        public bool IsFilter(Type type) { return m_filterProtos.Contains(type); }
        public HttpNetwork()
        {
            //m_filterProtos.AddLast(typeof(pps.ItemUseRequest));
        }
        public void StartGame()
        {
            //m_date = DateTime.UtcNow.Day;
            //m_headers = new Dictionary<string, string>();
            //m_headers.Add("X-VERSION", VersionReader.version.version);
            //proto.ProtoReg.Start();
            Util.TimerManager.Inst().onFrameUpdate.Add(OnTimer);
        }
        public void StopGame()
        {
            Util.TimerManager.Inst().onFrameUpdate.Rmv(OnTimer);
            //proto.ProtoReg.Stop();
        }
        //public void Initialize() { session = null; }
        public void SetHttpURL(string url) { }
        public void CommunicateImmediate(object proto)
        {
            //if (ProtocolManager.BeFiltered(proto.GetType()))
            {
                if (m_protoSending != null && m_protoSending.GetType() == proto.GetType()) { return; }
                var no = m_protoWaiting.First;
                while (no != null)
                {
                    if (proto.GetType() == no.Value.GetType()) { return; }
                    no = no.Next;
                }
            }
            m_protoWaiting.AddFirst(proto);
            OnTimer();
        }
        public void Communicate(object proto)
        {
            //if (ProtocolManager.BeFiltered(proto.GetType()))
            {
                if (m_protoSending != null && m_protoSending.GetType() == proto.GetType()) { return; }
                var no = m_protoWaiting.First;
                while (no != null)
                {
                    if (proto.GetType() == no.Value.GetType()) { return; }
                    no = no.Next;
                }
            }
            m_protoWaiting.AddLast(proto);
            OnTimer();
        }
        public bool hasWaiting { get { return m_protoSending != null || m_protoWaiting.Count > 0; } }
        void OnTimer()
        {
            if (DateTime.UtcNow.Day - m_date > 0)
            {
                //if (doSync != null) doSync(NetHelper._NetSync, null);
                m_date = DateTime.UtcNow.Day;
            }
            else if (m_protoSending == null)
            {
                if (m_protoWaiting.First != null)
                {
                    var proto = m_protoWaiting.First.Value;
                    m_protoWaiting.RemoveFirst();
                    Util.Logger.Instance.Info("Send protocol : " + proto.ToString());
                    m_protoSending = proto;
                    coroutineHelper.StartCoroutine(AnSyncCommunicate());
                }
            }
        }
        byte[] m_buffer = Util.PoolByteArrayAlloc.alloc.Alloc(1024);
        private void ResovleProto(byte[] data)
        {
            var protos = NetHelper.Decode(data);
            if (protos.Count > 0)
            {
                var rep = protos[0];
                if (!IsDelay(rep.GetType()))
                {
                    for (int index = 0; index < protos.Count; ++index)
                    {
                        var proto = protos[index];
                        Util.Logger.Instance.Info("Execute protocol : " + proto.ToString());
                        try
                        {
                            var factory = ProtoManager.manager.GetImpl(proto);
                            factory(proto, null).Process();
                        }
                        catch (System.Exception ex)
                        {
                            Util.Logger.Instance.Error(ex.Message, ex);
                        }
                    }
                }
                else
                {
                    protos.RemoveAt(0);
                    Util.Logger.Instance.Info("Execute protocol : " + rep.ToString());
                    try
                    {
                        var factory = ProtoManager.manager.GetImpl(rep);
                        factory(rep, protos).Process();
                    }
                    catch (System.Exception ex)
                    {
                        Util.Logger.Instance.Error(ex.Message, ex);
                    }
                }
            }
        }
        IEnumerator AnSyncCommunicate()
        {
            using (var www = Send(GamePath.url.logicURL, m_protoSending))
            {
                const float timeOut = 10f;
                float timer = 0f;
                while (true)
                {
                    timer += Time.deltaTime;
                    if (www.isDone)
                    {
                        Receive(www);
                        break;
                    }
                    else if (timer > timeOut)
                    {
                        string timeout = "httprequest timeout";
                        Util.Logger.Instance.Error(timeout);
                        var tmp = m_protoSending;
                        m_protoSending = null;
                        if (!IsFilter(tmp.GetType())) { onCommunicateFailed.Fire(timeout); }
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }
        WWW Send(string url, object proto)
        {
            if (!IsFilter(proto.GetType())) { onCommunicateDoing.Fire(); }
            m_headers.Clear();
            m_headers.Add("NMVC_APIRequest", "ProtoExe");
            if (!string.IsNullOrEmpty(session))
            {
                m_headers.Add("sId", session);
            }
            var position = NetHelper.Encode(proto, m_buffer, 0);
            return new WWW(url, Util.ByteArrayBuffer.ToArray(m_buffer, 0, position), m_headers);
        }
        void Receive(WWW www)
        {
            if (www.error != null)
            {
                Util.Logger.Instance.Error("error is :" + www.error);
                var tmp = m_protoSending;
                m_protoSending = null;
                if (!IsFilter(tmp.GetType())) { onCommunicateFailed.Fire(www.error); }
            }
            else
            {
                try
                {
                    Rpc = m_protoSending;
                    m_protoSending = null;
                    if (!IsFilter(Rpc.GetType())) { onCommunicateSucceed.Fire(); }

                    byte[] depress = www.bytes;

                    string contentEncoding = null;                    
                    if (www.responseHeaders.TryGetValue("CONTENT-ENCODING", out contentEncoding))
                    {
                        if(contentEncoding.ToUpper() == "GZIP") depress = Decode(www.bytes);
                    }

                    string crc = null;
                    www.responseHeaders.TryGetValue("CONTENT-MD5", out crc);  

                    if (crc == null || (crc == Util.Crypt.GetMD5_32(depress)))
                    {
                        string sId = string.Empty;
                        www.responseHeaders.TryGetValue("sId", out sId);
                        if (!string.IsNullOrEmpty(sId)) { session = sId; }
                        else if (www.text == "no session")
                        {
                            onCommunicateFailed.Fire(www.text);
                            return;
                        }
                        ResovleProto(depress);
                    }
                    else
                    {
                        if (!IsFilter(Rpc.GetType())) { onCommunicateFailed.Fire("data corrupt"); }
                    }
                }
                catch
                {
                    if (!IsFilter(Rpc.GetType())) { onCommunicateFailed.Fire("data corrupt"); }
                }
            }
        }
        byte[] Decode(byte[] data)
        {
            const int segment_length = 4096;
            var gzi = new GZipInputStream(new System.IO.MemoryStream(data));
            System.IO.MemoryStream re = new System.IO.MemoryStream();
            int count = 0;
            byte[] buffer = new byte[segment_length];
            while ((count = gzi.Read(buffer, 0, data.Length)) != 0)
            {
                re.Write(buffer, 0, count);
            }
            return re.ToArray();
        }
    }
}