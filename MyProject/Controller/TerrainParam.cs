using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using geniusbaby;

public class TerrainParam : MonoBehaviour
{
    public Camera eye { get; protected set; }
    public float maxPitch = 60f;
    public Vector2 xLimit;
    public Vector2 zLimit;
    public Vector2 heightLimit = new Vector2(2f, 50f);
    public float zoomSpeed = 40f;
    public float dragMoveSpeed = 0.1f;
    public float dragPitchSpeed = 0.2f;
    public float rotSpeed = 5f;
    public Color enviorment;
    public virtual void Awake()
    {
        RenderSettings.ambientLight = enviorment;
        eye = transform.Find(@"dynamic/eye").GetComponent<Camera>();
        eye.enabled = false;
    }
    public void SetCamera(Camera cam3d)
    {
        if (GEFov(16, 9))
        {
            cam3d.fieldOfView = 32;
        }
        else if (GEFov(3, 2))
        {
            cam3d.fieldOfView = 37;
        }
        else
        {
            cam3d.fieldOfView = 42;
        }
        cam3d.nearClipPlane = eye.nearClipPlane;
        cam3d.farClipPlane = eye.farClipPlane;
        cam3d.transform.position = eye.transform.position;
        cam3d.transform.rotation = eye.transform.rotation;
    }
    bool GEFov(int width, int height)
    {
        return (Desktop.realWidth * height >= width * Desktop.realHeight);
    }
}