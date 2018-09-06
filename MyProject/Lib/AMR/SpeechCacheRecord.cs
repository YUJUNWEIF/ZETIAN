using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace geniusbaby
{
    public class SpeechCacheRecord
    {
        const int audioLength = 10;
        bool m_hasDevice;
        IntPtr m_encoder;
        AudioClip m_clip;
        Action<IClipVisit, Bf> m_callback;
        ClipVisit cd = new ClipVisit();
        Bf bf = new Bf();
        float[] m_trimBuffer = new float[10 * AMRCodec.SAMPLE_RATE];
        int sampleRate = AMRCodec.SAMPLE_RATE;
        public SpeechCacheRecord(IntPtr encoder)
        {
            m_encoder = encoder;

            string[] mDevice = Microphone.devices;
            m_hasDevice = mDevice.Length > 0;
            if (m_hasDevice)
            {
                int minSampleRate = 0;
                int maxSampleRate = 0;
                Microphone.GetDeviceCaps(null, out minSampleRate, out maxSampleRate);
                if (sampleRate > maxSampleRate) sampleRate = maxSampleRate;
            }
        }        
        public void StartRecordAudio(Action<IClipVisit, Bf> callback)
        {
            if (!m_hasDevice) { return; }
            if (Microphone.IsRecording(null)) { return; }
            m_callback = callback;
            m_clip = Microphone.Start(null, false, audioLength, sampleRate);
        }
        public void StopRecordAudio()
        {
            if (!m_hasDevice || !Microphone.IsRecording(null)) { return; }
            int totalSamples = Microphone.GetPosition(null);
            Microphone.End(null);
            const float zero = 1f / short.MaxValue;
            if (m_clip.GetData(m_trimBuffer, 0))
            {
                if (totalSamples * m_clip.channels > m_trimBuffer.Length)
                {
                    Util.Logger.Instance.Error("fuck why ??");
                    totalSamples = m_trimBuffer.Length / m_clip.channels;
                }
                var cb = new ClipBufferVisit();
                int start = 0;
                int end = totalSamples - 1;
                while (start < totalSamples)
                {
                    float v = 0f;
                    for (int c = 0; c < m_clip.channels; ++c)
                    {
                        v += m_trimBuffer[start * m_clip.channels + c];
                    }
                    if ( Mathf.Abs( v) >= m_clip.channels * zero) { break; }
                    ++start;
                }
                while (end > start)
                {
                    float v = 0f;
                    for (int c = 0; c < m_clip.channels; ++c)
                    {
                        v += m_trimBuffer[end * m_clip.channels + c];
                    }
                    if (Mathf.Abs(v) >= m_clip.channels * zero) { break; }
                    --end;
                }

                if (end > start + sampleRate)//valid data need great than 1 sec
                {
                    cb.SetData(m_trimBuffer, sampleRate, m_clip.channels, start, (end - start) + 1);
                    m_callback(cb, bf);
                }
            }
        }
        public void TestR(Action<IClipVisit, Bf> callback, AudioClip clip, int totalSamples)
        {
            m_callback = callback;
            cd.SetData(clip, 0, totalSamples);
            m_callback(cd, bf);
        }
        //void Compress(IClipVisit cv, Bf bf)
        //{
        //    AMRCodec.EncoderStart(m_encoder, m_callback);
        //    int offsetSamples = 0;
        //    while (true)
        //    {
        //        var samples = cv.ResampleC1I16(bf, AMRCodec.SAMPLE_RATE, offsetSamples);
        //        if (samples <= 0) { break; }
        //        offsetSamples += samples;

        //        Marshal.Copy(bf.sBuffer, 0, bf.cpp, samples);
        //        AMRCodec.EncoderInput(m_encoder, bf.cpp, samples * sizeof(short));
        //    }
        //    AMRCodec.EncoderStop(m_encoder);
        //}
    }
}
