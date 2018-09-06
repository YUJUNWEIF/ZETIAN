using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace geniusbaby
{
    public class SpeechCachePlay
    {
        const int audioLength = 10;
        IntPtr m_decoder;
        AudioSource m_source;
        Bf bf = new Bf();
        Util.Coroutine m_playing;
        Util.CoroutineHelper m_coroutineHelper = new Util.CoroutineHelper();
        public AudioClip clip { get; private set; }
        public int totalSamples { get; private set; }
        public SpeechCachePlay(IntPtr decoder, AudioSource source)
        {
            m_decoder = decoder;
            m_source = source;
            clip = AudioClip.Create("cc", audioLength * AMRCodec.SAMPLE_RATE, 1, AMRCodec.SAMPLE_RATE, false);
        }
        //public void Play(ChatData data)
        //{
        //    Decompress(data, bf);
        //    Stop();
        //    m_playing = m_coroutineHelper.StartCoroutineImmediate(Play());
        //}
        //void Decompress(ChatData ad, Bf bf)
        //{
        //    AMRCodec.DecoderStart(m_decoder, DecodeCallback);
        //    int offset = 0;
        //    while (true)
        //    {
        //        var length = ad.GetData(bf.cpp, offset, Bf.MaxLength);
        //        if (length <= 0) { break; }
        //        AMRCodec.DecoderInput(m_decoder, bf.cpp, length);
        //        offset += length;
        //    }
        //    AMRCodec.DecoderStop(m_decoder);
        //}
        public void Stop()
        {
            if (m_playing != null)
            {
                m_playing.Kill();
                m_playing = null;
            }
            m_source.Stop();
        }
        float[] fBuffer = new float[320];
        void DecodeCallback(int frameCounter, IntPtr buffer, int bufferSize)
        {
            if (frameCounter == 0)
            {
                totalSamples = 0;
            }
            if (frameCounter >= 0)
            {
                var len = bufferSize / sizeof(short);
                Marshal.Copy(buffer, bf.sBuffer, 0, len);
                for (int index = 0; index < len; ++index)
                {
                    fBuffer[index] = bf.sBuffer[index] / 32768f;
                }
                clip.SetData(fBuffer, totalSamples);
                totalSamples += len;
            }
            else
            {
            }
        }
        IEnumerator Play()
        {
            Framework.Instance.bgmSource.mute = true;
            m_source.clip = clip;
            m_source.mute = false;
            m_source.loop = false;
            m_source.Play();
            while (m_source.isPlaying && m_source.timeSamples < totalSamples)
            {
                yield return null;
            }
            Stop();
            Framework.Instance.bgmSource.mute = false;
        }
    }
}
