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
public class GuiSpriteLine : MaskableGraphic
{
    public enum Type
    {
        Simple,
        Tiled,
    }
    [HideInInspector]
    [SerializeField]
    List<Transform> m_trans = new List<Transform>();
    [HideInInspector]
    [SerializeField]
    Sprite m_sprite;
    [HideInInspector]
    [SerializeField]
    Type m_spriteType;
    [HideInInspector]
    [SerializeField]
    Vector2 m_uvOffset;
    [HideInInspector]
    [SerializeField]
    int m_lineWidth = 1;

    List<Vector3> m_poses = new List<Vector3>();
    bool m_useTransform = true;

    public override Texture mainTexture
    {
        get { return m_sprite == null ? s_WhiteTexture : m_sprite.texture; }
    }
    protected override void OnPopulateMesh(Mesh m)
    {
        using (VertexHelper vertexHelper = new VertexHelper())
        {
            Process(vertexHelper);
            vertexHelper.FillMesh(m);
        }
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Process(vh);
    }
    public Sprite sprite
    {
        get { return m_sprite; }
        set
        {
            if (m_sprite == value) { return; }
            m_sprite = value;
            SetAllDirty();
        }
    }
    public Type type
    {
        get { return m_spriteType; }
        set
        {
            if (m_spriteType == value) { return; }
            m_spriteType = value;
            SetAllDirty();
        }
    }
    public Vector2 uvOffset
    {
        get { return m_uvOffset; }
        set
        {
            if (m_uvOffset == value) { return; }
            m_uvOffset = value;
            SetAllDirty();
        }
    }
    public List<Transform> trans
    {
        get { return m_trans; }
        set
        {
            m_useTransform = true;
            m_trans = value;
            SetAllDirty();
        }
    }
    public List<Vector3> poses
    {
        get { return m_poses; }
        set
        {
            m_useTransform = false;
            m_poses = value;
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
    void Process(VertexHelper vertexHelper)
    {
        if (m_useTransform)
        {
            if (m_trans == null) { return; }
            for (int index = 0; index < m_trans.Count; ++index)
            {
                if (index > 0)
                {
                    if (!m_trans[index - 1] || !m_trans[index]) { continue; }
                    Vector3 v0 = transform.worldToLocalMatrix.MultiplyPoint3x4(m_trans[index - 1].position);
                    Vector3 v1 = transform.worldToLocalMatrix.MultiplyPoint3x4(m_trans[index].position);
                    DrawLine(vertexHelper, v0, v1);
                }
            }
        }
        else
        {
            if (m_poses == null) { return; }
            for (int index = 0; index < m_poses.Count; ++index)
            {
                if (index > 0)
                {
                    DrawLine(vertexHelper, m_poses[index - 1], m_poses[index]);
                }
            }
        }
    }
    void DrawLine(VertexHelper vertexHelper, Vector3 v0, Vector3 v1)
    {
        Vector3 distance = (v1 - v0);
        if (distance == Vector3.zero) { return; }
        var roll = Vector3.Cross(distance.normalized, Vector3.forward) * m_lineWidth;
        var rc = (m_sprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(m_sprite) : new Vector4(0, 0, 1, 1);
        if (type == Type.Simple)
        {
            GenerateSimple(vertexHelper, v0, v1, roll, rc);
        }
        else if (type == Type.Tiled)
        {
            GenerateTiled(vertexHelper, v0, v1, roll, rc);
        }
    }
    void GenerateSimple(VertexHelper vertexHelper, Vector3 v0, Vector3 v1, Vector3 roll, Vector4 rc)
    {
        Vector2 uvMin = new Vector2(rc.x, rc.y);
        Vector2 uvMax = new Vector2(rc.z, rc.w);
        //Vector2 uv = new Vector2(m_uvOffset.x * (uvMax.x - uvMin.x), m_uvOffset.y * (uvMax.y - uvMin.y));
        var vertexIndex = vertexHelper.currentVertCount;
        Color clr = color;
        vertexHelper.AddVert(v0 - roll * 0.5f, clr, new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(v0 + roll * 0.5f, clr, new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(v1 + roll * 0.5f, clr, new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(v1 - roll * 0.5f, clr, new Vector2(uvMax.x, uvMin.y));

        vertexHelper.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 2);
        vertexHelper.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 0);
    }
    void GenerateTiled(VertexHelper vertexHelper, Vector3 v0, Vector3 v1, Vector3 roll, Vector4 rc)
    {
        Vector3 distance = (v1 - v0);
        var length = distance.magnitude;
        //var direction = distance / length;

        Vector4 border = Vector4.zero;
        Vector2 spriteSize = m_sprite.rect.size;
        float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
        float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;

        Vector2 uv = new Vector2(Mathf.Clamp01(m_uvOffset.x) * (rc.z - rc.x), Mathf.Clamp01(m_uvOffset.y) * (rc.w - rc.y));
        Vector2 uvMin = new Vector2(rc.x, rc.y);
        Vector2 uvMax = new Vector2(rc.z, rc.w);

        // Min to max max range for tiled region in coordinates relative to lower left corner.
        float xMin = 0f;
        float xMax = length;
        float yMin = -m_lineWidth * 0.5f;
        float yMax = m_lineWidth * 0.5f;

        // Safety check. Useful so Unity doesn't run out of memory if the sprites are too small.
        // Max tiles are 100 x 100.
        if ((xMax - xMin) > tileWidth * 100 || (yMax - yMin) > tileHeight * 100)
        {
            tileWidth = (xMax - xMin) / 100;
            tileHeight = (yMax - yMin) / 100;
        }
        var clipped = uvMax;
        var m_FillCenter = true;
        if (m_FillCenter)
        {
            for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
            {
                float y2 = y1 + tileHeight;
                if (y2 > yMax)
                {
                    clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1) + uv.y;
                    y2 = yMax;
                }
                var yv1 = roll * y1 / m_lineWidth;
                var yv2 = roll * y2 / m_lineWidth;

                clipped.x = uvMax.x;
                for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
                {
                    float x2 = x1 + tileWidth;
                    if (x2 > xMax)
                    {
                        clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);// + uv.x;
                        x2 = xMax;
                    }

                    var vertexIndex = vertexHelper.currentVertCount;
                    Color clr = color;
                    var xv1 = distance * x1 / length;
                    var xv2 = distance * x2 / length;
                    
                    vertexHelper.AddVert(v0 + xv1 + yv1, clr, new Vector2(uvMin.x, uvMin.y));
                    vertexHelper.AddVert(v0 + xv1 + yv2, clr, new Vector2(uvMin.x, clipped.y));
                    vertexHelper.AddVert(v0 + xv2 + yv2, clr, new Vector2(clipped.x, clipped.y));
                    vertexHelper.AddVert(v0 + xv2 + yv1, clr, new Vector2(clipped.x, uvMin.y));

                    vertexHelper.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 2);
                    vertexHelper.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex + 0);
                }
            }
        }
    }
    public float pixelsPerUnit
    {
        get
        {
            float spritePixelsPerUnit = 100;
            if (sprite)
                spritePixelsPerUnit = sprite.pixelsPerUnit;

            float referencePixelsPerUnit = 100;
            if (canvas)
                referencePixelsPerUnit = canvas.referencePixelsPerUnit;

            return spritePixelsPerUnit / referencePixelsPerUnit;
        }
    }
}