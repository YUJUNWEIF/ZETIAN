using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

[ExecuteInEditMode]
public class TextImage2DRender : MaskableGraphic, ITextImageRender
{
    TextImageRender renderImpl = new TextImageRender();
    Texture m_tex;
    ITextImage m_textImage;
    RectTransform m_cachedRc;
    public override Texture mainTexture { get { return m_tex ? m_tex : s_WhiteTexture; } }
    public ITextImage textImage
    {
        get { return m_textImage; }
        set
        {
            m_textImage = value;
            var parent = (m_textImage as TextImage2D).GetComponent<RectTransform>();
            m_cachedRc.pivot = (m_cachedRc.anchorMin = (m_cachedRc.anchorMax = new Vector2(0, 1)));
            m_cachedRc.SetParent(parent);
            m_cachedRc.localPosition = Vector3.zero;
            m_cachedRc.localScale = Vector3.one;
            m_cachedRc.anchoredPosition = Vector2.zero;
            m_cachedRc.sizeDelta = parent.sizeDelta;
        }
    }
    public void Outline(Outline copy)
    {
        if (copy)
        {
            var outline = gameObject.GetComponent<Outline>();
            if (!outline)
            {
                outline = gameObject.AddComponent<Outline>();
            }
            outline.enabled = true;
            outline.effectDistance = copy.effectDistance;
            outline.effectColor = copy.effectColor;
        }
        else
        {
            var outline = gameObject.GetComponent<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }
        }
    }
    public void Initialize(string name, Texture tex) { this.name = name; m_tex = tex; }
    protected override void OnEnable()
    {
        base.OnEnable();
        m_cachedRc = GetComponent<RectTransform>();
        if (!m_cachedRc) { m_cachedRc = gameObject.AddComponent<RectTransform>(); }
    }
    public void AfterProc()
    {
        renderImpl.AfterProc();
        rectTransform.sizeDelta = new Vector2(textImage.width, textImage.height);
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        renderImpl.OnPopulateMesh(vh);
    }
    public void AddVert(Vector3 position, Color color, Vector2 uv0)
    {
        renderImpl.AddVert(position, color, uv0);
    }
    public void AddTriangle(int idx0, int idx1, int idx2)
    {
        renderImpl.AddTriangle(idx0, idx1, idx2);
    }
    public int GetVertCount() { return renderImpl.GetVertCount(); }
    public Vector3 GetVertPosition(int index) { return renderImpl.GetVertPosition(index); }
    public void UnInitialize()
    {
        renderImpl.UnInitialize();
    }
}