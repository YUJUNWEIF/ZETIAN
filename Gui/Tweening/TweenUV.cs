using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TweenUV : UITweener
{
    public Renderer render;
    public string texName = "_MainTex";
    public Material matUV
    {
        get {
            if (render)
            {
#if UNITY_EDITOR
                return render.sharedMaterial;
#else
            return render.material;
#endif
            }
            return null;
        }
    }
    public Vector2 from;
    public Vector2 to;
    static public TweenUV Begin(GameObject go, float duration, Vector2 to)
    {
        TweenUV comp = go.GetComponent<TweenUV>();
        if (comp == null) { comp = go.AddComponent<TweenUV>(); }
        comp.from = comp.GetTextureOffset();
        comp.to = to;
        comp.Play();
        return comp;
    }
    Vector2 GetTextureOffset() { return matUV ? matUV.GetTextureOffset(texName) : Vector2.zero; }
    protected override void OnStep(float factor)
    {
        if (matUV)
        {
            matUV.SetTextureOffset(texName, from * (1f - factor) + to * factor);
        }
    }
}
