using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class Setting
    {
        public int music;
        public int sound;
        public void Load()
        {
            music = PlayerPrefs.GetInt("music");
            sound = PlayerPrefs.GetInt("sound");
        }
        public void Save()
        {
            PlayerPrefs.SetInt("music", music);
            PlayerPrefs.SetInt("sound", sound);
        }
    }
    public class Framework : Singleton<Framework>
    {
        class LogImp : Util.ILogImpl
        {
            public void Warn(string msg, Exception exception)
            {
                if (GamePath.debug.debugMode) { UnityEngine.Debug.LogWarning(msg); }
            }
            public void Info(string msg, Exception exception)
            {
                if (GamePath.debug.debugMode) { UnityEngine.Debug.Log(msg); }
            }
            public void Error(string msg, Exception exception)
            {
                UnityEngine.Debug.LogError(msg);
                if (exception != null)
                {
                    UnityEngine.Debug.LogError(exception.StackTrace);
                    BuglyAgent.ReportException(exception, msg);
                }
                else
                {
                    BuglyAgent.ReportException(msg, msg, msg);
                }
            }
            public void Fatal(string msg, Exception exception)
            {
                Error(msg, exception);
            }
            public void Debug(string msg, Exception exception)
            {
                if (GamePath.debug.debugMode) { UnityEngine.Debug.Log(msg); }
            }
        }
        class CoroutineCustomProcessor
        {
            public void StartGame()
            {
                Util.Coroutine.customProcesser = Process;
            }
            public void StopGame()
            {
                Util.Coroutine.customProcesser = null;
            }
            public Util.Coroutine.ProcessState Process(IEnumerator e)
            {
                var www = e.Current as UnityEngine.WWW;
                if (www != null)
                {
                    if (!www.isDone) { return Util.Coroutine.ProcessState.ProcessContinue; }
                    return Util.Coroutine.ProcessState.ProcessFinished;
                }
                var async = e.Current as UnityEngine.AsyncOperation;
                if (async != null)
                {
                    if (!async.isDone) { return Util.Coroutine.ProcessState.ProcessContinue; }
                    return Util.Coroutine.ProcessState.ProcessFinished;
                }
                return Util.Coroutine.ProcessState.NotProcess;
            }
        }
        public Camera camera2d { get; private set; }
        public Camera camera3d { get; private set; }
        public RectTransform desktop { get; private set; }
        public AudioListener audioListener { get; private set; }
        public AudioSource bgmSource { get; private set; }

        private Util.TimerManager m_timerManager = new Util.TimerManager();
        private StateManager m_stateManager = new StateManager();
        private MusicManager m_musicManager = new MusicManager();
        private SoundManager m_soundManager = new SoundManager();
        private DownloadManager m_downloadManager = new DownloadManager();
        private CoroutineCustomProcessor m_customProcessor = new CoroutineCustomProcessor();
        private PluginLib m_plugin = new PluginLib();
        private LSharpManager m_hotcodeManager = new LSharpManager();
        private ModuleManager m_moduleManager = new ModuleManager();
        private ControllerManager m_controllerManager = new ControllerManager();
        private SpritesManager m_resourcesManager = new SpritesManager();
        private BundleManager m_bundleManager = new BundleManager();
        private GameObjPool m_gameobjPool = new GameObjPool();
        private CameraControl m_cameraManager = new CameraControl();
        private SceneManager m_sceneManager = new SceneManager();
        private GuiManager m_guiManager;

        public readonly tab.CGReader reader = new tab.CGReader();
        public readonly Setting setting = new Setting();
        public static Util.FastRandom rand = new Util.FastRandom((uint)(DateTime.UtcNow.Ticks));

        public static Util.ParamActions onSave = new Util.ParamActions();
        public Util.ParamActions onSetting = new Util.ParamActions();

        public void StartGame(Transform trans, Camera cam2d, Camera cam3d, RectTransform desktop)
        {
            this.camera2d = cam2d;
            this.camera3d = cam3d;
            this.desktop = desktop;

            audioListener = trans.GetComponent<AudioListener>();
            bgmSource = trans.GetComponent<AudioSource>();
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Application.targetFrameRate = 30;
            }
            else
            {
                Application.targetFrameRate = -1;
            }

            Util.Logger.Instance.impl = new LogImp();
            m_plugin.StartGame();
            m_customProcessor.StartGame();
            m_musicManager.StartGame();
            m_soundManager.StartGame(camera2d.gameObject);

            Desktop.Initialize(desktop, trans.GetComponent<EventSystem>());
            camera2d.orthographicSize = Desktop.realHeight * 0.5f;
            //var scaler = desktop.GetComponent<CanvasScaler>();
            //scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            //scaler.screenMatchMode = Desktop.matchMode;
            desktop.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Desktop.realWidth);
            desktop.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Desktop.realHeight);

            setting.Load();
            MusicManager.Instance.CrossFadeTo(GamePath.music.musicVersion);

            m_guiManager = new GuiManager();
            GuiManager.Instance.ShowFrame(typeof(geniusbaby.LSharpScript.VersionCheckFrame).Name);
        }
        public void AfterInit()
        {
            reader.Parse("cfg/config.zip", (name, bytes) =>
            {
                switch (name)
                {
                    case GlobalDefine.ConfigContent.binHotcode: m_hotcodeManager.Regist(bytes); break;
                }
            });

            HotcodeEntry.StartGame();
            m_resourcesManager.StartGame();
            m_moduleManager.StartGame();
            m_controllerManager.StartGame();
            m_cameraManager.StartGame();
            m_sceneManager.StartGame();
            //var bytes = tab.CGReader.LoadFile("dict.mdx");
            //m_dictManager.StartGame(new System.IO.MemoryStream(bytes));
        }

        public void StopGame()
        {
            m_sceneManager.StopGame();
            m_cameraManager.StopGame();
            m_controllerManager.StopGame();
            m_moduleManager.StopGame();
            m_resourcesManager.StopGame();
            m_plugin.StopGame();
            HotcodeEntry.StopGame();
            Desktop.Uninitialize();
            m_guiManager = null;
            m_stateManager = null;

        }
        public void Update()
        {
            Util.Coroutine.defaultManager.Update();
            //FrameUpdate();
        }
        float m_lastTime = -1;
        public void FrameUpdate()
        {
            if (m_lastTime < 0)
            {
                m_lastTime = Time.realtimeSinceStartup;
            }
            var now = Time.realtimeSinceStartup;
            Util.TimerManager.Inst().FrameUpdate(now - m_lastTime);
            m_lastTime = now;
        }
        public void Save()
        {
            onSave.Fire();
        }
        public void UpSet()
        {
            setting.Save();
            onSetting.Fire();
        }
        public static bool ScreenToRectangleLocal(Vector2 screenPoint, out Vector2 localPoint, RectTransform rect = null)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect != null ? rect : Instance.desktop, screenPoint, Instance.camera2d, out localPoint);
        }
        public static Vector2 C2DWorldToScreen(Vector3 worldPoint)
        {
            return RectTransformUtility.WorldToScreenPoint(Instance.camera2d, worldPoint);
        }
        public static Vector2 C3DWorldPosToScreen(Vector3 worldPoint)
        {
            return Instance.camera3d.WorldToScreenPoint(worldPoint);
        }
        public static bool C2DWorldPosRectangleLocal(Vector3 worldPoint, out Vector2 localPos, RectTransform rect = null)
        {
            var screenPos = C2DWorldToScreen(worldPoint);
            return ScreenToRectangleLocal(screenPos, out localPos, rect);
        }
        public static bool C3DWorldPosRectangleLocal(Vector3 worldPoint, out Vector2 localPos, RectTransform rect = null)
        {
            var screenPos = C3DWorldPosToScreen(worldPoint);
            return ScreenToRectangleLocal(screenPos, out localPos, rect);
        }
    }
}