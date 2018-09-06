using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct Title
    {
        public int id;
        public long endAtMs;
        public bool isNew;
    }
    public class TitleModule : Singleton<TitleModule>, IModule
    {
        public Util.ParamActions onTitleSync = new Util.ParamActions();
        public Util.ParamActions onBuffSync = new Util.ParamActions();
        public Title title { get; private set; }
        public RangeValue exp { get; private set; }
        public List<Title> buffs { get; private set; }
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Sync(int mId, int lv, int exp)
        {
            //this.title = new Title() { id = mId, lv = lv };
            //var titleCfg = tab.title.Inst().Find(mId);
            //var levelCfg = titleCfg.levels[lv];
            //this.exp = new RangeValue(exp, levelCfg.exp);
            onTitleSync.Fire();
        }
        public void Sync(List<Title> buffs)
        {
            this.buffs = buffs;
            onBuffSync.Fire();
        }
        public void Update(List<Title> ups)
        {
            for (int index = 0; index < ups.Count; ++index)
            {
                var up = ups[index];
                var exist = buffs.FindIndex(it => it.id == up.id);
                if (exist >= 0)
                {
                    buffs[exist] = up;
                }
                else
                {
                    buffs.Add(up);
                }
            }
            onBuffSync.Fire();
        }
    }
}
