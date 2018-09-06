using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DistortEffect : MonoBehaviour
{
    public Material _Material;
    //扭曲的时间系数  
    [Range(0.0f, 1.0f)]
    public float DistortTimeFactor = 0.15f;
    //扭曲的强度  
    [Range(0.0f, 0.2f)]
    public float DistortStrength = 0.01f;
    //噪声图  
    public Texture NoiseTexture = null;
    float[] time = new float[4];
    void OnWillRenderObject()
    {
        if (_Material)
        {
            _Material.SetTexture("_NoiseTex", NoiseTexture);
            _Material.SetFloat("_DistortTimeFactor", DistortTimeFactor);
            _Material.SetFloat("_DistortStrength", DistortStrength);
            var t = Time.time;
            time[0] = t * 0.05f;
            time[1] = t * 1f;
            time[2] = t * 2f;
            time[3] = t * 3f;
            _Material.SetFloatArray("_Time", time);
            //_Material.SetTexture("_MaskTex", renderTexture);
        }
    }
    //void OnEnable()
    //{
    //    mainCam = GetComponent<Camera>();
    //    if (mainCam == null)
    //        return;

    //    if (renderTexture == null)
    //        renderTexture = RenderTexture.GetTemporary(Screen.width >> downSample, Screen.height >> downSample, 0);
    //}

    //void OnDestroy()
    //{
    //    if (renderTexture)
    //    {
    //        RenderTexture.ReleaseTemporary(renderTexture);
    //    }
    //}

    ////在真正渲染前的回调，此处渲染Mask遮罩图  
    //void OnPreRender()
    //{
    //    //maskObjShader进行渲染  
    //    if (mainCam.enabled)
    //    {
    //        mainCam.targetTexture = renderTexture;
    //    }
    //}
}
