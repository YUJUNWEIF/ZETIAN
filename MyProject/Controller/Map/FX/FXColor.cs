using UnityEngine;
using System.Collections.Generic;

public class FXColor : MonoBehaviour
{
    Animation anim;
    List<string> m_activeClips = new List<string>();
    string m_animName;
    const string propName = "_ColorChange";
    void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim == null) { anim = gameObject.AddComponent<Animation>(); }
    }
    void OnDestroy()
    {
        m_activeClips.ForEach(current => anim.RemoveClip(current));
        m_activeClips.Clear();
    }
    public void Change(Color src, Color dst, float loopTime)
    {
        PlayAnimation(src, dst, loopTime, @"_Change" + propName, WrapMode.Loop);
    }
    public void Flash()
    {
        PlayAnimation(Color.white, new Color(4f, 4f, 4f, 1f), 0.2f, @"_Flash" + propName, WrapMode.Loop);
    }
    public void ChangeOff() { StopAnimation(@"_Change" + propName); }
    public void FlashOff() { StopAnimation(@"_Flash" + propName); }
    void PlayAnimation(Color src, Color dst, float t, string name, WrapMode mode)
    {
        if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null && GetComponent<Renderer>().material.HasProperty(propName))
        {
            StopAnimation(m_animName);

            m_animName = name;
            if (anim.GetClip(m_animName) == null)
            {
                var clip = new AnimationClip();
                Keyframe[] rkf = new Keyframe[] { new Keyframe(0f, src.r, 0f, 0f), new Keyframe(t / 2f, dst.r, 0f, 0f), new Keyframe(t, src.r, 0f, 0f) };
                clip.SetCurve("", typeof(Material), propName + ".r", new AnimationCurve(rkf));
                Keyframe[] gkf = new Keyframe[] { new Keyframe(0f, src.g, 0f, 0f), new Keyframe(t / 2f, dst.g, 0f, 0f), new Keyframe(t, src.g, 0f, 0f) };
                clip.SetCurve("", typeof(Material), propName + ".g", new AnimationCurve(gkf));
                Keyframe[] bkf = new Keyframe[] { new Keyframe(0f, src.b, 0f, 0f), new Keyframe(t / 2f, dst.b, 0f, 0f), new Keyframe(t, src.b, 0f, 0f) };
                clip.SetCurve("", typeof(Material), propName + ".b", new AnimationCurve(bkf));
                clip.wrapMode = mode;
                anim.AddClip(clip, m_animName);
            }
            anim.Play(m_animName);
            m_activeClips.Add(m_animName);
        }
    }
    void StopAnimation(string name)
    {
        if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null && GetComponent<Renderer>().material.HasProperty(propName))
        {
            GetComponent<Renderer>().material.SetColor(propName, Color.white);
            if (anim.IsPlaying(name)) { anim.Stop(name); }
            anim.RemoveClip(name);
            m_activeClips.Remove(name);
        }
    }
}