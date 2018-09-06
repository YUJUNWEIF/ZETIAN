using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace geniusbaby
{
    public class MusicManager : Singleton<MusicManager>
    {
        Util.CoroutineHelper m_coroutineHelper = new Util.CoroutineHelper();
        Util.Coroutine m_crossFader;
        AudioSource bgmSource;
        public void StartGame()
        {
            bgmSource = Framework.Instance.bgmSource;

            Framework.Inst().onSetting.Add(OnSetting);
            OnSetting();
        }
        public void StopGame()
        {
            Framework.Inst().onSetting.Rmv(OnSetting);
        }
        public void PlayBGM(string name, Action<AudioClip> actualPlay = null)
        {
            BundleManager.Instance.LoadAsync(GamePath.asset.audio, name,
                obj =>
                {
                    var clip = obj as AudioClip;
                    if (bgmSource != null && clip != null)
                    {
                        bgmSource.clip = clip;
                        bgmSource.Play();
                    }
                    if (actualPlay != null) { actualPlay(clip); }
                });
        }
        public void CrossFadeTo(string name, float duration = 2f)
        {
            BundleManager.Instance.LoadAsync(GamePath.asset.audio, name,
                obj =>
                {
                    if (m_crossFader != null)
                    {
                        m_crossFader.Kill();
                    }
                    m_crossFader = m_coroutineHelper.StartCoroutine(CrossFadeTo(obj as AudioClip));
                });
        }
        IEnumerator CrossFadeTo(AudioClip clip, float duration = 1f)
        {
            var setting = Framework.Inst().setting;
            var volumn = (float)FEFixed.InnerNew(setting.music);

            var current = Time.time;
            if (bgmSource.clip)
            {
                while (Time.time - current < duration)
                {
                    bgmSource.volume = (1f - (Time.time - current) / duration) * volumn;
                    yield return null;
                }
            }
            bgmSource.clip = clip;
            bgmSource.Play();
            while (Time.time - current - duration < duration)
            {
                bgmSource.volume = ((Time.time - current) / duration - 1) * volumn;
                yield return null;
            }
            bgmSource.volume = volumn;
            m_crossFader = null;
        }
        void OnSetting()
        {
            var setting = Framework.Inst().setting;
            bgmSource.volume = (float)FEFixed.InnerNew(setting.music);
        }
    }
}