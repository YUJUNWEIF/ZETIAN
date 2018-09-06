using UnityEngine;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class Item
    {
        public int mId;
        public int count;
        public Item(int mId)
        {
            this.mId = mId;
            count = 0;
        }
    }
    public class PackageItem : Item
    {
        public int stackId;
        public PackageItem(int stackId, int mId) : base(mId)
        {
            this.stackId = stackId;
        }
    }
    public class PackageModule : Singleton<PackageModule>, IModule
    {
        public List<PackageItem> items = new List<PackageItem>();
        public Util.ParamActions onSync = new Util.ParamActions();
        public Util.Param1Actions<PackageItem> onUpdate = new Util.Param1Actions<PackageItem>();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Reset()
        {
            items.Clear();
        }
        public void Sync(List<PackageItem> items)
        {
            this.items = items;
            onSync.Fire();
        }
        public void Update(List<PackageItem> ups)
        {
            bool needSync = false;
            for (int index = 0; index < ups.Count; ++index)
            {
                var exist = items.FindIndex(it => it.stackId == ups[index].stackId);
                if (exist >= 0)
                {
                    onUpdate.Fire(items[exist] = ups[index]);
                }
                else
                {
                    items.Add(ups[index]);
                }
            }
            if (needSync)
            {
                onSync.Fire();
            }
        }
        public void Rmv(int stackId)
        {
            var exist = items.FindIndex(it => it.stackId == stackId);
            if (exist >= 0)
            {
                items.RemoveAt(exist);
                onSync.Fire();
            }
        }
        public PackageItem Find(int stackId)
        {
            return items.Find(it => it.stackId == stackId);
        }
        public Item FindAllWithModuleId(int moduleId)
        {
            var item = new Item(moduleId);
            for (int index = 0; index < items.Count; ++index)
            {
                if (items[index].mId == moduleId) { item.count += items[index].count; }
            }
            return item;
        }
    }
}

