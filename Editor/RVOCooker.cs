using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

[ExecuteInEditMode]
public class RVOCooker : EditorWindow
{
    class CookMap
    {
        struct IV
        {
            public int x;
            public int z;
        }
        struct Edge
        {
            public int min;
            public int max;
            public static Edge invalid { get { return new Edge(); } }
            public Edge(int a, int b)
            {
                min = a < b ? a : b;
                max = a > b ? a : b;
            }
            public static bool operator ==(Edge x, Edge y)
            {
                return x.min == y.min && x.max == y.max;
            }
            public static bool operator !=(Edge x, Edge y)
            {
                return !(x == y);
            }
            public static implicit operator bool(Edge edge)
            {
                return edge != invalid;
            }
            public override bool Equals(object obj)
            {
                if (obj is Edge)
                {
                    var other = (Edge)obj;
                    return min == other.min && max == other.max;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return min;
            }
            public override string ToString()
            {
                return "(" + min.ToString() + "," + max.ToString() + ")";
            }
        }
        class Tri
        {
            public int a;
            public int b;
            public int c;
            public bool positive;
            public Edge la;
            public Edge lb;
            public Edge lc;
            public Tri(Vector3[] normals, int a, int b, int c)
            {
                this.a = a;
                this.b = b;
                this.c = c;

                var normal = normals[a] + normals[b] + normals[c];
                positive = Vector3.Dot(normal, Vector3.up) > 0;
                la = new Edge(a, b);
                lb = new Edge(b, c);
                lc = new Edge(c, a);
            }
            bool IsEdge(Edge edge)
            {
                return (edge == la || edge == lb || edge == lc);
            }
            public Edge GetEdgeCommon(Tri tri)
            {
                if (tri.IsEdge(la)) { return la; }
                if (tri.IsEdge(lb)) { return lb; }
                if (tri.IsEdge(lc)) { return lc; }
                return Edge.invalid;
            }
            public bool ContainEdge(Edge edge)
            {
                return (edge == la || edge == lb || edge == lc);
            }
            public override string ToString()
            {
                return "(" + positive.ToString() + ")" + la.ToString() + "|" + lb.ToString() + "|" + lc.ToString();
            }
        }
        class Polygon
        {
            static float minAngle = Mathf.Sin(Mathf.PI * 5f / 180);
            IList<IV> ivs;
            public List<Edge> edges { get; private set; }
            public List<int> points { get; private set; }
            public Polygon(List<int> points, IList<IV> ivs)
            {
                this.points = points;
                this.ivs = ivs;
                while (true)
                {
                    var oldcount = points.Count;
                    RemoveDuplicatePoint();
                    RemoveTriPointOnline();
                    if (oldcount == points.Count)
                    {
                        break;
                    }
                }
                edges = new List<Edge>();
                if (points.Count >= 3)
                {
                    for (int index = 0; index < points.Count; ++index)
                    {
                        edges.Add(new Edge(points[index], points[(index + 1) % points.Count]));
                    }
                }
                else if (points.Count >= 2)
                {
                    edges.Add(new Edge(points[0], points[1]));
                }
            }
            void RemoveDuplicatePoint()
            {
                for (int index = 0; index < points.Count; ++index)
                {
                    if (points[index] == points[(index - 1 + points.Count) % points.Count])
                    {
                        points.RemoveAt(index);
                    }
                }
            }
            void RemoveTriPointOnline()//去掉一条直线上的点
            {
                int index = 0;
                while (points.Count >= 3 && index < points.Count - 1)
                {
                    var cv0 = ivs[points[index]];
                    var lv1 = ivs[points[(index - 1 + points.Count) % points.Count]];
                    var lv2 = ivs[points[(index - 2 + points.Count) % points.Count]];


                    var line1_dir = new FEVector2D(cv0.x - lv1.x, cv0.z - lv1.z).normalized;
                    var line2_dir = new FEVector2D(lv1.x - lv2.x, lv1.z - lv2.z).normalized;

                    var det = FEVector2D.Det(line1_dir, line2_dir);
                    var online = Mathf.Abs(det) < minAngle;
                    if (online)
                    {
                        points.RemoveAt((index - 1 + points.Count) % points.Count); //去掉lv1
                        ++index;
                    }
                    else
                    {
                        ++index;
                    }
                }
            }
            public List<FEVector2D> ClockWise(float grid)//计算多边形法线
            {
                var vss = points.ConvertAll(it => new FEVector2D(ivs[it].x * grid, ivs[it].z * grid));
                if (vss.Count >= 3)
                {
                    float minY = float.MaxValue;
                    int minIndex = -1;
                    for (int index = 0; index < vss.Count; ++index)
                    {
                        var v = vss[index];
                        if (minIndex < 0 || v.y < minY)
                        {
                            minIndex = index;
                            minY = v.y;
                        }
                    }

                    var prev = vss[(minIndex - 1 + vss.Count) % vss.Count];
                    var next = vss[(minIndex + 1) % vss.Count];

                    var det = FEVector2D.Det(vss[minIndex] - prev, next - vss[minIndex]);
                    if (det < 0)//reverse vertices
                    {
                        vss.Reverse();
                    }
                }
                return vss;
            }
            public static Polygon Merge(Polygon x, Polygon y)
            {
                for (int index = 0; index < x.edges.Count; ++index)
                {
                    var edge = x.edges[index];
                    if (y.edges.Contains(edge))
                    {
                        var points = new List<int>();
                        var xIndex = x.points.FindIndex(it => it == edge.min);
                        if (x.points[(xIndex + 1) % x.points.Count] != edge.max)
                        {
                            x.points.Reverse();
                            xIndex = (x.points.Count - 1) - xIndex;
                        }
                        var yIndex = y.points.FindIndex(it => it == edge.min);
                        if (y.points[(yIndex + 1) % y.points.Count] == edge.max)
                        {
                            y.points.Reverse();
                            yIndex = (y.points.Count - 1) - yIndex;
                        }
                        for (int j = 0; j < x.points.Count - 1; ++j)
                        {
                            points.Add(x.points[(xIndex + 1 + j) % x.points.Count]);
                        }
                        for (int j = 0; j < y.points.Count - 1; ++j)
                        {
                            points.Add(y.points[(yIndex + j) % y.points.Count]);
                        }
                        return new Polygon(points, x.ivs);
                    }
                }
                return null;
            }
        }
        public RVO.ObstacleMap CookMeshes(MeshFilter[] filters, float grid)
        {
            var obsMap = new RVO.ObstacleMap();
            for (int index = 0; index < filters.Length; ++index)
            {
                var filter = filters[index];
                var mat = filter.transform.localToWorldMatrix;
                var vertices = filter.sharedMesh.vertices;
                var normals = filter.sharedMesh.normals;
                for (int j = 0; j < vertices.Length; ++j)
                {
                    vertices[j] = mat.MultiplyPoint3x4(vertices[j]);
                }
                for (int j = 0; j < normals.Length; ++j)
                {
                    normals[j] = mat.MultiplyVector(normals[j]);
                }

                List<Edge> edges = new List<Edge>();
                List<int> triangles = new List<int>();
                for (int j = 0; j < filter.sharedMesh.subMeshCount; ++j)
                {
                    triangles.Clear();
                    filter.sharedMesh.GetTriangles(triangles, j);
                    Proc(edges, triangles, normals);
                }
                Convert(vertices, edges, grid, obsMap);
            }
            return obsMap;
        }
        void Convert(Vector3[] vertices, List<Edge> edges, float grid, RVO.ObstacleMap obsMap)
        {
            List<List<Vector3>> fpolygons = new List<List<Vector3>>();

            while (edges.Count > 0)
            {
                var polygon = Polygonal(vertices, edges);
                if (polygon.Count > 1)
                {
                    fpolygons.Add(polygon);
                }
            }

            List<IV> ivs = new List<IV>();
            List<Polygon> polygons = new List<Polygon>();
            for (int index = 0; index < fpolygons.Count; ++index)
            {
                var indices = Indexer(ivs, fpolygons[index], grid);
                var polygon = new Polygon(indices, ivs);
                if (polygon.edges.Count > 0)
                {
                    polygons.Add(polygon);
                }
            }

            for (int index = 0; index < polygons.Count; ++index)
            {
                for (int j = index + 1; j < polygons.Count;)
                {
                    var merge = Polygon.Merge(polygons[index], polygons[j]);
                    if (merge != null)
                    {
                        polygons[index] = merge;
                        polygons.RemoveAt(j);
                    }
                    else
                    {
                        ++j;
                    }
                }
            }
            for (int index = 0; index < polygons.Count; ++index)
            {
                var polygon = polygons[index];
                if (polygon.edges.Count > 0)
                {
                    var vss = polygon.ClockWise(grid);
                    obsMap.obstacles.Add(new RVO.ObstacleMap.Obs() { vss = vss });
                }
            }
        }

        List<int> Indexer(List<IV> ivs, List<Vector3> fpoints, float grid)
        {
            List<int> indices = new List<int>();
            for (int index = 0; index < fpoints.Count; ++index)
            {
                var v = fpoints[index];
                var curX = Mathf.RoundToInt(v.x / grid);
                var curZ = Mathf.RoundToInt(v.z / grid);

                var exist = ivs.FindIndex(it => it.x == curX && it.z == curZ);
                if (exist >= 0)
                {
                    indices.Add(exist);
                }
                else
                {
                    ivs.Add(new IV() { x = curX, z = curZ });
                    indices.Add(ivs.Count - 1);
                }
            }
            return indices;
        }

        List<Vector3> Polygonal(Vector3[] vertices, List<Edge> edges)
        {
            var polygon = new List<Vector3>();
            var edge = edges[edges.Count - 1];
            int pa = edge.min;
            int pb = edge.max;

            edges.RemoveAt(edges.Count - 1);
            polygon.Add(vertices[pa]);

            while (edges.Count > 0)
            {
                bool succeed = false;
                for (int index = 0; index < edges.Count; ++index)
                {
                    edge = edges[index];
                    if (pa == edge.min)
                    {
                        succeed = true;
                        polygon.Add(vertices[pa = edge.max]);
                        edges.RemoveAt(index);
                        break;
                    }
                    if (pa == edge.max)
                    {
                        succeed = true;
                        polygon.Add(vertices[pa = edge.min]);
                        edges.RemoveAt(index);
                        break;
                    }
                }
                if (!succeed)
                {
                    break;
                }
            }
            return polygon;
        }
        void Proc(List<Edge> edges, List<int> triangles, Vector3[] normals)
        {
            var tris = new List<Tri>();

            for (int index = 0; index < triangles.Count; index += 3)
            {
                var tri = new Tri(normals, triangles[index + 0], triangles[index + 1], triangles[index + 2]);
                tris.Add(tri);
            }

            for (int index = 0; index < tris.Count; ++index)
            {
                var me = tris[index];
                if (!me.positive) { continue; }
                
                CheckEdge(tris, edges, me, me.la);
                CheckEdge(tris, edges, me, me.lb);
                CheckEdge(tris, edges, me, me.lc);
            }
        }
        void CheckEdge(List<Tri> tris, List<Edge> edges, Tri me, Edge edge)
        {
            //var exist = edges.IndexOf(edge);
            //if (exist < 0)
            //{
            //    var tri = tris.Find(it => it != me && it.ContainEdge(edge));
            //    if (tri == null || !tri.positive)
            //    {
            //        edges.Add(edge);
            //    }
            //}
            //else
            //{
            //    edges.RemoveAt(exist);
            //}

            if (!edges.Contains(edge))
            {
                var tri = tris.Find(it => it != me && it.ContainEdge(edge));
                if (tri == null || !tri.positive)
                {
                    edges.Add(edge);
                }
            }
        }
    }


    public const float grid = 1f;
    [MenuItem("Custom/RVOCooker")]
    static void ExportFrames()
    {
        RVOCooker window = (RVOCooker)GetWindow<RVOCooker>(true, "FishArray");
        window.Show();

    }

    // 显示窗体里面的内容
    int m_select = -1;
    string m_arrayName = "New RVO";
    List<string> m_files;
    
    void OnGUI()
    {
        //if (m_files == null)
        //{
        //    m_files = new List<string>();
        //    new tab.CGReader().Parse(GlobalDefine.fishArray, (name, bytes) =>
        //    {
        //        if (name.EndsWith(".xml")) { m_files.Add(name.Substring(0, name.Length - 4)); }
        //    });
        //}
        //if (tab.CSVTableManager.Inst() == null)
        //{
        //    new tab.CSVTableManager();
        //    new tab.CGReader().Parse(GlobalDefine.config, (name, bytes) =>
        //    {
        //        switch (name)
        //        {
        //            case GlobalDefine.ConfigContent.binXls: tab.CSVTableManager.Inst().LoadBinaryZipFile(bytes); break;
        //        }
        //    });
        //}
        GUILayout.BeginVertical();
        m_arrayName = GUILayout.TextField(m_arrayName);
        if (GUILayout.Button("New RVO") && !string.IsNullOrEmpty(m_arrayName))
        {
            var go = Selection.activeGameObject;
            var filters = go.GetComponentsInChildren<MeshFilter>(false);
            var cm = new CookMap();
            var obsMap = cm.CookMeshes(filters, grid);
            DrawObstacleMap(obsMap);

            var smr = go.GetComponentsInChildren<SkinnedMeshRenderer>(false);
            var os = new MemoryStream();
            obsMap.Marsh(new Util.OutStream(os));
            File.WriteAllBytes("test.bin", os.ToArray());

        }
        //GUILayout.BeginScrollView(new Vector2(20, -80));
        //if (m_files != null)
        //{
        //    m_select = GUILayout.SelectionGrid(m_select, m_files.ToArray(), 2);
        //}
        //GUILayout.EndScrollView();
        //if (GUILayout.Button("Open"))
        //{
        //    if (m_select >= 0 && m_select < m_files.Count)
        //    {
        //        new tab.CGReader().Parse(GlobalDefine.fishArray, (name, bytes) =>
        //        {
        //            if (name == m_files[m_select] + ".xml")
        //            {
        //                var array = new GameObject("array").AddComponent<FishArray>();
        //                array.SetArray(FEFishArray.FromXml(bytes));
        //            }
        //        });

        //        m_files = null;
        //        Close();
        //    }
        //}
        //GUILayout.EndVertical();
    }

    static void DrawObstacleMap(RVO.ObstacleMap obsMap)
    {
        for (int index = 0; index < obsMap.obstacles.Count; ++index)
        {
            DrawObstacle(obsMap.obstacles[index]);
        }
    }
    static void DrawObstacle(RVO.ObstacleMap.Obs obstacle)
    {
        var vs = new Vector3[obstacle.vss.Count + 1];
        for (int index = 0; index <= obstacle.vss.Count; ++index)
        {
            var v = obstacle.vss[index % obstacle.vss.Count];
            vs[index] = new Vector3(v.x, 0f, v.y);
        }
        var line = new GameObject().AddComponent<LineRenderer>();
        line.positionCount = vs.Length;
        line.SetPositions(vs);

        //for (int index = 0; index < obstacle.vss.Count; ++index)
        //{
        //    var start = obstacle.vss[index % obstacle.vss.Count];
        //    var end = obstacle.vss[(index + 1) % obstacle.vss.Count];

        //    Handles.DrawLine(new Vector3(start.x, 0f, start.y), new Vector3(end.x, 0f, end.y));
        //}
    }
}
