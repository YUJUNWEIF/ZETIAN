using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace geniusbaby.ui
{
    class ClipLooper
    {
        AudioClip createClip;
        public ClipLooper(AudioClip clip)
        {
            m_capcity = clip.samples - 1;
            Clear();
        }
        public void Clear()
        {
            m_front = 0;
            m_rear = 0;
        }
        public int Discard(int count)
        {
            if (count > Count()) { count = Count(); }
            m_front += count;
            return count;
        }
        public int Enqueue(float[] buffer, int count)
        {
            int samples = count / createClip.channels;
            int free = Capcity() - Count();
            if (samples > free) { samples = free; }
            int start = m_rear % m_capcity;
            int end = (m_rear + samples) % m_capcity;
            if (start < end)
            {
                createClip.SetData(buffer, start);
            }
            else
            {
                samples = m_capcity - start;
                createClip.SetData(buffer, start);
            }
            m_rear += samples;
            return samples;
        }
        public int Count() { return m_rear - m_front; }
        public int Free() { return Capcity() - Count(); }
        public int Capcity() { return m_capcity - 1; }
        public bool Full()
        {
            return m_front % m_capcity == (m_rear + 1) % m_capcity;    //判断循环链表是否满，留一个预留空间不用
        }
        public bool Empty()
        {
            return (m_front == m_rear);    //判断是否为空
        }
        private int m_capcity;
        private int m_front;    //指向队列第一个元素  
        private int m_rear;    //指向队列最后一个元素的下一个元素
    }
    public class SpeechRealTimePlay
    {
        const int audioLength = 5;      
        public AudioSource source;        
        ClipLooper clipLooper;
        AudioClip createClip;
        Util.LoopQueue<float> m_dbs;
        int pos = 0;
        int lastPlayAt = 0;
        int lastSamples = 0;
        bool pause = false;
        float[] temp = new float[320];
        float[] temp_ = new float[320];

        private void Awake()
        {
            createClip = AudioClip.Create("cc", audioLength, 1, AMRCodec.SAMPLE_RATE, true, PCMReadCallBack);
            clipLooper = new ClipLooper(createClip);
        }
        public void Start(Util.LoopQueue<float> dbs)
        {
            m_dbs = dbs;

            source.Stop();
            source.clip = createClip;
            source.mute = false;
            source.loop = true;
            source.Play();
            pos = 0;
            lastPlayAt = 0;
        }        
        public void Stop()
        {
            source.Stop();
        }
        void PCMReadCallBack(float[] data)
        {

        }
        void Update()
        {
            if (source.isPlaying)
            {
                while (true)
                {
                    var free = clipLooper.Free();
                    if (free > m_dbs.Count)
                    {
                        free = m_dbs.Count;
                    }
                    if (free <= 0) { break; }
                    int actualCount = m_dbs.Dequeue(temp, temp.Length);

                    int c = clipLooper.Enqueue(temp, actualCount);
                    if (c < actualCount)
                    {
                        Array.Copy(temp, c, temp_, 0, actualCount - c);
                    }
                }

                PP();
            }
        }
        void PP()
        {
            int passed = source.timeSamples - lastSamples;
            if (passed < 0) { passed += createClip.samples; }
            lastSamples = source.timeSamples;
            lastPlayAt += lastSamples;

            if (lastPlayAt > pos)
            {
                source.Pause();
            }
            else
            {
                source.UnPause();
            }
        }
    }
}
