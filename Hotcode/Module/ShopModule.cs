using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class ShopItem
    {
        public int slotId;
        public int shopId;
        public int itemId;
        public bool onsale;
        public bool sellout;
    }
    public class ShopModule : Singleton<ShopModule>, IModule
    {
        public long tickdown { get; private set; }
        public List<ShopItem> shopItems = new List<ShopItem>();
        public Util.ParamActions onSync = new Util.ParamActions();
        public Util.Param1Actions<ShopItem> onUpdate = new Util.Param1Actions<ShopItem>();
        public Util.ParamActions onShopParam = new Util.ParamActions();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Sync(long tickDown, List<ShopItem> shopItems)
        {
            this.tickdown = tickDown;
            this.shopItems = shopItems;
            onSync.Fire();
        }
        public void Update(long tickDown, List<ShopItem> ups)
        {
            this.tickdown = tickDown;
            onShopParam.Fire();

            for (int index = 0; index < ups.Count; ++index)
            {
                var up = ups[index];
                var exist = shopItems.FindIndex(it => it.shopId == up.shopId && it.itemId == up.itemId);
                if (exist >= 0)
                {
                    onUpdate.Fire(shopItems[exist] = up);
                }
            }
        }
    }
}

