using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ShopItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text icon_count;
        public Image icon_costType;
        public Text icon_costValue;
        public Image icon_sellout;
        public Image lockRoot;
        public Text lockRoot_des;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            icon_count = transform.Find("@icon/@count").GetComponent<Text>();
            icon_costType = transform.Find("@icon/@costType").GetComponent<Image>();
            icon_costValue = transform.Find("@icon/@costValue").GetComponent<Text>();
            icon_sellout = transform.Find("@icon/@sellout").GetComponent<Image>();
            lockRoot = transform.Find("@lockRoot").GetComponent<Image>();
            lockRoot_des = transform.Find("@lockRoot/@des").GetComponent<Text>();
        }
        void __DoInit()
        {
        }
        void __DoUninit()
        {
        }
        void __DoShow()
        {
        }
        void __DoHide()
        {
        }
//generate code end
        ShopItem m_shopItem;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (m_shopItem.shopId > 0)
                {
                    var com = api.GetComponent<LSharpItemPanel>();
                    com.ListComponent.SelectIndex(com.index);
                }
            });
        }
        public ShopItem GetValue() { return m_shopItem; }
        public void SetValue(ShopItem value)
        {
            m_shopItem = value;
            if (m_shopItem.shopId > 0)
            {
                icon.gameObject.SetActive(true);
                lockRoot.gameObject.SetActive(false);

                var libCfg = geniusbaby.tab.shop.Inst().Find(m_shopItem.shopId);
                var shopCfg = libCfg.libs.Find(it => it.itemId == m_shopItem.itemId);
                var itemCfg = geniusbaby.tab.item.Inst().Find(shopCfg.itemId);
                icon.sprite = SpritesManager.Inst().Find(string.IsNullOrEmpty(shopCfg.icon) ? itemCfg.icon : shopCfg.icon);
                var costCfg = geniusbaby.tab.item.Inst().Find(shopCfg.cost.id);
                icon_costType.sprite = SpritesManager.Inst().Find(costCfg.icon);
                icon_costValue.text = shopCfg.cost.count.ToString();
                OnUpdate(m_shopItem);
            }
            else
            {
                icon.gameObject.SetActive(false);
                lockRoot.gameObject.SetActive(true);
            }
        }
        public override void OnShow()
        {
            base.OnShow();
            ShopModule.Inst().onUpdate.Add(OnUpdate);
        }
        public override void OnHide()
        {
            ShopModule.Inst().onUpdate.Rmv(OnUpdate);
            base.OnHide();
        }
        void OnUpdate(ShopItem shopItem)
        {
            if (shopItem.shopId > 0 && m_shopItem.shopId == shopItem.shopId)
            {
                m_shopItem = shopItem;
                icon_sellout.enabled = m_shopItem.sellout;
            }
        }
    }
}
