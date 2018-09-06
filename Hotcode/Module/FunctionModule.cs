using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct Function
    {
        public const int Unknown = 0;
        public const int PVE = 100;
        public const int Pet = 200;
        public const int PetEntry = 201;
        public const int PetHatch = 210;
        public const int PetMerge = 211;
        public const int PetExplorer = 212;
        public const int Pacakge = 300;
        public const int Title = 400;
        public const int Gun = 500;
        public const int Achieve = 600;
        public const int Form = 700;
        public const int Friend = 800;
        public const int Quiz = 900;
    }

    public class FunctionModule : Singleton<FunctionModule>, IModule
    {
        Dictionary<int, bool> highlightTips = new Dictionary<int, bool>();
        public List<int> lockeds = new List<int>();
        public Util.ParamActions onSync = new Util.ParamActions();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { highlightTips.Clear(); }
        
        public void Lock(List<int> funcs)
        {
            lockeds = funcs;
        }
        public void Open(List<int> funcs)
        {
            for (int index = 0; index < funcs.Count; ++index)
            {
                var func = funcs[index];
                lockeds.Remove(func);
                if (!highlightTips.ContainsKey(func)) { highlightTips.Add(func, true); }
            }
            onSync.Fire();
        }
        public bool NeedDialog(int func)
        {
            bool need = false;
            if (highlightTips.TryGetValue(func, out need))
            {
                highlightTips[func] = false;
            }
            return need;
        }
        public bool NeedHighLight(int func)
        {
            return highlightTips.ContainsKey(func);
        }
        public bool RemoveHighLight(int func)
        {
            if (highlightTips.Remove(func)) { onSync.Fire(); return true; }
            return false;
        }
    }
}