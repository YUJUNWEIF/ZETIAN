using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

[ExecuteInEditMode]
public class TextImage3DRender : MonoBehaviour, ITextImageRender
{
    TextImageRender renderImpl = new TextImageRender();
    Texture m_tex;
    MeshFilter m_meshFilter;
    MeshRenderer m_meshRender;
    Material m_mat;
    Transform m_cachedRc;
    ITextImage m_textImage;
    private void Awake()
    {
        m_cachedRc = gameObject.GetComponent<Transform>();
        m_meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!m_meshFilter) { m_meshFilter = gameObject.AddComponent<MeshFilter>(); }
        m_meshRender = gameObject.GetComponent<MeshRenderer>();
        if (!m_meshRender) { m_meshRender = gameObject.AddComponent<MeshRenderer>(); }
                
        m_mat = new Material(Shader.Find("Custom/FontEffect"));
        m_meshRender.material = m_mat;
    }
    public ITextImage textImage
    {
        get { return m_textImage; }
        set
        {
            m_textImage = value;
            var parent = (m_textImage as TextImage3D).transform;
            m_cachedRc.SetParent(parent);
            m_cachedRc.localPosition = Vector3.zero;
            m_cachedRc.localScale = Vector3.one;
            m_cachedRc.localRotation = Quaternion.identity;
        }
    }
    public void Outline(Outline copy) { }
    public void Initialize(string name, Texture tex)
    {
        this.name = name;
        m_tex = tex;
        m_mat.mainTexture = tex;
    }
    public void AfterProc()
    {
        renderImpl.AfterProc();
        m_meshFilter.sharedMesh = renderImpl.MakeMesh();
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