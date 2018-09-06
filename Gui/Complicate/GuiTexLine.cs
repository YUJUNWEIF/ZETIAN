using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Sprites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

[RequireComponent(typeof(RectTransform))]
public class GuiTexLine : MaskableGraphic
{
    [HideInInspector]
    [SerializeField]
    List<Transform> m_dots = new List<Transform>();
    [HideInInspector]
    [SerializeField]
    Texture2D m_tex;
    //[HideInInspector]
    [SerializeField]
    Rect m_uvRect = new Rect(0, 0, 1f, 1f);
    [HideInInspector]
    [SerializeField]
    int m_lineWidth = 1;
    public VertexHelper vertexHelper;
    public override Texture mainTexture
    {
        get { return m_tex == null ? s_WhiteTexture : m_tex; }
    }
    protected override void OnPopulateMesh(Mesh m)
    {
        Process();
        if (vertexHelper != null)
        {
            vertexHelper.FillMesh(m);
            vertexHelper.Dispose();
            vertexHelper = null;
        }
    }
    public Texture2D tex
    {
        get { return m_tex; }
        set
        {
            if (m_tex == value) { return; }
            m_tex = value;
            SetAllDirty();
        }
    }
    public Rect rect
    {
        get { return m_uvRect; }
        set
        {
            if (m_uvRect == value) { return; }
            m_uvRect = value;
            SetAllDirty();
        }
    }
    public List<Transform> dots
    {
        get { return m_dots; }
        set
        {
            m_dots = value;
            SetAllDirty();
        }
    }
    public int lineWidth
    {
        get { return m_lineWidth; }
        set
        {
            if (m_lineWidth != value)
            {
                m_lineWidth = value;
                SetAllDirty();
            }
        }
    }
    void Process()
    {
        if (m_dots == null) { return; }
        vertexHelper = new VertexHelper();
        for (int index = 0; index < m_dots.Count; ++index)
        {
            if (index > 0)
            {
                if (!m_dots[index - 1] || !m_dots[index]) { continue; }

                Vector3 v0 = transform.worldToLocalMatrix.MultiplyPoint3x4(m_dots[index - 1].position);
                Vector3 v1 = transform.worldToLocalMatrix.MultiplyPoint3x4(m_dots[index].position);
                Vector3 distance = (v1 - v0);
                if (distance == Vector3.zero) { continue; }
                var roll = Vector3.Cross(distance.normalized, Vector3.forward) * m_lineWidth;
                //GenerateSimple(v0, v1, roll, m_uvRect);

                var vertexIndex = vertexHelper.currentVertCount;
                Color clr = color;
                vertexHelper.AddVert(v0 - roll * 0.5f, clr, new Vector2(m_uvRect.xMin, m_uvRect.yMin));
                vertexHelper.AddVert(v0 + roll * 0.5f, clr, new Vector2(m_uvRect.xMin, m_uvRect.yMax));
                vertexHelper.AddVert(v1 + roll * 0.5f, clr, new Vector2(m_uvRect.xMax, m_uvRect.yMax));
                vertexHelper.AddVert(v1 - roll * 0.5f, clr, new Vector2(m_uvRect.xMax, m_uvRect.yMin));

                vertexHelper.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 2);
                vertexHelper.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 0);
            }
        }
    }
    void GenerateSimple(Vector3 v0, Vector3 v1, Vector3 roll, Rect rc)
    {
        Vector2 uvMin = new Vector2(rc.xMin, rc.yMin);
        Vector2 uvMax = new Vector2(rc.xMax, rc.yMax);
        var vertexIndex = vertexHelper.currentVertCount;
        Color clr = color;
        vertexHelper.AddVert(v0 - roll * 0.5f, clr, new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(v0 + roll * 0.5f, clr, new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(v1 + roll * 0.5f, clr, new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(v1 - roll * 0.5f, clr, new Vector2(uvMax.x, uvMin.y));

        vertexHelper.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 2);
        vertexHelper.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 0);
    }
}