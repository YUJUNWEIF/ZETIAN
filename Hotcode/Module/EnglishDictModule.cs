using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class EnglishDictModule : Singleton<EnglishDictModule>, IModule
    {
        public Util.ParamActions onQuery = new Util.ParamActions();
        public geniusbaby.pps.WordQueryResponse query { get; private set; }
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }

        public void Sychronize(geniusbaby.pps.WordQueryResponse query)
        {
            this.query = query;
            onQuery.Fire();
        }
    }
}
