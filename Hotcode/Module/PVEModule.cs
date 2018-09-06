using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class PVE
    {
        public int id;
        public bool unlock;
    }
    public class PVEModule : Singleton<PVEModule>, IModule
    {
        public List<PVE> pves { get; private set; }
        public Util.ParamActions onSync = new Util.ParamActions();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void SyncPVEs(List<PVE> pves)
        {
            this.pves = pves;
            onSync.Fire();
        }
        public PVE GetPVE(int id)
        {
            return pves.Find(it => it.id == id);
        }
    }
}
