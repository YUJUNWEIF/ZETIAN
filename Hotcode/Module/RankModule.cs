using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class Rank
    {
        public string uId;
        public string name;
        public int rank;
        public int value;
    }
    public class RankModule : Singleton<RankModule>, IModule
    {
        public long utcRefreshAt { get; private set; }
        public List<Rank> powerTop = new List<Rank>();
        public int myPowerRank { get; private set; }
        public Util.ParamActions onSync = new Util.ParamActions();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Synchronize(long utcRefreshAt, List<Rank> powerTop, int myPowerRank)
        {
            this.utcRefreshAt = utcRefreshAt;
            this.powerTop = powerTop;
            this.myPowerRank = myPowerRank;
            onSync.Fire();
        }
    }
}