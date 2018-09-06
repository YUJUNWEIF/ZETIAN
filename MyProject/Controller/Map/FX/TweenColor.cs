using UnityEngine;
using System.Collections.Generic;

public class TweenColor : UITweener
{
    public Renderer render;
    void Awake()
    {
        if (!render)
        {
            render = GetComponentInChildren<Renderer>();
        }
    }
    void SetTweenTarget(string nodeName)
    {
        if (!string.IsNullOrEmpty(nodeName))
        {
            var node = transform.Find(nodeName);
            if (node) { render = node.GetComponent<Renderer>(); }
        }
    }
    public Color from;
    public Color to;
    static public TweenColor Begin(GameObject go, float duration, Color from, Color to, Renderer render)
    {
        TweenColor comp = go.GetComponent<TweenColor>();
        if (comp == null) { comp = go.AddComponent<TweenColor>(); }
        comp.render = render;
        comp.duration = duration;
        comp.from = from;
        comp.to = to;
        comp.Play();
        return comp;
    }
    protected override void OnStep(float factor)
    {
        if (render && render.material)
        {
            render.material.color = from * (1f - factor) + to * factor;
        }
    }
    //void PlayAnimation(Color src, Color dst, float t, string name, WrapMode mode)
    //{
    //    if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null && GetComponent<Renderer>().material.HasProperty(propName))
    //    {
    //        StopAnimation(m_animName);

    //        m_animName = name;
    //        if (anim.GetClip(m_animName) == null)
    //        {
    //            var clip = new AnimationClip();
    //            Keyframe[] rkf = new Keyframe[] { new Keyframe(0f, src.r, 0f, 0f), new Keyframe(t / 2f, dst.r, 0f, 0f), new Keyframe(t, src.r, 0f, 0f) };
    //            clip.SetCurve("", typeof(Material), propName + ".r", new AnimationCurve(rkf));
    //            Keyframe[] gkf = new Keyframe[] { new Keyframe(0f, src.g, 0f, 0f), new Keyframe(t / 2f, dst.g, 0f, 0f), new Keyframe(t, src.g, 0f, 0f) };
    //            clip.SetCurve("", typeof(Material), propName + ".g", new AnimationCurve(gkf));
    //            Keyframe[] bkf = new Keyframe[] { new Keyframe(0f, src.b, 0f, 0f), new Keyframe(t / 2f, dst.b, 0f, 0f), new Keyframe(t, src.b, 0f, 0f) };
    //            clip.SetCurve("", typeof(Material), propName + ".b", new AnimationCurve(bkf));
    //            clip.wrapMode = mode;
    //            anim.AddClip(clip, m_animName);
    //        }
    //        anim.Play(m_animName);
    //        m_activeClips.Add(m_animName);
    //    }
    //}
    //void StopAnimation(string name)
    //{
    //    if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null && GetComponent<Renderer>().material.HasProperty(propName))
    //    {
    //        GetComponent<Renderer>().material.SetColor(propName, Color.white);
    //        if (anim.IsPlaying(name)) { anim.Stop(name); }
    //        anim.RemoveClip(name);
    //        m_activeClips.Remove(name);
    //    }
    //}
}