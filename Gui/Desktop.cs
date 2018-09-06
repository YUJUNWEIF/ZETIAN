using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IPrefabLoader
{
    GameObject Load(string path, string name);
}

public class Desktop
{
    public const int EditorWidth = 1920 / 2;
    public const int EditorHeight = 1080 / 2;

    public static RectTransform root { get; private set; }
    public static Canvas canvas { get; private set; }
    public static CanvasScaler scaler { get; private set; }
    public static GraphicRaycaster raycaster { get; private set; }
    public static EventSystem evSys { get; private set; }
    
    public static int realWidth { get; private set; }
    public static int realHeight { get; private set; }
    public static CanvasScaler.ScreenMatchMode matchMode { get; private set; }
    public static float aspect { get { return realWidth * 1f / realHeight; } }
    static IPrefabLoader m_loader;
    public static void Initialize(RectTransform rc, EventSystem sys)
    {
        root = rc;
        canvas = rc.GetComponent<Canvas>();
        scaler = rc.GetComponent<CanvasScaler>();
        raycaster = rc.GetComponent<GraphicRaycaster>();
        evSys = sys;

        if (Screen.width * EditorHeight < EditorWidth * Screen.height)
        {
            realWidth = EditorWidth;
            realHeight = EditorWidth * Screen.height / Screen.width;
        }
        else
        {
            realHeight = EditorHeight;
            float scale = Screen.height * 1.0f / realHeight;
            realWidth = Mathf.RoundToInt(Screen.width / scale);
        }
        scaler.referenceResolution = new Vector2(realWidth, realHeight);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        //editorWidth = Mathf.RoundToInt(scaler.referenceResolution.x);
        //editorHeight = Mathf.RoundToInt(scaler.referenceResolution.y);

        //float scale = Screen.height * 1.0f / editorHeight;
        //realHeight = editorHeight;
        //realWidth = Mathf.RoundToInt(Screen.width / scale);
    }
    public static void Uninitialize() { }

    public static PointerEventData MakeEventData(Vector2 position)
    {
        PointerEventData eventData = new PointerEventData(evSys);
        eventData.pressPosition = position;
        eventData.position = position;
        return eventData;
    }
    public static GameObject PrefabLoader(string path, string name)
    {
        return m_loader.Load(path, name);
    }
}