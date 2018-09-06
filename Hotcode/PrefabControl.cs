using System;
using System.Collections.Generic;
using geniusbaby.ui;

namespace geniusbaby
{
    public class PrefabControl : Singleton<PrefabControl>, IGameEvent
    {
        public void OnStartGame()
        {
            FuncTipModule.Inst().onFuncTip.Add(OnFuncTipGuide);
        }
        public void OnStopGame()
        { 
            FuncTipModule.Inst().onFuncTip.Rmv(OnFuncTipGuide);
        }
        void OnFuncTipGuide(int subFunc, object param)
        {
            var frame = GuiManager.Inst().ShowFrame(typeof(LSharpScript.FuncTipFrame).Name);
            var script = LSharpScript.T.As<LSharpScript.FuncTipFrame>(frame);
            script.Display(subFunc, param);
        }
    }
}
