using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ItemUseItemPanel : ILSharpScript
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
        float m_startTime = -1f;
        bool longPress;
        int fibonacci1;
        int fibonacci2;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        private PackageItem m_item;
        public PackageItem GetValue() { return m_item; }
        public void SetValue(PackageItem value)
        {
            OnPackageUpdate(m_item = value);
            var itemCfg = geniusbaby.tab.item.Inst().Find(m_item.mId);
            icon.sprite = SpritesManager.Inst().Find(itemCfg.icon);
        }
        public override void OnShow()
        {
            base.OnShow();
            PackageModule.Inst().onUpdate.Add(OnPackageUpdate);
        }
        public override void OnHide()
        {
            PackageModule.Inst().onUpdate.Rmv(OnPackageUpdate);
            base.OnHide();
        }
        void OnPackageUpdate(PackageItem up)
        {
            if (m_item.stackId == up.stackId)
            {
                m_item = up;
                count.text = m_item.count.ToString();
            }
        }
        public void OnPointerDown()
        {
            //(ListComponent as ItemUseListPanel).OnUse(m_item, 1);
            //m_startTime = -1f;
            //fibonacci1 = 0;
            //fibonacci2 = 1;
            //longPress = true;
        }
        public void OnPointerUp() { longPress = false; }
        public void OnPointerExit() { longPress = false; }
        void Update()
        {
            //if (longPress)
            //{
            //    m_startTime += Time.deltaTime;
            //    if (m_startTime >= 0.5f)
            //    {
            //        var count = fibonacci1 + fibonacci2;
            //        fibonacci1 = fibonacci2;
            //        fibonacci2 = count;
            //        if (count > m_item.count) { count = m_item.count; }
            //        m_startTime = 0f;
            //        (ListComponent as ItemUseListPanel).OnUse(m_item, count);
            //    }
            //}
        }
    }
}
