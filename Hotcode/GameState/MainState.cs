using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class WaitMainSubStateSwitchComplete : Util.IYieldInstruction
    {
        public bool done { get { return !MainState.Instance.IsSwitching(); } }
    }    
    public class MainState : ActionStateManager, IActionState
    {
        public static MainState Instance { get; private set; }
        public void Enter(ActionStateManager manager)
        {
            Instance = this;
            ModuleManager.Inst().MainEnter();
            GuiManager.Inst().Initialize();
            ReplaceAll<SubMainState>();
            manager.CompleteSwitch();
        }
        public void Leave()
        {
            var subState = GetCurrent();
            if (subState != null) { subState.Leave(); }
            GuiManager.Inst().Uninitialize();
            ModuleManager.Inst().MainExit();
            Instance = null;
        }
        public void ChangeStateByName(string stateName)
        {
            if (stateName == GetCurrent().GetType().Name) { return; }
            if (stateName == typeof(SubMainState).Name)
            {
                ReplaceAll<SubMainState>();
            }
            else if (stateName == typeof(SubFightState).Name)
            {
                ChangeState<SubFightState>();
            }
        }
        public static bool IsSubState<T>() where T : IActionState
        {
            var mainState = MainState.Instance;
            if (mainState != null)
            {
                return mainState.GetCurrent().GetType() == typeof(T);
            }
            return false;
        }
    }
    public class SubMainState : IActionState
    {
        ActionStateManager m_manager;
        IList<GuiManager.FrameMeta> m_queuedFrame;

        public void Enter(ActionStateManager manager)
        {
            m_manager = manager;
            MusicManager.Inst().CrossFadeTo(GamePath.music.musicMain);
            GuiManager.Inst().RestoreQueuedFrame(m_queuedFrame);
            SceneManager.Instance.onComplete.Add(OnMapLoaded);
            SceneManager.Inst().EnterMap("home", null, null);
            GuiManager.Instance.ShowFrame(typeof(LSharpScript.LoadingFrame).Name);
        }
        public void Leave()
        {
            GuiManager.Inst().HideFrame(typeof(LSharpScript.PvpMatchFrame).Name);//patch

            m_queuedFrame = GuiManager.Inst().SaveQueuedFrame();
            GuiManager.Inst().HideAll();
            SceneManager.Instance.LeaveMap();
            SceneManager.Instance.onComplete.Rmv(OnMapLoaded);
        }
        public void Update() { }
        void OnMapLoaded()
        {
            if (m_queuedFrame == null)
            {
                GuiManager.Inst().ShowFrame(typeof(LSharpScript.HomePageFrame).Name);
                if (GuideModule.Inst().tutoring)
                {
                    LSharpScript.GuideMaskFrame.FirstEntry(GuideModule.Instance.guide);
                }
            }
            if (!GuideModule.Instance.tutoring && KnowledgeModule.Inst().lgapId <= 0)
            {
                GuiManager.Inst().ShowFrame(typeof(LSharpScript.WordPackageFrame).Name);
            }
            if (GuideModule.Inst().pvp != null)
            {
                new pps.GSrvPvpNotifyImpl(GuideModule.Inst().pvp, null).Process();
                GuideModule.Inst().pvp = null;
            }
            var script = GuiManager.Inst().GetCachedFrame(typeof(LSharpScript.LoadingFrame).Name) as LSharpScript.LSharpFrame;
            if (script != null && script.IsShow)
            {
                LSharpScript.T.As<LSharpScript.LoadingFrame>(script).DelayHide();
                GuiManager.Inst().BringToTop(script);
            }
            m_manager.CompleteSwitch();

            SceneManager.Inst().terrain.SetCamera(Framework.Instance.camera3d);
        }
    }
    public class SubFightState : IActionState
    {
        ActionStateManager m_manager;
        IList<GuiManager.FrameMeta> m_queuedFrame;
        public static SubFightState Instance { get; private set; }
        public SubFightState() { Instance = this; }
        public void Enter(ActionStateManager manager)
        {
            m_manager = manager;
            SceneManager.Inst().onComplete.Add(OnMapLoaded);
            FightSceneManager.Inst().Enter();
            GuiManager.Inst().RestoreQueuedFrame(m_queuedFrame);
            GuiManager.Inst().ShowFrame(typeof(LSharpScript.LoadingFrame).Name);
        }
        public void Leave()
        {
            m_queuedFrame = GuiManager.Inst().SaveQueuedFrame();
            GuiManager.Inst().HideAll();
            FightSceneManager.Inst().Leave();
            SceneManager.Inst().onComplete.Rmv(OnMapLoaded);
            TcpNetwork.Inst().Stop();
        }
        public void Update() { }
        void OnMapLoaded()
        {
            //TcpNetwork.Inst().Send(new pps.FightMapLoadProgressReport() { value = 100 });
        }     
        public void CompleteSwitch()
        {
            var script = GuiManager.Inst().GetCachedFrame(typeof(LSharpScript.LoadingFrame).Name) as LSharpScript.LSharpFrame;
            if (script != null && script.IsShow)
            {
                LSharpScript.T.As<LSharpScript.LoadingFrame>(script).DelayHide();
                GuiManager.Inst().BringToTop(script);
            }
            m_manager.CompleteSwitch();
            FightSceneManager.Inst().CompleteEnter();
        }
    }
    public class SubPetState : IActionState
    {
        ActionStateManager m_manager;
        IList<GuiManager.FrameMeta> m_queuedFrame;
        public void Enter(ActionStateManager manager)
        {
            m_manager = manager;
            SceneManager.Inst().onComplete.Add(OnMapLoaded);
            PetSceneManager.Inst().Enter();
            //SceneManager.Inst().EnterMap("pet", null, null);
            GuiManager.Inst().RestoreQueuedFrame(m_queuedFrame);
            GuiManager.Inst().ShowFrame(typeof(LSharpScript.LoadingFrame).Name);
        }
        public void Leave()
        {
            m_queuedFrame = GuiManager.Inst().SaveQueuedFrame();
            GuiManager.Inst().HideAll();
            PetSceneManager.Inst().Leave();
            SceneManager.Inst().onComplete.Rmv(OnMapLoaded);
        }
        public void Update() { }
        void OnMapLoaded()
        {
            //if (m_queuedFrame == null) { GuiManager.Inst().PushFrame<ui.PetIslandFrame>(); }
            //var script = GuiManager.Inst().GetCachedLSharpFrame(typeof(LSharpScript.LoadingFrame).Name);
            //var frame = script.GetComponent<ui.GuiFrame>();
            //if (frame != null && frame.IsShow)
            //{
            //    LSharpScript.T.As<LSharpScript.LoadingFrame>(script).DelayHide();
            //    GuiManager.Inst().BringToTop(frame);
            //}
            //m_manager.CompleteSwitch();
        }
    }
    public class SubFriendPetState : IActionState
    {
        ActionStateManager m_manager;
        IList<GuiManager.FrameMeta> m_queuedFrame;
        public void Enter(ActionStateManager manager)
        {
            m_manager = manager;
            SceneManager.Inst().onComplete.Add(OnMapLoaded);
            FriendPetSceneManager.Inst().Enter();
            //SceneManager.Inst().EnterMap("pet", null, null);
            GuiManager.Inst().RestoreQueuedFrame(m_queuedFrame);
            GuiManager.Inst().ShowFrame(typeof(LSharpScript.LoadingFrame).Name);
        }
        public void Leave()
        {
            m_queuedFrame = GuiManager.Inst().SaveQueuedFrame();
            GuiManager.Inst().HideAll();
            FriendPetSceneManager.Inst().Leave();
            SceneManager.Inst().onComplete.Rmv(OnMapLoaded);
        }
        public void Update() { }
        void OnMapLoaded()
        {
            //if (m_queuedFrame == null) { GuiManager.Inst().PushFrame<ui.FriendPetIslandFrame>(); }
            //var script = GuiManager.Inst().GetCachedLSharpFrame(typeof(LSharpScript.LoadingFrame).Name);
            //var frame = script.GetComponent<ui.GuiFrame>();
            //if (frame != null && frame.IsShow)
            //{
            //    LSharpScript.T.As<LSharpScript.LoadingFrame>(script).DelayHide();
            //    GuiManager.Inst().BringToTop(frame);
            //}
            //m_manager.CompleteSwitch();
        }
    }
}