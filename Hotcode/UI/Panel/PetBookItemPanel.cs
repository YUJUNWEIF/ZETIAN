using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetBookItemPanel : ILSharpScript
    {
//generate code begin
        public Image Quality_icon;
        public Text count;
        public Text name;
        void __LoadComponet(Transform transform)
        {
            Quality_icon = transform.Find("Quality/@icon").GetComponent<Image>();
            count = transform.Find("@count").GetComponent<Text>();
            name = transform.Find("@name").GetComponent<Text>();
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
        PackageItem m_packgeItem;
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
            Quality_icon.sprite = SpritesManager.Inst().Find(itemCfg.icon);
            count.text = m_packgeItem.count.ToString();
            name.text = itemCfg.name;
        }
    }
}
