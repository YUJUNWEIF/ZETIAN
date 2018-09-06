using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class GMapChatFrame : ILSharpScript
    {
        class CustomFactory : ICustomFactory<LSharpItemPanel, object>
        {
            LinkedList<LSharpItemPanel> m_caches = new LinkedList<LSharpItemPanel>();
            IListBase m_list;
            public CustomFactory(int w, int h)
            {
                cWidth = w;
                cHeight = h;
            }
            public void OnInitialize(IListBase list) { m_list = list; }
            public void OnUnInitialize() { }
            public void DeleteItem(LSharpItemPanel item)
            {
                Util.UnityHelper.Hide(item);
                Util.UnityHelper.CallUnInitialize(item);
                m_caches.AddLast(item);
            }
            public LSharpItemPanel NewItem(object value)
            {
                var now = (IGMapChatData)value;
                LSharpItemPanel it = null;
                var no = m_caches.First;
                while (no != null)
                {
                    var old = (IGMapChatData)no.Value.AttachValue;
                    if (old.type == now.type)
                    {
                        it = no.Value;
                        m_caches.Remove(no);
                        break;
                    }
                    no = no.Next;
                }
                if (!it)
                {
                    switch (now.type)
                    {
                        case ChatType.Text: it = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(ChatTextItemPanel).Name); break;
                        case ChatType.Audio: it = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(ChatAudioItemPanel).Name); break;
                        case ChatType.Multi: break;
                        default: return null;
                    }
                }
                it.ListComponent = m_list;
                Util.UnityHelper.CallInitialize(it);
                Util.UnityHelper.Show(it, m_list.transform);
                return it;
            }
            public int cWidth { get; private set; }
            public int cHeight { get; private set; }
        }
//generate code begin
        public LSharpListSuper BG_clip_list;
        public DragGridCell BG_talk;
        public InputField BG_input;
        public Button BG_send;
        public Button BG_close;
        void __LoadComponet(Transform transform)
        {
            BG_clip_list = transform.Find("BG/clip/@list").GetComponent<LSharpListSuper>();
            BG_talk = transform.Find("BG/@talk").GetComponent<DragGridCell>();
            BG_input = transform.Find("BG/@input").GetComponent<InputField>();
            BG_send = transform.Find("BG/@send").GetComponent<Button>();
            BG_close = transform.Find("BG/@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            BG_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_clip_list.OnShow();
        }
        void __DoHide()
        {
            BG_clip_list.OnHide();
        }
//generate code end
        bool m_needAutoRoll;
        public int height = 50;
        SpeechCacheRecord soundRecorder;
        SpeechCachePlay soundPlayer;
        IntPtr m_encoder;
        IntPtr m_decoder;
        SoundTouch m_tuner;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            var lister = (RectTransform)BG_clip_list.transform;
            var logic = new CustomSuperLogic<LSharpItemPanel, object>();
            var factory = new CustomFactory((int)lister.rect.width, height);
            new VerticalSuperDisplayImpl<LSharpItemPanel, object>().Attach(logic);
            BG_clip_list.SetCustomDisplayLogic(logic, factory);
            __DoInit();

            m_encoder = AMRCodec.EncoderInit();
            m_decoder = AMRCodec.DecoderInit();
            m_tuner = new SoundTouch();
            m_tuner.Initialize();

            var source = api.GetComponent<AudioSource>();
            soundRecorder = new SpeechCacheRecord(m_encoder);
            soundPlayer = new SpeechCachePlay(m_decoder, source);
            ChatManager.Inst().soundPlayer = soundPlayer;

            BG_clip_list.OnInitialize();

            BG_send.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(BG_input.text))
                {
                    var result = BG_input.text.TrimStart(' ', '\t').TrimEnd(' ', '\t');
                    BG_input.text = string.Empty;
                    ChatManager.Inst().Send(new pps.GMapChatTextSendReport() { reciverSId = -1, msg = result });
                }
            });
            BG_close.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
            });
            BG_talk.onPress.Add(ev =>
            {
                AudioListener.pause = true;
                soundRecorder.StartRecordAudio(RecordResult);
                //soundRecorder.TestR(RecordResult, testClip, 2 * testClip.frequency);
            });
            BG_talk.onRelease.Add(ev =>
            {
                AudioListener.pause = false;
                soundRecorder.StopRecordAudio();
            });
        }
        public override void OnUnInitialize()
        {
            __DoUninit();
            m_tuner.UnInitialize();
            AMRCodec.EncoderUnInit(m_encoder);
            AMRCodec.DecoderUnInit(m_decoder);
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            ChatManager.Instance.onAdd.Add(OnChatAddEvent);
            ChatManager.Instance.onRmv.Add(OnChatAddEvent);
            OnChannelEvent();
        }
        public override void OnHide()
        {
            ChatManager.Instance.onAdd.Rmv(OnChatAddEvent);
            ChatManager.Instance.onRmv.Rmv(OnChatAddEvent);
            __DoHide();
            base.OnHide();
        }
        void RecordResult(IClipVisit cv, Bf bf)
        {
            AMRCodec.EncoderStart(m_encoder, EncodeCallback);
            int offsetSamples = 0;
            while (true)
            {
                var samples = cv.ResampleC1I16(bf, AMRCodec.SAMPLE_RATE, offsetSamples);
                if (samples <= 0) { break; }
                offsetSamples += samples;

                Marshal.Copy(bf.sBuffer, 0, bf.cpp, samples);
                AMRCodec.EncoderInput(m_encoder, bf.cpp, samples * sizeof(short));
            }
            AMRCodec.EncoderStop(m_encoder);
        }
        //void RecordAndTuneResult(IClipVisit cv, Bf bf)
        //{
        //    AMRCodec.EncoderStart(m_encoder, EncodeCallback);
        //    m_tuner.Start(cv.sampleRate, 1, m_default, (result, samples) =>
        //    {
        //        Marshal.Copy(result, 0, bf.cpp, samples);
        //        AMRCodec.EncoderInput(m_encoder, bf.cpp, samples * sizeof(short));
        //    });
        //    int offsetSamples = 0;
        //    while (true)
        //    {
        //        var samples = cv.ResampleC1I16(bf, AMRCodec.SAMPLE_RATE, offsetSamples);
        //        if (samples <= 0) { break; }
        //        offsetSamples += samples;
        //        m_tuner.Input(bf.sBuffer, samples, bf);
        //    }
        //    m_tuner.Stop(bf);
        //    AMRCodec.EncoderStop(m_encoder);
        //}
        int m_bufferPos = 0;
        byte[] m_buffer = new byte[500 * sizeof(short)];
        void EncodeCallback(int frameCounter, IntPtr buffer, int bufferSize)
        {
            if (!ChatManager.Inst().isOnline) { return; }
            if (frameCounter == 0)
            {
                m_bufferPos = 0;
            }
            if (frameCounter >= 0)
            {
                if (m_bufferPos + bufferSize < m_buffer.Length)
                {
                    Marshal.Copy(buffer, m_buffer, m_bufferPos, bufferSize);
                    m_bufferPos += bufferSize;
                }
                else
                {
                    SendSeg();
                    Marshal.Copy(buffer, m_buffer, m_bufferPos, bufferSize);
                    m_bufferPos += bufferSize;
                }
            }
            else
            {
                SendSeg();
                ChatManager.Inst().Send(new pps.GMapChatAudioSendReport() { reciverSId = -1, data = null });
            }
        }
        void SendSeg()
        {
            if (m_bufferPos > 0)
            {
                var sd = new byte[m_bufferPos];
                Array.Copy(m_buffer, 0, sd, 0, m_bufferPos);
                ChatManager.Inst().Send(new pps.GMapChatAudioSendReport() { reciverSId = -1, data = sd });
                m_bufferPos = 0;
            }
        }
        void OnUseEmotion(string emotionCmd)
        {
            BG_input.text += ChatModule.CodecEmotion(emotionCmd);
        }
        void OnChatAddEvent(IGMapChatData current)
        {
            BG_clip_list.SetValues(ChatManager.Instance.chats.ConvertAll(it => (object)it));
        }
        void OnChannelEvent()
        {
            BG_clip_list.SetValues(ChatManager.Inst().chats.ConvertAll(it => (object)it));
        }
    }
}
