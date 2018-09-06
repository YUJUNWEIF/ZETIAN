using UnityEngine;
using System;
using System.Collections.Generic;

public class ModelAnimation
{
    protected Transform m_transform;
    public Renderer render { get; private set; }
    public Material material { get { return render ? render.material : null; } }
    public Color orignal { get; private set; }
    public void SetVisibleData(string modelName, Transform root)
    {
        m_transform = root;
        var no = m_transform.Find(modelName);
        if (no)
        {
            render = no.GetComponent<Renderer>();
            if (render && render.material) { orignal = render.material.color; }
        }
    }
    public void ChangeModelShader(string kName)
    {
        Shader shader = Shader.Find(kName);
        if (shader == null) { return; }
        DoOnRenderer<MeshRenderer>(it => { if (it.material != null) { it.material.shader = shader; } }, true);
        DoOnRenderer<SkinnedMeshRenderer>(it => { if (it.material != null) { it.material.shader = shader; } }, true);
    }
    public void PlayColorAnim(Color src, Color dst, float duration)
    {
        TweenColor.Begin(m_transform.gameObject, duration, src, dst, render);
    }
    public void ChangeColor(Color color)
    {
        if (render && render.material)
        {
            render.material.color = color;
        }
    }
    public void StopTweener()
    {
        var script = m_transform.gameObject.GetComponent<TweenColor>();
        if (script) { script.Stop(); }
    }
    void DoOnRenderer<T>(Action<T> action, bool includeInactive) where T : Component
    {
        Array.ForEach(m_transform.GetComponentsInChildren<T>(includeInactive), it => action(it));
    }
}