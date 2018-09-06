using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Net;
using geniusbaby.pps;
using HTMLEngine.UGUI;
using Net;
using Util;

namespace geniusbaby.LSharpScript
{
    public class WordDictFrame : ILSharpScript
    {
//generate code begin
        public InputField Basic_input;
        public Button Basic_search;
        public UGUIDemo Basic_Clip_content;
        public LSharpList Basic_similar;
        public Button close;
        void __LoadComponet(Transform transform)
        {
            Basic_input = transform.Find("Basic/@input").GetComponent<InputField>();
            Basic_search = transform.Find("Basic/@search").GetComponent<Button>();
            Basic_Clip_content = transform.Find("Basic/Clip/@content").GetComponent<UGUIDemo>();
            Basic_similar = transform.Find("Basic/@similar").GetComponent<LSharpList>();
            close = transform.Find("@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            Basic_similar.OnInitialize();
        }
        void __DoUninit()
        {
            Basic_similar.OnUnInitialize();
        }
        void __DoShow()
        {
            Basic_similar.OnShow();
        }
        void __DoHide()
        {
            Basic_similar.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            Basic_similar.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordListItemPanel).Name);
            __DoInit();

            Basic_similar.OnInitialize();
            Basic_similar.selectListener.Add(() =>
            {
                var now = Basic_similar.itemSelected.First;
                if (now != null)
                {
                    Util.UnityHelper.Hide(Basic_similar);
                    Basic_input.text = (string)Basic_similar.values[now.Value];
                    Send(new WordQueryRequest() { queryType = 1, word = Basic_input.text });
                }
            });
            Basic_input.onValueChanged.AddListener(text =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    Send(new WordQueryRequest() { queryType = 2, word = text });
                    //var refWords = DictManager.Inst().FindSimilar(text);
                    //Util.UnityHelper.Show(Basic_similar);
                    //Basic_similar.SetValues(refWords);
                }
                else
                {
                    Util.UnityHelper.Hide(Basic_similar);
                }
            });
            Basic_search.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(Basic_input.text))
                {
                    Util.UnityHelper.Hide(Basic_similar);
                    Send(new WordQueryRequest() { queryType = 1, word = Basic_input.text });
                    //Basic_Clip_content.html = DictManager.Inst().Find(Basic_input.text);
                }
            });
            close.onClick.AddListener(() => GuiManager.Inst().HideFrame(api.name));
            Basic_Clip_content.linkRefClick = (txt) => TtsSpeak.Inst().Speak(txt);
            Basic_Clip_content.spriteGetter = (name) => { return null; };
            Basic_Clip_content.fontGetter = (name) => { return null; };

            SYNC = (ushort)(Framework.rand.NextUInt() & 0xFFFF);
        }

        KcpClient<UxpOnUdp> kcp = new KcpClient<UxpOnUdp>();
        Refresher m_kcpRingRefresher = new Refresher(1000, 0);
        INetwork m_network;
        ushort SYNC = 0;
        
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            Basic_input.text = string.Empty;
            EnglishDictModule.Inst().onQuery.Add(OnQuery);
            StartKcp(GamePath.url.udpAddr, OnAbnormal);
        }
        public override void OnHide()
        {
            Stop();
            EnglishDictModule.Inst().onQuery.Rmv(OnQuery);
            __DoHide();
            base.OnHide();
        }
        public void Display(string word)
        {
            if (word != Basic_input.text)
            {
                Basic_input.text = word;
                Basic_search.onClick.Invoke();
            }
        }
        void OnQuery()
        {
            var proto = EnglishDictModule.Inst().query;
            switch (proto.queryType)
            {
                case 1:
                    Basic_similar.gameObject.SetActive(false);
                    Basic_Clip_content.html = proto.translation;
                    break;
                case 2:
                    Basic_similar.gameObject.SetActive(true);
                    Basic_similar.SetValues(proto.similars.ConvertAll(it => (object)it));
                    break;
            }
        }
        void OnAbnormal()
        {
            Stop();
            StartKcp(GamePath.url.udpAddr, OnAbnormal);
        }
        void Send(object proto)
        {
            if (m_network != null) { m_network.Send(proto); }
        }
        void StartKcp(IpAddr ipAddr, Action onAbnormal)
        {
            var addr = new IPEndPoint(IPAddress.Parse(ipAddr.ip), ipAddr.port);
            var onUdp = new UxpOnUdp(++SYNC, GamePath.net.keepAlive * 1000, TimerManager.Inst().RealTimeMS());
            kcp.Start(onUdp, addr, onAbnormal);
            m_network = kcp;
            Util.TimerManager.Inst().onFrameUpdate.Add(OnUpdate);
        }
        void Stop()
        {
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
