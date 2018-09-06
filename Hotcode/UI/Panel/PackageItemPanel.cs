using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PackageItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text count;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            count = transform.Find("@count").GetComponent<Text>();
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
        private PackageItem m_packgeItem;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
        }
        public PackageItem GetValue() { return m_packgeItem; }
        public void SetValue(PackageItem value)
        {
            m_packgeItem = value;
            var itemCfg = geniusbaby.tab.item.Inst().Find(m_packgeItem.mId);
            icon.sprite = SpritesManager.Inst().Find(itemCfg.icon);
            count.text = m_packgeItem.count.ToString();
        }
        public override void OnShow()
        {
            base.OnShow();
            PackageModule.Inst().onUpdate.Add(OnUpdate);
        }
        public override void OnHide()
        {
            PackageModule.Inst().onUpdate.Rmv(OnUpdate);
            base.OnHide();
        }
        void OnUpdate(PackageItem item)
        {
            if (m_packgeItem.stackId == item.stackId)
            {
                SetValue(item);
            }
        }
    }
}
