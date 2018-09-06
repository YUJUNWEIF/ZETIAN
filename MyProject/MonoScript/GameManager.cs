using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Util;
using geniusbaby;

public class GameManager : MonoBehaviour
{
    public Camera camera2d;
    public Camera camera3d;
    public RectTransform desktop;
    Framework m_framework;
    int fps = 0;
    float time;
    public void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.dataPath);
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //命令游戏尝试以一个特定的帧率渲染。 
            Application.targetFrameRate = 30;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
        //以秒计，自游戏开始的真实时间（只读）。 
        time = Time.realtimeSinceStartup;
        m_framework = new Framework();
        m_framework.StartGame(transform, camera2d, camera3d, desktop);
       
    }

    void OnDestroy()
    {
        m_framework.StopGame();
    }
    void OnApplicationFocus(bool focus)
    {
        if (!focus) { m_framework.Save(); }
    }
    void OnApplicationQuit()
    {
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        //当用户按下手机的返回键或home键退出游戏
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
        {
            Application.Quit();
        }
#endif
        ++fps;
        if(fps >= 30)
        {
            var curTime = Time.time;
            var display = (int)(fps / (curTime - time));
            //DebugWindow.debug(string.Format("fps : {0:N2}", (fps / (curTime - time))));
            DebugWindow.debug(display.ToString());
            fps = 0;
            time = curTime;
        }
    }
    void LateUpdate()
    {
        m_framework.Update();
        m_framework.FrameUpdate();
    }
}