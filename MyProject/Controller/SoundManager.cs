using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace geniusbaby
{
    public class SoundManager : Singleton<SoundManager>
    {
        class QueuedClip
        {
            public AudioSource source;
            public string clipName;
            public float endAt;
        }
        List<AudioSource> m_audios = new List<AudioSource>();
        LinkedList<QueuedClip> m_queuedClips = new LinkedList<QueuedClip>();
        const int maxQueueClip = 5;
        const int maxAdditiveClip = 2;

        public void StartGame(GameObject binder)
        {
            var audios = binder.GetComponents<AudioSource>();
            for (int index = 0; index < maxQueueClip; ++index)
            {
                if (index < audios.Length)
                {
                    m_audios.Add(audios[index]);
                }
                else
                {
                    m_audios.Add(binder.AddComponent<AudioSource>());
                }
            }

            Util.TimerManager.Instance.Add(OnUpdate, 100);
            Framework.Inst().onSetting.Add(OnSetting);
            OnSetting();
        }

        public void StopGame()
        {
            Util.TimerManager.Instance.Remove(OnUpdate);
            Framework.Inst().onSetting.Rmv(OnSetting);
        }
        public AudioClip PlaySound(string name, bool oneShoot = true)
        {
            AudioClip clip;
            clip = BundleManager.Instance.LoadSync<AudioClip>(GamePath.asset.audio, name);
            PlaySound(clip, oneShoot);
            return clip;
        }
        public void PlaySound(AudioClip clip, bool oneShoot = true)
        {
            if (m_queuedClips.Count > 0)
            {
                var no = m_queuedClips.First;
                while (no != null)
                {
                    no.Value.source.Stop();
                    no = no.Next;
                }
                m_queuedClips.Clear();
            }

            var setting = Framework.Inst().setting;
            var volumn = (float)FEFixed.InnerNew(setting.sound);
            if (clip != null)
            {
                QueuedClip queuedClip = new QueuedClip();
                queuedClip.clipName = clip.name;
                queuedClip.endAt = Time.time + clip.length;
                queuedClip.source = m_audios[0];
                if (oneShoot) { queuedClip.source.PlayOneShot(clip, volumn); }
                else
                {
                    queuedClip.source.clip = clip;
                    queuedClip.source.volume = volumn;
                    queuedClip.source.Play();
                }
            }
        }

        public void PlayQueuedSound(AudioClip clip, bool oneShoot = true)
        {
            if (!clip || GetAdditive(clip) >= maxAdditiveClip) { return; }

            QueuedClip queuedClip = new QueuedClip();
            queuedClip.clipName = clip.name;
            queuedClip.endAt = Time.time + clip.length;
            if (m_queuedClips.Count >= maxQueueClip)
            {
                float endAt = -1;
                var no = m_queuedClips.First;
                while (no != null)
                {
                    if (queuedClip.source == null || no.Value.endAt < endAt)
                    {
                        queuedClip.source = no.Value.source;
                        endAt = no.Value.endAt;
                    }
                    no = no.Next;
                }
                queuedClip.source.Stop();
            }
            else
            {
                for (int index = 0; index < m_audios.Count; ++index)
                {
                    bool exist = false;
                    var no = m_queuedClips.First;
                    while (no != null)
                    {
                        if (exist = (no.Value.source == m_audios[index])) { break; }
                        no = no.Next;
                    }
                    if (!exist)
                    {
                        queuedClip.source = m_audios[index];
                    }
                }
            }

            var setting = Framework.Inst().setting;
            var volumn = (float)FEFixed.InnerNew(setting.sound);
            if (oneShoot) { queuedClip.source.PlayOneShot(clip, volumn); }
            else
            {
                queuedClip.source.clip = clip;
                queuedClip.source.volume = volumn;
                queuedClip.source.Play();
            }
            m_queuedClips.AddLast(queuedClip);
        }
        void OnUpdate()
        {
            var no = m_queuedClips.First;
            while (no != null)
            {
                var tmp = no;
                no = no.Next;
                if (Time.time >= tmp.Value.endAt)
                {
                    tmp.Value.source.Stop();
                    m_queuedClips.Remove(tmp);
                }
            }
        }
        void OnSetting()
        {
            var setting = Framework.Inst().setting;
            for (int index = 0; index < m_audios.Count; ++index)
            {
                m_audios[index].volume = (float)FEFixed.InnerNew(setting.sound);
            }
        }
        int GetAdditive(AudioClip clip)
        {
            int additive = 0;
            var no = m_queuedClips.First;
            while (no != null)
            {
                if (no.Value.clipName == clip.name) { ++additive; }
                no = no.Next;
            }
            return additive;
        }
    }
}