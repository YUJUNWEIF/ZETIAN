using UnityEngine;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using DeJson;
using geniusbaby;
using geniusbaby.archive;

namespace geniusbaby
{
    public class LoginModule : Singleton<LoginModule>, IModule
    {
        private Account m_account;
        private bool m_isLogin;
        public Util.ParamActions onAccount = new Util.ParamActions();
        public Account account { get { return m_account; } }
        public bool isLogin { get { return m_isLogin; } }
        public void Login(Account account)
        {
            m_account = account;
            m_isLogin = true;
            onAccount.Fire();
        }
        public void Logout()
        {
            m_isLogin = false;
            onAccount.Fire();
        }
        public void Forget()
        {
            if (m_account != null)
            {
                SQLiteTableManager.account.Remove(0);
                m_account = null;
                onAccount.Fire();
            }
        }
        public void OnLogin() { Load(); }
        public void OnLogout() { Save(); }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Load()
        {
            m_account = SQLiteTableManager.account.Get(0);
        }
        public void Save()
        {
            if (m_account != null)
            {
                SQLiteTableManager.account.Update(0, m_account);
            }
        }
    }
}
