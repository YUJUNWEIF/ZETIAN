using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[ExecuteInEditMode]
public class GuiTexSprite : MonoBehaviour
{
    public enum Effect
    {
        None,
        Shadow,
        Outline,
    }

    [SerializeField]
    Sprite m_sprite;
    [SerializeField]
    Vector2 m_size = new Vector2(100, 100);
    [SerializeField]
    Color m_color = Color.white;
    public Color color
    {
        get { return m_color; }
        set
        {
            if (m_color != value)
            {
                m_color = value;
                if (meshRender.sharedMaterial)
                {
                    meshRender.sharedMaterial.SetColor("_Color", m_color);
                }
            }
        }
    }

    void OnDidApplyAnimationProperties()
    {
        if (meshRender.sharedMaterial)
        {
            meshRender.sharedMaterial.SetColor("_Color", m_color);
        }
    }

    public Vector2 size
    {
        get { return m_size; }
        set
        {
            if (m_size != value)
            {
                m_size = value;
                if (m_size.x < 1f) { m_size.x = 1f; }
                if (m_size.y < 1f) { m_size.y = 1f; }
                RenderTex();
            }
        }
    }
    public Sprite sprite
    {
        get { return m_sprite; }
        set
        {
            if (m_sprite != value)
            {
                m_sprite = value;
                RenderTex();
            }
        }
    }
    Mesh mesh;
    public MeshRenderer meshRender
    {
        get
        {
            var render = GetComponent<MeshRenderer>();
            if (render == null) { render = gameObject.AddComponent<MeshRenderer>(); }
            //render.hideFlags = HideFlags.HideInInspector;
            render.shadowCastingMode = ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.useLightProbes = false;
            render.reflectionProbeUsage = ReflectionProbeUsage.Off;
            return render;
        }
    }
    public MeshFilter meshFilter
    {
        get
        {
            var filter = GetComponent<MeshFilter>();
            if (filter == null) { filter = gameObject.AddComponent<MeshFilter>(); }
            //filter.hideFlags = HideFlags.HideInInspector;
            return filter;
        }
    }
    void Awake()
    {
        meshRender.sharedMaterial = new Material(Shader.Find("Custom/TexSprite"));
        meshRender.sharedMaterial.hideFlags = HideFlags.DontSave;
    }
    void OnDistroy()
    {
        if (meshRender.sharedMaterial == null)
        {
            Util.UnityHelper.SafeRelease(meshRender.sharedMaterial);
            meshRender.sharedMaterial = null;
        }
    }
    void OnEnable()
    {
        meshRender.enabled = true;
        mesh = new Mesh();
        mesh.hideFlags = HideFlags.HideAndDontSave;
        mesh.MarkDynamic();
        meshFilter.sharedMesh = mesh;
        RenderTex();
    }
    void OnDisable()
    {
        meshRender.enabled = false;
  
    }
    public void SetNativeSize()
    {
        if (m_sprite && m_sprite.texture)
        {
            m_size = m_sprite.rect.size;
            RenderTex();
        }
    }
    void RenderTex()
    {
        //var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(m_sprite);

        if (meshFilter.sharedMesh != null) meshFilter.sharedMesh.Clear();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0 - size.x * 0.5f, 0 - size.y * 0.5f, 0f);
        vertices[1] = new Vector3(0 + size.x * 0.5f, 0 - size.y * 0.5f, 0f);
        vertices[2] = new Vector3(0 + size.x * 0.5f, 0 + size.y * 0.5f, 0f);
        vertices[3] = new Vector3(0 - size.x * 0.5f, 0 + size.y * 0.5f, 0f);
        Rect uv = new Rect(0, 0, 1, 1);
        if (m_sprite && m_sprite.texture)
        {
            if (m_sprite.texture.width != 0 && m_sprite.texture.height != 0)
            {
                uv.xMin = (m_sprite.rect.xMin + 0 * m_sprite.rect.width) / m_sprite.texture.width;
                uv.xMax = (m_sprite.rect.xMax - 0 * m_sprite.rect.width) / m_sprite.texture.width;
                uv.yMin = (m_sprite.rect.yMin + 0 * m_sprite.rect.height) / m_sprite.texture.height;
                uv.yMax = (m_sprite.rect.yMax - 0 * m_sprite.rect.height) / m_sprite.texture.height;
            }
        }


        mesh.Clear(false);
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        //mesh.colors = new Color[4] { Color.white, Color.white, Color.white, Color.white };
        //mesh.RecalculateBounds();
        mesh.uv = new Vector2[4] {
                new Vector2(uv.xMin, uv.yMin),
                new Vector2(uv.xMax, uv.yMin),
                new Vector2(uv.xMax, uv.yMax),
                new Vector2(uv.xMin, uv.yMax) };
        if (meshRender.sharedMaterial)
        {
            meshRender.sharedMaterial.mainTexture = m_sprite ? m_sprite.texture : null;
            meshRender.sharedMaterial.SetColor("_Color", m_color);
        }
    }
}