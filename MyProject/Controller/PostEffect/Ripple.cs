using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RIPPLE_TYPE
{
    // method A
    // calculate a sinus, based only on time
    // this will make the ripples look like poking a soft rubber sheet, since sinus position is fixed
    RIPPLE_TYPE_RUBBER,                                     // a soft rubber sheet
    // method B
    // calculate a sinus, based both on time and distance
    // this will look more like a high viscosity fluid, since sinus will travel with radius   
    RIPPLE_TYPE_GEL,                                        // high viscosity fluid
    // method c
    // like method b, but faded for time and distance to center
    // this will look more like a low viscosity fluid, like water     
    RIPPLE_TYPE_WATER,                                      // low viscosity fluid
};

public class rippleData
{
    public bool parent;                         // ripple is a parent
    public bool[] childCreated;                 // child created ( in the 4 direction )
    public RIPPLE_TYPE rippleType;                     // type of ripple ( se update: )
    public Vector2 center;                         // ripple center ( but you just knew that, didn't you? )
    public Vector2 centerUv;               // ripple center in texture coordinates
    public float radius;                         // radius at which ripple has faded 100%
    public float strength;                       // ripple strength 
    public float runtime;                        // current run time
    public float currentRadius;                  // current radius
    public float rippleCycle;                    // ripple cycle timing
    public float lifespan;                       // total life span       
};

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Ripple : MonoBehaviour
{
    public const int CHILD_LEFT = 0;
    public const int CHILD_TOP = 1;
    public const int CHILD_RIGHT = 2;
    public const int CHILD_BOTTOM = 3;

    public float RIPPLE_BASE_GAIN = 0.1f;      // an internal constant
    public float RIPPLE_RADIUS = 500;       // radius in pixels  
    public float RIPPLE_RIPPLE_CYCLE = 0.25f;      // timing on ripple ( 1/frequenzy )
    public float RIPPLE_LIFESPAN = 3.6f;      // entire ripple lifespan
    public float RIPPLE_CHILD_MODIFIER = 2.0f;       // strength modifier
    
    public int quadXCount = 60;
    public int quadYCount = 40;
    public int MAXRIPPLE = 5;

    public RIPPLE_TYPE rippleType = RIPPLE_TYPE.RIPPLE_TYPE_WATER;
    int m_bufferSize;                   // vertice buffer size
    Vector3[] m_vertices;                      // vertices
    Vector2[] m_uvOlds;            // texture coordinates ( original )
    Vector2[] m_uvNows;             // texture coordinates ( ripple corrected )
    bool[] m_edgeVertice;                  // vertice is a border vertice        
    List<rippleData> m_ripples = new List<rippleData>();                   // list of running ripples
    public Vector2 contentSize = new Vector2(960, 640);

    MeshFilter m_render;
    Mesh m_mesh;
    void OnEnable()
    {
        m_mesh = new Mesh();
        Vector2 normalized;
        m_bufferSize = (quadXCount + 1) * (quadYCount + 1);// calculate buffer size
        m_vertices = new Vector3[m_bufferSize];
        m_uvOlds = new Vector2[m_bufferSize];
        int[] triangles = new int[quadXCount * quadYCount * 6];
        m_edgeVertice = new bool[m_bufferSize];

        int index = 0;
        for (int y = 0; y <= quadYCount; y++)
        {
            for (int x = 0; x <= quadXCount; x++)
            {
                normalized.x = (float)x / quadXCount;
                normalized.y = (float)y / quadYCount;
                m_vertices[index] = new Vector3((normalized.x - 0.5f) * contentSize.x, (normalized.y - 0.5f) * contentSize.y);
                m_uvOlds[index] = new Vector2(normalized.x, normalized.y);
                m_edgeVertice[index] = (x == 0) || (x == quadXCount) || (y == 0) || (y == quadYCount - 1);
                ++index;
            }
        }
        index = 0;
        for (int y = 0; y < quadYCount; y++)
        {
            for (int x = 0; x < quadXCount; x++)
            {
                triangles[6 * index + 0] = (x + 0) + (y + 0) * (quadXCount + 1);
                triangles[6 * index + 1] = (x + 0) + (y + 1) * (quadXCount + 1);
                triangles[6 * index + 2] = (x + 1) + (y + 0) * (quadXCount + 1);
                triangles[6 * index + 3] = (x + 1) + (y + 0) * (quadXCount + 1);
                triangles[6 * index + 4] = (x + 0) + (y + 1) * (quadXCount + 1);
                triangles[6 * index + 5] = (x + 1) + (y + 1) * (quadXCount + 1);
                ++index;
            }
        }
        m_mesh.vertices = m_vertices;
        m_mesh.uv = m_uvOlds;
        m_mesh.triangles = triangles;
        m_render = GetComponent<MeshFilter>();
        m_render.sharedMesh = m_mesh;
 //       m_render.mesh = m_mesh;
    }

    public void AddRipple(Vector2 pos, RIPPLE_TYPE type, float strength)
    {
        rippleData newRipple = new rippleData();
        newRipple.parent = true;
        newRipple.childCreated = new bool[4];
        newRipple.rippleType = type;
        newRipple.center = pos;// - contentSize * 0.5f;
        newRipple.centerUv = new Vector2(pos.x / contentSize.x,   pos.y / contentSize.y);
        newRipple.radius = RIPPLE_RADIUS; // * strength;
        newRipple.strength = strength;
        newRipple.runtime = 0;
        newRipple.currentRadius = 0;
        newRipple.rippleCycle = RIPPLE_RIPPLE_CYCLE;
        newRipple.lifespan = RIPPLE_LIFESPAN;
        m_ripples.Add(newRipple);
        while (m_ripples.Count > 5) { m_ripples.RemoveAt(0); }
    }

    public void AddRippleChild(rippleData parent, int type)
    {
        rippleData newRipple = parent;
        Vector2 pos = Vector2.zero;
        newRipple.parent = false;
        switch (type)
        {
            case CHILD_LEFT: pos = new Vector2(-parent.center.x, parent.center.y); break;
            case CHILD_TOP: pos = new Vector2(parent.center.x, 320 + (320 - parent.center.y)); break;
            case CHILD_RIGHT: pos = new Vector2(480 + (480 - parent.center.x), parent.center.y); break;
            case CHILD_BOTTOM: pos = new Vector2(parent.center.x, -parent.center.y); break;
        }
        newRipple.center = pos;
        newRipple.centerUv = new Vector2(pos.x / contentSize.x, pos.y / contentSize.y);
        newRipple.strength *= RIPPLE_CHILD_MODIFIER;
        parent.childCreated[type] = true;
        m_ripples.Add(newRipple);
    }

    void Update()
    {
        if (m_ripples.Count == 0) return;
        m_uvNows = new Vector2[m_uvOlds.Length];
        Array.Copy(m_uvOlds, m_uvNows, m_uvOlds.Length);
        float dt = Time.deltaTime;
        for (int index = m_ripples.Count - 1; index >= 0; --index)
        {
            rippleData ripple = m_ripples[index];
            ripple.runtime += dt;
            if (ripple.runtime >= ripple.lifespan)
            {
                m_ripples.RemoveAt(index);
                continue;
            }
            ripple.currentRadius = ripple.radius * ripple.runtime / ripple.lifespan;

            var center = (Vector3)ripple.center;
            for (int count = 0; count < m_bufferSize; count++)
            {
                if (m_edgeVertice[count]) continue;
                float distance = (center - m_vertices[count]).magnitude;
                if (distance > ripple.currentRadius) continue;

                float correction;
                switch (rippleType)
                {
                    default:
                    case RIPPLE_TYPE.RIPPLE_TYPE_RUBBER:
                        correction = Mathf.Sin(2 * Mathf.PI * ripple.runtime / ripple.rippleCycle);
                        break;
                    case RIPPLE_TYPE.RIPPLE_TYPE_GEL:
                        correction = Mathf.Sin(2 * Mathf.PI * (ripple.currentRadius - distance) / ripple.radius * ripple.lifespan / ripple.rippleCycle);
                        break;
                    case RIPPLE_TYPE.RIPPLE_TYPE_WATER:
                        correction = (ripple.radius * ripple.rippleCycle / ripple.lifespan) / (ripple.currentRadius - distance);
                        if (correction > 1.0f) correction = 1.0f;
                        correction *= correction;
                        correction *= Mathf.Sin(2 * Mathf.PI * (ripple.currentRadius - distance) / ripple.radius * ripple.lifespan / ripple.rippleCycle);
                        break;
                }

                correction *= 1 - (distance / ripple.currentRadius); // fade with distance
                correction *= 1 - (ripple.runtime / ripple.lifespan);// fade with time                        
                correction *= RIPPLE_BASE_GAIN;// adjust for base gain
                correction *= ripple.strength;// adjust for user strength   

                Vector2 uv = m_uvNows[count];
                correction /= (ripple.centerUv - uv).magnitude;// finally modify the coordinate by interpolating. because of interpolation, adjustment for distance is needed, 
                uv += (uv - ripple.centerUv) * correction;
                //uv.x = Mathf.Clamp(uv.x, 0f, 1f);
                //uv.y = Mathf.Clamp(uv.x, 0f, 1f);
                m_uvNows[count] = uv;
            }

            //bool RIPPLE_BOUNCE = false;
            //if (RIPPLE_BOUNCE && ripple.parent)
            //{
            //    if ((ripple.childCreated[CHILD_LEFT] == false) && (ripple.currentRadius > ripple.center.x))
            //    {
            //        addRippleChild(ripple, CHILD_LEFT);
            //    }
            //    if ((ripple.childCreated[CHILD_TOP] == false) && (ripple.currentRadius > 320 - ripple.center.y))
            //    {
            //        addRippleChild(ripple, CHILD_TOP);
            //    }
            //    if ((ripple.childCreated[CHILD_RIGHT] == false) && (ripple.currentRadius > 480 - ripple.center.x))
            //    {
            //        addRippleChild(ripple, CHILD_RIGHT);
            //    }
            //    if ((ripple.childCreated[CHILD_BOTTOM] == false) && (ripple.currentRadius > ripple.center.y))
            //    {
            //        addRippleChild(ripple, CHILD_BOTTOM);
            //    }
            //}
        }
        m_mesh.uv = (m_ripples.Count == 0) ? m_uvOlds : m_uvNows;
        //m_render.sharedMesh = m_mesh;
        //m_render.mesh = m_mesh;
    }
}