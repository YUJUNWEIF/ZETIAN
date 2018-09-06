using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

public class Crash : Singleton<Crash>, IPluginLib
{
    /*
#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void InstallUncaughtExceptionHandler(string url);
	
	[DllImport("__Internal")]
	private static extern void UploadBug(string msg);

#elif UNITY_ANDROID
#endif
    public static void InstallCrashHandler(string url)
    {
        if (Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
        {

#if UNITY_IPHONE
            InstallUncaughtExceptionHandler(url);
#elif UNITY_ANDROID
#endif
        }
    }
    public static void UploadToLogServer(string msg)
    {
        if (Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
        {

#if UNITY_IPHONE
            UploadBug(msg);
#elif UNITY_ANDROID
#endif
        }
    }
*/
    private const string BuglyAppIDForiOS = "93b3dce5fa";
    private const string BuglyAppIDForAndroid = "144dc1eaa6";
    //private static float StandardScreenWidth = 640.0f;
    //private static float StandardScreenHeight = 960.0f;
    private float screenWidth;
    private float screenHeight;
    bool m_debugMode;
#if UNITY_ANDROID
    public void Initialize(AndroidJavaClass unityPlayer, AndroidJavaObject toaster, string msgReceiver, bool debugMode)
#else
    public void Initialize(string msgReceiver, bool debugMode)
#endif
    {
        BuglyAgent.PrintLog(LogSeverity.LogInfo, "Demo Start()");
        BuglyAgent.DebugLog("Demo.Awake()", "Screen: {0} x {1}", Screen.width, Screen.height);
        m_debugMode = debugMode;
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        InitBuglySDK();

        BuglyAgent.PrintLog(LogSeverity.LogWarning, "Init bugly sdk done");
        // set tag
#if UNITY_ANDROID
        BuglyAgent.SetScene(3450);
#else
        BuglyAgent.SetScene(3261);
#endif
    }
    public void UnInitialize() { }
    void InitBuglySDK()
    {
        // TODO NOT Required. Set the crash reporter type and log to report
        // BuglyAgent.ConfigCrashReporter (1, 2);

        // TODO NOT Required. Enable debug log print, please set false for release version
        BuglyAgent.ConfigDebugMode(m_debugMode);
        // TODO NOT Required. Register log callback with 'BuglyAgent.LogCallbackDelegate' to replace the 'Application.RegisterLogCallback(Application.LogCallback)'
        BuglyAgent.RegisterLogCallback((condition, stackTrace, type) =>
        {
            //if (type == LogType.Error || type == LogType.Exception)
            //{
            //    m_msg = string.Format("[{0}] - {1}\n{2}", type.ToString(), condition, stackTrace);
            //    //Util.Logger.Instance.Error("--------- OnApplicationLogCallbackHandler ---------\n");
            //    //Util.Logger.Instance.Error(string.Format("Current thread: {0}", Thread.CurrentThread.ManagedThreadId));
            //    //Util.Logger.Instance.Error(string.Format("[{0}] - {1}\n{2}", type.ToString(), condition, stackTrace));
            //    //Util.Logger.Instance.Error("--------- OnApplicationLogCallbackHandler ---------");
            //}
            if (type == LogType.Error || type == LogType.Exception)
            {
                UnityEngine.Debug.LogError(string.Format("[{0}] - {1}\n{2}", type.ToString(), condition, stackTrace));
            }
        });

        // BuglyAgent.ConfigDefault ("Bugly", null, "ronnie", 0);

#if UNITY_IPHONE || UNITY_IOS
        BuglyAgent.InitWithAppId (BuglyAppIDForiOS);
#elif UNITY_ANDROID
        BuglyAgent.InitWithAppId(BuglyAppIDForAndroid);
#endif

        // TODO Required. If you do not need call 'InitWithAppId(string)' to initialize the sdk(may be you has initialized the sdk it associated Android or iOS project),
        // please call this method to enable c# exception handler only.
        BuglyAgent.EnableExceptionHandler();

        // TODO NOT Required. If you need to report extra data with exception, you can set the extra handler
        //        BuglyAgent.SetLogCallbackExtrasHandler (MyLogCallbackExtrasHandler);

        BuglyAgent.PrintLog(LogSeverity.LogInfo, "Init the bugly sdk");
    }

    //void OnGUI()
    //{
    //    if (m_debugMode)
    //    {
    //        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
    //        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), m_msg);
    //        GUILayout.EndArea();
    //    }
    //}
}