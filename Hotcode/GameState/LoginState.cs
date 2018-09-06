using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public class LoginState : IActionState
    {
        public void Enter(ActionStateManager manager)
        {
            ModuleManager.Instance.Login();
            MusicManager.Instance.CrossFadeTo(GamePath.music.musicLogin);
            GuiManager.Instance.ShowFrame(typeof(LSharpScript.StartGameFrame).Name);
            manager.CompleteSwitch();
        }
        public void Leave()
        {
            GuiManager.Instance.Uninitialize();
            ModuleManager.Instance.Logout();
        }
        public void Update() { }        
    }
}