using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPluginLib
{
#if UNITY_ANDROID
    void Initialize(AndroidJavaClass unityPlayer, AndroidJavaObject toaster, string msgReceiver, bool debugMode);
#else
    void Initialize(string msgReceiver, bool debugMode);
#endif
    void UnInitialize();
}
namespace geniusbaby
{
    class PluginLib : Singleton<PluginLib>
    {
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer;
        AndroidJavaObject toaster;
        const string unityPlayerClassName = "com.unity3d.player.UnityPlayer";
        const string toasterClassName = "com.fsyl.tts.ToasterImpl";
#endif
        List<IPluginLib> m_plugins = new List<IPluginLib>();
        public PluginLib()
        {
            m_plugins.Add(new Crash());
            m_plugins.Add(new TtsSpeak());

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                unityPlayer = new AndroidJavaClass(unityPlayerClassName);
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    toaster = new AndroidJavaObject(toasterClassName, curActivity);
                }
            }
#endif
        }
        public void StartGame()
        {
            for (int index = 0; index < m_plugins.Count; ++index)
            {
#if UNITY_ANDROID
                m_plugins[index].Initialize(unityPlayer, toaster, string.Empty, GamePath.debug.debugMode);
#else
                m_plugins[index].Initialize(string.Empty, GamePath.debug.debugMode);
#endif
            }
        }
        public void StopGame()
        {
            for (int index = 0; index < m_plugins.Count; ++index)
            {
                m_plugins[index].UnInitialize();
            }
        }
    }
}
