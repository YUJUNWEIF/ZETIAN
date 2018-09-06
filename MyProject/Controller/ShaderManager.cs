using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShaderManager : MonoBehaviour
{
    // Fields
    private bool bBlack;
    private bool bFade;
    public bool cooking;
    public Material cookShadersCover;
    private GameObject cookShadersObject;
    public bool cookShadersOnMobiles = true;
    private static ShaderManager instance;
    public Color m_kFadeColor = new Color(0f, 0f, 0f, 0f);
    public Shader[] shaders;

    // Methods
    private void Awake()
    {
        Screen.sleepTimeout = 0;
        if (cookShadersOnMobiles)
        {
            if (cookShadersCover.HasProperty("_TintColor"))
            {
                Util.Logger.Instance.Warn(transform.ToString() + "Dualstick: the CookShadersCover material needs a _TintColor property to properly hide the cooking process");
            }
            CreateCameraCoverPlane(base.transform);
            if (cookShadersCover != null)
            {
                cookShadersCover.SetColor("_TintColor", Color.black);
            }
        }
    }
    public IEnumerator BlackIn(float fTime, Color src, Color dst)
    {
        if (cookShadersObject != null)
        {
            var fElapsedTime = 0f;
            var mat = cookShadersObject.GetComponent<Renderer>().sharedMaterial;
            while (fElapsedTime < fTime)
            {
                var spin = fElapsedTime / fTime;
                var clr = src * spin + dst * (1 - spin);
                mat.SetColor("_TintColor", clr);
                fElapsedTime += Time.deltaTime;
                yield return null;

            }
        }
    }
    public IEnumerator BlackIn(float fTime)
    {
        yield return BlackIn(fTime, Color.black, new Color());
    }
    public IEnumerator BlackOut(Transform kRoot, float fTime)
    {
        yield return BlackIn(fTime, new Color(), Color.black);

    }

    public IEnumerator BlackOutIn(Transform kRoot, float fPhase1, float fPhase2, GameObject kEventReceiver)
    {
        yield return BlackIn(fPhase1, Color.black, new Color());
        yield return BlackIn(fPhase2, new Color(), Color.black);
    }

    private IEnumerator CookShaders()
    {
        cooking = true;

        var cookStartTime = Time.time;
        if (shaders.Length <= 0)
        {
            yield break;
        }
        var static_shader = new string[] { "Mobile/Particles/Alpha Blended", "Mobile/Particles/Multiply", "Mobile/Particles/VertexLit Blended", "Mobile/Particles/Additive" };

        var mat = new Material(shaders[0]);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = transform;
        cube.transform.localPosition = new Vector3(0f, 0f, 4f);
        yield return null;
        foreach (var shader in shaders)
        {
            if (shader != null)
            {
                mat.shader = shader;
                cube.GetComponent<Renderer>().material = mat;
            }
        }
        foreach (var name in static_shader)
        {
            var shader = Shader.Find(name);
            if (shader != null)
            {
                mat.shader = shader;
                cube.GetComponent<Renderer>().material = mat;

            }
            else
            {
                Util.Logger.Instance.Warn("can't find shader " + name);
            }
            yield return null;
        }
        Object.DestroyImmediate(mat);
        Object.DestroyImmediate(cube);

        Util.Logger.Instance.Info("shader cooking time " + (Time.time - cookStartTime).ToString());

        var clr = Color.black;
        while (clr.a > 0)
        {
            clr.a -= Time.deltaTime * 0.5f;
            cookShadersCover.SetColor("_TintColor", clr);
            yield return null;
        }
    }

    private GameObject CreateCameraCoverPlane(Transform kParent)
    {
        bFade = true;
        cookShadersObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cookShadersObject.GetComponent<Renderer>().material = cookShadersCover;
        cookShadersObject.transform.parent = kParent;
        cookShadersObject.transform.localPosition = new Vector3(0f, 0f, 1.55f);
        cookShadersObject.transform.localRotation = Quaternion.identity;
        cookShadersObject.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        cookShadersObject.transform.localScale = new Vector3(2.4f, 1.5f, 1.5f);
        return cookShadersObject;
    }

    public void DestroyCameraCoverPlane()
    {
        if (cookShadersObject != null)
        {
            Object.DestroyImmediate(cookShadersObject);
        }
        cookShadersObject = null;
        bFade = false;
    }

    public void PlayBlackEaseInOut(Camera kTarget, float fPhase1, float fPhase2, GameObject kEventReceiver)
    {
        if (kTarget != null)
        {
            base.StartCoroutine(this.BlackOutIn(kTarget.transform, fPhase1, fPhase2, kEventReceiver));
        }
        else
        {
            base.StartCoroutine(this.BlackOutIn(base.transform, fPhase1, fPhase2, kEventReceiver));
        }
    }

    public void StartBlackIn()
    {
        if (bBlack)
        {
            StartCoroutine(BlackIn(1f));
            bBlack = false;
        }
    }

    public void StartBlackOut()
    {
        if (!bBlack)
        {
            StartCoroutine(BlackOut(transform, 1f));
            bBlack = true;
        }
    }

    public void StartCookShaders()
    {
        if (cookShadersOnMobiles)
        {
            StartCoroutine(CookShaders());
        }
    }

    public static ShaderManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("SceneCamera").GetComponent<ShaderManager>();
            }
            return instance;
        }
    }
}
