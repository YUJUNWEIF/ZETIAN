using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace geniusbaby.ui
{
    public class SpeechRealTimeRecord : MonoBehaviour
    {
        const int audioLength = 5;
        const float delayTime = 0.1f;

        public Button r;
        public Button p;

        public Button startRecord;
        public Button stopRecord;
        
        public AudioClip clip;

        private bool m_hasDevice;

        float fromtime;
        bool recording;
        int offsetSamples;
        int totalSamples;
        Util.CoroutineHelper coroutineHelper = new Util.CoroutineHelper();

        private void Awake()
        {
            string[] mDevice = Microphone.devices;
            if (mDevice.Length > 0)
            {
                int minSampleRate = 0;
                int maxSampleRate = 0;
                Microphone.GetDeviceCaps(null, out minSampleRate, out maxSampleRate);
                m_hasDevice = AMRCodec.SAMPLE_RATE >= minSampleRate && AMRCodec.SAMPLE_RATE <= maxSampleRate;
            }

            r.onClick.AddListener(() => StartRecordAudio(RecordCallBack));
            p.onClick.AddListener(() => StopRecordAudio());
        }

        Util.LoopQueue<float> m_dbs = new Util.LoopQueue<float>(4 * 1024 * 1024);
        void RecordCallBack(float[] buffer, int length)
        {
            m_dbs.Enqueue(buffer, length);
        }
        public float GetRecordTime()
        {
            return Time.realtimeSinceStartup - fromtime;
        }
        Action<float[], int> callback;
        float[] frameBuffer = new float[320];
        AudioClip recordClip;
        
        public void StartRecordAudio(Action<float[], int> callback)
        {
            if (!m_hasDevice) { return; }
            if (Microphone.IsRecording(null)) { return; }
            coroutineHelper.StartCoroutineImmediate(Record());

            m_dbs.Clear();


            if (!m_hasDevice) { return; }
        }
        public void StopRecordAudio()
        {
            if (!m_hasDevice) { return; }
            recording = false;
            totalSamples = (int)(GetRecordTime() * AMRCodec.SAMPLE_RATE);
            if (!Microphone.IsRecording(null))
                return;
            Microphone.End(null);
            //PlayRecordAudio(totalSamples);
        }
        IEnumerator Record()
        {
            recording = true;
            fromtime = Time.realtimeSinceStartup;

            recordClip = Microphone.Start(null, false, audioLength, AMRCodec.SAMPLE_RATE);
            offsetSamples = 0;
            while (recording)
            {
                 var lastPos = Microphone.GetPosition(null);
                if (Microphone.IsRecording(null))
                {
                    int length = lastPos / AMRCodec.SAMPLE_RATE;
                }
                else
                {
                    int length = audioLength / AMRCodec.SAMPLE_RATE;
                }
                yield return null;
            }
        }
        public void FixedUpdate()
        {
            int now = totalSamples;
            if (recording)
            {
                float to = (Time.realtimeSinceStartup - delayTime);
                now = GetSamples(to - fromtime);
            }

            var length = now - offsetSamples;
            while (length > 0)
            {
                var fragment = length;
                if (fragment > frameBuffer.Length / clip.channels)
                {
                    fragment = frameBuffer.Length / clip.channels;
                }
                length -= fragment;

                int bs = ClipBufferSamples();
                if (offsetSamples + fragment < bs)
                {
                    clip.GetData(frameBuffer, offsetSamples);
                    callback(frameBuffer, fragment * clip.channels);

                    offsetSamples += fragment;
                }
                else
                {
                    int first = (bs - offsetSamples);
                    int second = fragment - first;

                    clip.GetData(frameBuffer, offsetSamples);
                    callback(frameBuffer, first * clip.channels);

                    clip.GetData(frameBuffer, 0);
                    callback(frameBuffer, second * clip.channels);

                    offsetSamples = (offsetSamples + fragment) % bs;
                }
            }
        }
        int ClipBufferSamples()
        {
            return audioLength * AMRCodec.SAMPLE_RATE;
        }
        int GetSamples(float time)
        {
            if (time > 0f)
            {
                return (int)(time * AMRCodec.SAMPLE_RATE);
            }
            return 0;
        }
    }
}
