using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ShopFrame : ILSharpScript
    {
//generate code begin
        public Text BG_tickDown;
        public LSharpListSuper BG_clip_list;
        public Image BG_detail_icon;
        public Text BG_detail_icon_name;
        public Text BG_detail_des;
        public Image BG_detail_cost_type;
        public Text BG_detail_cost_value;
        public Button BG_detail_buy;
        void __LoadComponet(Transform transform)
        {
            BG_tickDown = transform.Find("BG/@tickDown").GetComponent<Text>();
            BG_clip_list = transform.Find("BG/clip/@list").GetComponent<LSharpListSuper>();
            BG_detail_icon = transform.Find("BG/detail/@icon").GetComponent<Image>();
            BG_detail_icon_name = transform.Find("BG/detail/@icon/@name").GetComponent<Text>();
            BG_detail_des = transform.Find("BG/detail/@des").GetComponent<Text>();
            BG_detail_cost_type = transform.Find("BG/detail/cost/@type").GetComponent<Image>();
            BG_detail_cost_value = transform.Find("BG/detail/cost/@value").GetComponent<Text>();
            BG_detail_buy = transform.Find("BG/detail/@buy").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            BG_clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_clip_list.OnShow();
        }
        void __DoHide()
        {
            BG_clip_list.OnHide();
        }
//generate code end
        ShopItem m_shopItem;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(ShopItemPanel).Name);
            __DoInit();

            BG_clip_list.selectListener.Add((Action)(() =>
            {
                var no = BG_clip_list.itemSelected.First;
                if (no != null)
                {
                    Display(this.m_shopItem = (ShopItem)BG_clip_list.values[no.Value]);
                }
            }));
            BG_detail_buy.onClick.AddListener(() =>
            {
                if (!m_shopItem.sellout)
                {
                    HttpNetwork.Inst().Communicate(new ShopBuyRequest() { slotId = m_shopItem.slotId });
                }
            });
        }
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }

        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
            ShopModule.Inst().onSync.Add(OnSync);
            TokenModule.Inst().onSync.Add(OnSync);
            Util.TimerManager.Inst().Add(OnTimer, 1000);
            OnSync();
            BG_clip_list.SelectIndex(0);
            OnTimer();
        }

        public override void OnHide()
        {
            ShopModule.Inst().onSync.Rmv(OnSync);
            TokenModule.Inst().onSync.Rmv(OnSync);
            Util.TimerManager.Inst().Remove(OnTimer);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            var shopItems = ShopModule.Inst().shopItems;
            BG_clip_list.SetValues(shopItems.ConvertAll(it =>(object)it));
            BG_clip_list.SelectIndex(0);
        }
        void OnTimer()
        {
            var now = Util.TimerManager.Inst().RealTimeMS() / 1000;
            var tickdown = ShopModule.Inst().tickdown - now;
            var ts = TimeSpan.FromSeconds(tickdown > 0 ? tickdown : 0);
            BG_tickDown.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            if (tickdown < 0 )
            {
                //send proto to refresh
            }
        }
        void Display(ShopItem shopItem)
        {
            var libCfg = geniusbaby.tab.shop.Inst().Find(m_shopItem.shopId);
            var shopCfg = libCfg.libs.Find(it => it.itemId == m_shopItem.itemId);
            var itemCfg = geniusbaby.tab.item.Inst().Find(shopCfg.itemId);

            var icon = shopCfg.icon;
            if (string.IsNullOrEmpty(shopCfg.icon)) { icon = itemCfg.icon; }
            BG_detail_icon.sprite = SpritesManager.Inst().Find(icon);
            BG_detail_icon_name.text = shopCfg.name;
            BG_detail_des.text = shopCfg.des;
            if (string.IsNullOrEmpty(shopCfg.des)) { BG_detail_des.text = itemCfg.des; }

            var costTypeCfg = tab.item.Inst().Find(shopCfg.cost.id);
            BG_detail_cost_type.sprite = SpritesManager.Inst().Find(costTypeCfg.icon);
            BG_detail_cost_value.text = shopCfg.cost.count.ToString();
            var token = TokenModule.Inst().Get(shopCfg.cost.id);
            BG_detail_cost_value.color = token >= shopCfg.cost.count ? Color.green : Color.red;
        }
    }
}
