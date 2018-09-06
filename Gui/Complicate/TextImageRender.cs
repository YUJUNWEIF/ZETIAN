using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public interface ITextImageRender
{
    string name { get; }
    ITextImage textImage { set; }
    void Outline(Outline copy);
    void Initialize(string name, Texture tex);
    void UnInitialize();
    void AddVert(Vector3 position, Color color, Vector2 uv0);
    int GetVertCount();
    Vector3 GetVertPosition(int index);
    void AfterProc();
}

public class TextImageRender
{
    public VertexHelper vertexHelper;
    List<Vector3> verts = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> indices = new List<int>();
    public void AfterProc()
    {
        for (int index = 0; index < verts.Count; index += 4)
        {
            AddTriangle(index + 0, index + 1, index + 2);
            AddTriangle(index + 2, index + 3, index + 0);
        }
    }
    public Mesh MakeMesh()
    {
        var mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        mesh.SetTriangles(indices, 0);
        return mesh;
    }
    public void OnPopulateMesh(VertexHelper vh)
    {
        if (verts.Count > 0)
        {
            vh.Clear();
            for (int index = 0; index < verts.Count; ++index)
            {
                vh.AddVert(verts[index], colors[index], uvs[index]);
            }
            for (int index = 0; index < indices.Count / 3; ++index)
            {
                vh.AddTriangle(indices[index * 3 + 0], indices[index * 3 + 1], indices[index * 3 + 2]);
            }
        }
    }
    public void AddVert(Vector3 position, Color color, Vector2 uv0)
    {
        verts.Add(position);
        colors.Add(color);
        uvs.Add(uv0);
    }
    public void AddTriangle(int idx0, int idx1, int idx2)
    {
        indices.Add(idx0);
        indices.Add(idx1);
        indices.Add(idx2);
    }
    public int GetVertCount() { return verts.Count; }
    public Vector3 GetVertPosition(int index) { return verts[index]; }
    public void UnInitialize()
    {
        vertexHelper = new VertexHelper();
        verts.Clear();
        colors.Clear();
        uvs.Clear();
        indices.Clear();
    }
}