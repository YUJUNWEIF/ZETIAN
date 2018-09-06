using System;
using System.Collections.Generic;
using UnityEngine;

public class TtsSpeak : Singleton<TtsSpeak>, IPluginLib
{
#if UNITY_IPHONE
    [DllImport("__Internal")]
    static extern void _Initialize();

    [DllImport("__Internal")]
    static extern void _Speak(string text);
#endif

#if UNITY_ANDROID
    //AndroidJavaClass unityPlayer;
    //AndroidJavaObject toaster;
    AndroidJavaObject tts;
    //const string unityPlayerClassName = "com.unity3d.player.UnityPlayer";
    //const string toasterClassName = "com.fsyl.tts.ToasterImpl";
    const string ttsSpeakClassName = "com.fsyl.tts.TtsSpeak";
    public void Initialize(AndroidJavaClass unityPlayer, AndroidJavaObject toaster, string msgReceiver, bool debugMode)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                tts = new AndroidJavaObject(ttsSpeakClassName, curActivity, toaster);
            }
        }
    }
    public void UnInitialize() { }
    public void Speak(string text)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            tts.Call("speak", text, 1);
        }
    }
#else
    public void Initialize(string msgReceiver, bool debugMode) { }
    public void UnInitialize() { }
    public void Speak(string text) { }
#endif
}