using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;



namespace geniusbaby.ui
{
    public class Tunne : MonoBehaviour
    {
        const int length = 30;
        const int MAX_BUFFSIZE = 1024 * 1024;

        public AudioSource source;
        public Slider tempoBar;
        public Slider rateBar;
        public Slider pitchBar;
        public Toggle aafilterTog;
        public Slider aafilterLengthBar;
        public Toggle quickSeekingTog;
        public Slider sequenceMSBar;
        public Slider seekWindowMSBar;
        public Slider overlapMsBar;

        public Button startRecord;
        public Button startRecordTune;
        public Button stopRecord;
        public Button play;


        public AudioSource soure;
        public AudioClip testClip;

        public InputField input;
        public Button speak;

        private bool m_hasDevice;
        SoundParam m_default = new SoundParam();

        void Restore()
        {
            tempoBar.value = m_default.tempoDelta.Percent;
            rateBar.value = m_default.rateDelta.Percent;
            pitchBar.value = m_default.pitchDelta.Percent;

            aafilterTog.isOn = m_default.antiAlias;
            aafilterLengthBar.value = (m_default.antiAliasLength / 8f - 1f) / 15f;
            quickSeekingTog.isOn = m_default.quickSeek;
            //sequenceMSBar.value = m_default.sequenceMS / 1000f;
            //seekWindowMSBar.value = m_default.seekWindowMS / 1000f;
            //overlapMsBar.value = m_default.overlapMs / 1000f;
        }
        IntPtr m_encoder;
        IntPtr m_decoder;
        SoundTouch m_tuner = new SoundTouch();
        ClipVisit cd = new ClipVisit();
        AMRVisit ad = new AMRVisit();
        SpeechCacheRecord soundRecorder;
        SpeechCachePlay soundPlayer;
        Bf bf = new Bf();
  
        public void Initialize()
        {
            m_encoder = AMRCodec.EncoderInit();
            m_decoder = AMRCodec.DecoderInit();
            m_tuner = new SoundTouch();
            m_tuner.Initialize();

            soundRecorder = new SpeechCacheRecord(m_encoder);
            soundPlayer = new SpeechCachePlay(m_decoder, source);
        }
        public void UnInitialize()
        {
            m_tuner.UnInitialize();
            AMRCodec.EncoderUnInit(m_encoder);
            AMRCodec.DecoderUnInit(m_decoder);
        }
        private void Awake()
        {
            Initialize();

            string[] mDevice = Microphone.devices;
            m_hasDevice = mDevice.Length > 0;

            Restore();
            tempoBar.onValueChanged.AddListener(v => m_default.tempoDelta.Lerp(v));
            rateBar.onValueChanged.AddListener(v => m_default.rateDelta.Lerp(v));
            pitchBar.onValueChanged.AddListener(v => m_default.pitchDelta.Lerp(v));

            aafilterTog.onValueChanged.AddListener(isOn =>
            {
                m_default.antiAlias = isOn;
            });
            aafilterLengthBar.onValueChanged.AddListener(v =>
            {
                //m_default.antiAliasLength = ((int)(v * 127 + 0.5f) / 8 + 1) * 8;
            });
            quickSeekingTog.onValueChanged.AddListener(isOn =>
            {
                //m_default.quickSeek = isOn;
            });
            sequenceMSBar.onValueChanged.AddListener(v => m_default.sequenceMS.Lerp(v));
            seekWindowMSBar.onValueChanged.AddListener(v => m_default.seekWindowMS.Lerp(v));
            overlapMsBar.onValueChanged.AddListener(v => m_default.overlapMs.Lerp(v));

            startRecord.onClick.AddListener(() =>
            {
                soundRecorder.StartRecordAudio(RecordResult);
                //soundRecorder.StartRecordAudio((cv, bf) =>
                //{
                //    SaveClip(bf, cv, cv.samples);
                //});
            });
            startRecordTune.onClick.AddListener(() =>
            {
                soundRecorder.StartRecordAudio(RecordAndTuneResult);
            });
            stopRecord.onClick.AddListener(() =>
            {
                soundRecorder.StopRecordAudio();
            });
            play.onClick.AddListener(() => Play());

            speak.onClick.AddListener(() => TtsSpeak.Inst().Speak(input.text));
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
        void RecordAndTuneResult(IClipVisit cv, Bf bf)
        {
            AMRCodec.EncoderStart(m_encoder, EncodeCallback);
            m_tuner.Start(cv.sampleRate, 1, m_default, (result, samples) =>
            {
                Marshal.Copy(result, 0, bf.cpp, samples);
                AMRCodec.EncoderInput(m_encoder, bf.cpp, samples * sizeof(short));
            });
            int offsetSamples = 0;
            while (true)
            {
                var samples = cv.ResampleC1I16(bf, AMRCodec.SAMPLE_RATE, offsetSamples);
                if (samples <= 0) { break; }
                offsetSamples += samples;
                m_tuner.Input(bf.sBuffer, samples, bf);
            }
            m_tuner.Stop(bf);
            AMRCodec.EncoderStop(m_encoder);
        }
        int m_bufferPos = 0;
        byte[] m_buffer = new byte[500 * sizeof(short)];
        ChatData m_chatData = new ChatData(0);
        void EncodeCallback(int frameCounter, IntPtr buffer, int bufferSize)
        {
            if (frameCounter == 0)
            {
                m_chatData.Clear();
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
            }
        }
        void SendSeg()
        {
            if (m_bufferPos > 0)
            {
                m_chatData.Fill(m_buffer, 0, m_bufferPos);
                m_bufferPos = 0;
            }
        }
        public void Play()
        {
            //soundPlayer.Play(m_chatData);
        }
        void SaveClip(Bf bf, IClipVisit cv, int samples)
        {
            if (samples > cv.samples) { samples = cv.samples; }
            var fs = File.OpenWrite("aaa.wav");
            int len = AMRCodec.WavWriteHeader(bf.cpp, cv.sampleRate, sizeof(short) * 8, cv.channels, samples * cv.channels * sizeof(short));
            Marshal.Copy(bf.cpp, bf.bBuffer, 0, len);
            fs.Write(bf.bBuffer, 0, len);

            var offsetSamples = 0;
            while (true)
            {
                len = cv.GetData(bf.fBuffer, offsetSamples);
                if (len <= 0) { break; }
                offsetSamples += len;

                for (int index = 0; index < len; ++index)
                {
                    var a = (short)(bf.fBuffer[index] * short.MaxValue);
                    bf.bBuffer[index * 2 + 0] = (byte)((a >> 0) & 0xFF);
                    bf.bBuffer[index * 2 + 1] = (byte)((a >> 8) & 0xFF);
                }
                fs.Write(bf.bBuffer, 0, len * 2);

                if (offsetSamples >= samples) { break; }
            }
            fs.Close();
        }
    }
}
