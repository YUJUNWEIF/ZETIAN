using System;
using System.IO;
using System.Collections.Generic;

namespace geniusbaby
{
    public interface IModule
    {
        void OnLogin();
        void OnLogout();
        void OnMainEnter();
        void OnMainExit();
    }

    public class ModuleManager : Singleton<ModuleManager>
    {
        private List<IModule> m_modules = new List<IModule>();
        private SQLite.SQLiteConnection db;
        public void StartGame()
        {
            //m_modules.Add(new AchieveModule());
            //m_modules.Add(new LoginModule());
            //m_modules.Add(new PlayerModule());
            //m_modules.Add(new GunModule());
            //m_modules.Add(new PackageModule());
            //m_modules.Add(new MapInfoModule());
            //m_modules.Add(new PVEModule());
            //m_modules.Add(new PvpRoomModule());
            //m_modules.Add(new ArchiveModule());
            //m_modules.Add(new PetModule());
            //m_modules.Add(new TitleModule());
            //m_modules.Add(new KnowledgeModule());
            //m_modules.Add(new TokenModule());
            //m_modules.Add(new ChatModule());
            //m_modules.Add(new FriendModule());
            //m_modules.Add(new MailModule());
            //m_modules.Add(new FuncTipModule());
            //m_modules.Add(new GuideModule());
            //m_modules.Add(new ShopModule());
            //m_modules.Add(new EnglishDictModule());
        }
        public void Regist(IModule module)
        {
            m_modules.Add(module);
        }
        public void StopGame()
        {
            m_modules.Clear();
        }
        public void Login()
        {
            for (int index = 0; index < m_modules.Count; ++index)
            {
                try
                {
                    m_modules[index].OnLogin();
                }
                catch (Exception ex)
                {
                    Util.Logger.Instance.Error(ex.Message, ex);
                }
            }
        }
        public void Logout()
        {
            for (int index = 0; index < m_modules.Count; ++index)
            {
                try
                {
                    m_modules[index].OnLogout();
                }
                catch (Exception ex)
                {
                    Util.Logger.Instance.Error(ex.Message, ex);
                }
            }
        }
        public void MainEnter()
        {
            for (int index = 0; index < m_modules.Count; ++index)
            {
                try
                {
                    m_modules[index].OnMainEnter();
                }
                catch (System.Exception ex)
                {
                    Util.Logger.Instance.Error(ex.StackTrace, ex);
                }
            }
        }
        public void MainExit()
        {
            for (int index = 0; index < m_modules.Count; ++index)
            {
                try
                {
                    m_modules[index].OnMainExit();
                }
                catch (System.Exception ex)
                {
                    Util.Logger.Instance.Error(ex.StackTrace, ex);
                }
            }
        }
    }
}
