using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using geniusbaby;

public class SkinAnimation
{
    Transform m_transform;
    Dictionary<string, Transform> m_boneMap = new Dictionary<string, Transform>();
    List<AnimationState> m_clips = new List<AnimationState>();
    string m_currentAnim = string.Empty;
    Animation m_animation;
    public void FinishWith(string before, string after, float speed = 1f)
    {
        m_currentAnim = before;
        
        AnimationState state = m_animation[before];
        if (state)
        {
            state.speed = speed;
            m_animation.CrossFade(before, 0.2f, PlayMode.StopSameLayer);

            if (state.wrapMode != WrapMode.Loop && state.wrapMode != WrapMode.PingPong)
            {
                state = m_animation[after];
                if (state)
                {
                    state.speed = speed;
                    m_animation.CrossFadeQueued(after, 0.2f, QueueMode.CompleteOthers, PlayMode.StopSameLayer);
                }
            }
        }
        else
        {
            state = m_animation[after];
            if (state)
            {
                state.speed = speed;
                m_animation.CrossFade(after, 0.2f, PlayMode.StopSameLayer);
            }
        }
    }
    public void Play(string clipName, float speed)
    {
        AnimationState state = m_animation[clipName];
        if (state == null) { return; }
        state.speed = speed;
        //state.time = state.length * animStartAtPercent;
        if (!m_animation.isPlaying || m_currentAnim != clipName)
        {
            if (m_animation.clip)
            {
                if (m_animation.clip.wrapMode == WrapMode.Loop || m_animation.clip.wrapMode == WrapMode.PingPong) Stop();
            }
            m_currentAnim = clipName;
            m_animation.CrossFade(clipName, 0.2f, PlayMode.StopSameLayer);
        }
    }
    public void Stop()
    {
        if (!string.IsNullOrEmpty(m_currentAnim))
        {
            m_animation.Stop(m_currentAnim);
            m_currentAnim = string.Empty;
        }
    }
    public void FreezeAnimation()
    {
        Play(m_currentAnim, 0f);
    }
    public void UnfreezeAnimation()
    {
        Play(m_currentAnim, 1f);
    }
    public void Blend(string clipName)
    {
        m_animation.Blend(clipName, 2f);
    }
    public void PlayQueued(string clipName, float speed = 1f)
    {
        AnimationState state = m_animation[clipName];
        if (state != null)
        {
            state.speed = speed;
            m_animation.PlayQueued(clipName, QueueMode.CompleteOthers);
        }
    }
    public string GetActiveAnimation() { return m_currentAnim; }
    public float GetAnimationLength(string clipName)
    {
        AnimationState state = m_animation[clipName];
        return (state != null) ? state.length : 0f;
    }
    public WrapMode GetAnimationWrapMode(string clipName)
    {
        AnimationState state = m_animation[clipName];
        return (state != null) ? state.clip.wrapMode : WrapMode.Default;
    }
    public bool IsPlaying(string clipName) { return m_animation.IsPlaying(clipName); }

    public void SetVisibleData(Transform root, AnimationClip[] clips, AnimationCullingType cullingType = AnimationCullingType.BasedOnRenderers)
    {
        m_transform = root;
        m_animation = m_transform.GetComponent<Animation>();
        if (m_animation == null)
        {
            m_animation = m_transform.gameObject.AddComponent<Animation>();
        }
        m_animation.cullingType = cullingType;
        if (clips != null)
        {
            for (int animType = 0; animType < clips.Length; ++animType)
            {
                var clip = clips[animType];
                if (clip)
                {
                    if (!m_animation.GetClip(clip.name)) { m_animation.AddClip(clip, clip.name); }
                }
            }
        }
    }
    public Transform GetBone(string name)
    {
        Transform bone;
        if (!m_boneMap.TryGetValue(name, out bone))
        {
            bone = Util.UnityHelper.FindChild(m_transform, name);
            if (bone != null) { m_boneMap.Add(name, bone); }
        }
        return bone;
    }
}

