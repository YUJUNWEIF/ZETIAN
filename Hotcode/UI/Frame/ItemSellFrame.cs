using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ItemSellFrame : ILSharpScript
    {
//generate code begin
        public Image Image_icon;
        public Text Image_name;
        public Text Image_des;
        public Slider Image_counter;
        public Text Image_counter_count;
        public Button Image_counter_add;
        public Button Image_counter_sub;
        public Button Image_confirm;
        public Button Image_cancel;
        public Image Image_Cost_costType;
        public Text Image_Cost_costValue;
        void __LoadComponet(Transform transform)
        {
            Image_icon = transform.Find("Image/@icon").GetComponent<Image>();
            Image_name = transform.Find("Image/@name").GetComponent<Text>();
            Image_des = transform.Find("Image/@des").GetComponent<Text>();
            Image_counter = transform.Find("Image/@counter").GetComponent<Slider>();
            Image_counter_count = transform.Find("Image/@counter/@count").GetComponent<Text>();
            Image_counter_add = transform.Find("Image/@counter/@add").GetComponent<Button>();
            Image_counter_sub = transform.Find("Image/@counter/@sub").GetComponent<Button>();
            Image_confirm = transform.Find("Image/@confirm").GetComponent<Button>();
            Image_cancel = transform.Find("Image/@cancel").GetComponent<Button>();
            Image_Cost_costType = transform.Find("Image/Cost/@costType").GetComponent<Image>();
            Image_Cost_costValue = transform.Find("Image/Cost/@costValue").GetComponent<Text>();
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
        PackageItem m_item;
        RangeValue m_counter;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            Image_counter.onValueChanged.AddListener(percent =>
            {
                SetCount(Mathf.RoundToInt(percent * 100));
            });
            Image_counter_add.onClick.AddListener(() => SetCount(m_counter.current + 1));
            Image_counter_sub.onClick.AddListener(() => SetCount(m_counter.current - 1));

            Image_confirm.onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                HttpNetwork.Inst().Communicate(new ItemSellRequest() { stackId = m_item.stackId, count = m_counter.current });
            });
            Image_cancel.onClick.AddListener(() => GuiManager.Instance.HideFrame(api.name));
        }
        public void Sell(PackageItem item)
        {
            m_item = item;
            m_counter = new RangeValue(0, item.count);

            var itemCfg = geniusbaby.tab.item.Inst().Find(m_item.mId);
            Image_icon.sprite = SpritesManager.Inst().Find(itemCfg.icon);
            Image_name.text = itemCfg.name;
            Image_des.text = itemCfg.des;
            var costTypeCfg = geniusbaby.tab.item.Inst().Find(itemCfg.sellPrice.id);
            Image_Cost_costType.sprite = SpritesManager.Inst().Find(costTypeCfg.icon);

            SetCount(0);
        }

        void SetCount(int count)
        {
            if (count < 0) { count = 0; }
            if (count > m_item.count) { count = m_item.count; }
            m_counter.current = count;

            var itemCfg = geniusbaby.tab.item.Inst().Find(m_item.mId);
            Image_counter_count.text = m_counter.ToStyle1();
            Image_Cost_costValue.text = (m_counter.current * itemCfg.sellPrice.count).ToString();

            var percent = m_counter.current * 1f / m_counter.max;
            if (Mathf.RoundToInt(percent * 100) != Mathf.RoundToInt(Image_counter.value * 100))
            {
                Image_counter.value = percent;
            }
        }
    }
}
