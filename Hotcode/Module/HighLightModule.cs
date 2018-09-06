using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct SubFunction
    {
        public const int TitleLvUp = Function.Title + 1;
        public const int TitleOpen = Function.Title + 2;
        public const int GunOpen = Function.Gun + 1;
        public const int GunLvUp = Function.Gun + 2;
        public const int AchieveSatified = Function.Achieve + 1;
    }

    public class HighLight
    {
        public int moduleId;
        public bool highLight;
        public List<HighLight> children = new List<HighLight>();
        public bool NeedHighLight()
        {
            if (children != null && children.Count > 0)
            {
                return children.Exists(it => it.NeedHighLight());
            }
            return highLight;
        }
    }
    public class FuncTipModule : Singleton<FuncTipModule>, IModule
    {
        interface IHighLight
        {
            void Initialize();
            void UnInitialize();
            void OnTimer();
        }
        interface IFuncGuide
        {
            void Initialize();
            void UnInitialize();
        }
        class HL_Form : IHighLight
        {
            public void Initialize()
            {
                MailModule.Instance.onMailSync.Add(OnMailSync);
                MailModule.Instance.onMailUpdate.Add(OnMailUpdate);
                OnMailSync();
            }
            public void UnInitialize()
            {
                MailModule.Instance.onMailSync.Rmv(OnMailSync);
                MailModule.Instance.onMailUpdate.Rmv(OnMailUpdate);
            }
            public void OnTimer() { }
            void OnMailSync()
            {
                bool newMailTip = MailModule.Instance.mails.Exists(it => it.hasRead);
                FuncTipModule.Instance.CheckFuncTip(newMailTip, Function.Form);
            }
            void OnMailUpdate(Mail form) { OnMailSync(); }
        }
        class HL_Friend : IHighLight
        {
            public void Initialize()
            {
                FriendModule.Instance.onSync.Add(OnSyncFriend);
                OnSyncFriend();
            }
            public void UnInitialize()
            {
                FriendModule.Instance.onSync.Rmv(OnSyncFriend);
            }
            public void OnTimer() { }
            void OnSyncFriend()
            {
                bool newApply = FriendModule.Instance.friends.Exists(it => it.type == Friend.Type.NeedSelfConfirm);
                FuncTipModule.Instance.CheckFuncTip(newApply, Function.Friend);
            }
        }
        
        class FG_Title : IFuncGuide
        {
            public void Initialize()
            {
                TitleModule.Inst().onTitleSync.Add(OnSync);
            }
            public void UnInitialize()
            {
                TitleModule.Inst().onTitleSync.Rmv(OnSync);
            }
            void OnSync() { }
            void OnUpdate(Title up, Title old)
            {
                //if (up.id != old.id || up.lv != old.lv) { FuncTipModule.Inst().onFuncTip.Fire(SubFunction.TitleLvUp, up); }
            }
        }
        class FG_Gun: IFuncGuide
        {
            public void Initialize()
            {
                GunModule.Inst().onGunSync.Add(OnSync);
                GunModule.Inst().onGunUpdate.Add(OnUpdate);
            }
            public void UnInitialize()
            {
                GunModule.Inst().onGunSync.Rmv(OnSync);
                GunModule.Inst().onGunUpdate.Rmv(OnUpdate);
            }
            void OnSync() { }
            void OnUpdate(Gun up, Gun old)
            {
                if (old.locked)
                {
                    //if (!up.locked) { FuncTipModule.Inst().onFuncTip.Fire(SubFunction.GunOpen, up); }
                }
                else
                {
                    //if (up.exp.current != old.exp.current && up.exp.current >= up.exp.max) { FuncTipModule.Inst().onFuncTip.Fire(SubFunction.GunLvUp, up); }
                }
            }
        }
        class FG_Achive : IFuncGuide
        {
            public void Initialize()
            {
                AchieveModule.Inst().onAchieveUpdate.Add(OnUpdate);
            }
            public void UnInitialize()
            {
                AchieveModule.Inst().onAchieveUpdate.Rmv(OnUpdate);
            }
            void OnUpdate(Achieve up, Achieve old)
            {
                if (up.lv != old.lv) { FuncTipModule.Inst().onFuncTip.Fire(SubFunction.AchieveSatified, up); }
            }
        }
        List<IHighLight> m_modules = new List<IHighLight>();
        List<IFuncGuide> m_guides = new List<IFuncGuide>();
        List<HighLight> m_highLight = new List<HighLight>();
        public Util.Param1Actions<int[]> onHighLight = new Util.Param1Actions<int[]>();
        public Util.Param2Actions<int, object> onFuncTip = new Util.Param2Actions<int, object>();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter()
        {
            m_modules.Add(new HL_Form());
            m_modules.Add(new HL_Friend());
            m_modules.ForEach(it => it.Initialize());

            m_guides.Add(new FG_Title());
            m_guides.Add(new FG_Gun());
            m_guides.Add(new FG_Achive());
            m_guides.ForEach(it => it.Initialize());
            Util.TimerManager.Instance.Add(OnTimer, 100);
        }
        public void OnMainExit()
        {
            Util.TimerManager.Instance.Remove(OnTimer);
            m_modules.ForEach(it => it.UnInitialize());
            m_modules.Clear();

            m_guides.ForEach(it => it.UnInitialize());
            m_guides.Clear();
        }
        void OnTimer()
        {
            m_modules.ForEach(it => it.OnTimer());
        }
        public bool GetFuncTip(params int[] modules)
        {
            return GetFuncTip(m_highLight, 0, modules);
        }
        bool GetFuncTip(List<HighLight> data, int moduleIndex, params int[] modules)
        {
            var moduleData = data.Find(it => it.moduleId == modules[moduleIndex]);
            if (moduleData != null)
            {
                if (moduleIndex == modules.Length - 1)
                {
                    return moduleData.NeedHighLight();
                }
                else if (moduleData.children != null)
                {
                    return GetFuncTip(moduleData.children, moduleIndex + 1, modules);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        void CheckFuncTip(bool newState, params int[] modules)
        {
            CheckFuncTip(m_highLight, newState, 0, modules);
        }
        void CheckFuncTip(List<HighLight> data, bool newState, int moduleIndex, params int[] modules)
        {
            var moduleData = data.Find(it => it.moduleId == modules[moduleIndex]);
            if (moduleData == null)
            {
                moduleData = new HighLight() { moduleId = modules[moduleIndex] };
                data.Add(moduleData);
            }
            if (moduleData.NeedHighLight() != newState)
            {
                moduleData.highLight = newState;
                var subModules = new int[moduleIndex + 1];
                System.Array.Copy(modules, 0, subModules, 0, moduleIndex + 1);
                onHighLight.Fire(subModules);
            }
            if (moduleIndex + 1 < modules.Length)
            {
                CheckFuncTip(moduleData.children, newState, moduleIndex + 1, modules);
            }
        }
    }
}
