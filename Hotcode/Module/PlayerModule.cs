using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class Player
    {
        public string id;
        public string name;
        public int lvl;
        public RangeValue exp;
    }
    public class PlayerModule : Singleton<PlayerModule>, IModule
    {
        Player m_player;
        public Player player { get { return m_player; } }
        public Util.ParamActions onSync = new Util.ParamActions();
        public void OnLogin() { }
        public void OnLogout() {  }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Sync(Player player)
        {
            m_player = player;
            onSync.Fire();
        }
        public static string MyId()
        {
            return Instance.player.id;
        }
    }
}

