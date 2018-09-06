using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class DropItemPanel : ILSharpScript
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
        int m_moduleId;
        RectTransform m_cachedRc;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            m_cachedRc = (RectTransform)api.transform;
        }
        public void SetCardDrop(Item dp)
        {
            SetCardDrop(dp.mId, dp.count);
        }
        public void SetCardDrop(int mId, int itc)
        {
            m_moduleId = mId;
            if (m_moduleId > 0)
            {
                var itemCfg = geniusbaby.tab.item.Inst().Find(m_moduleId);
                icon.sprite = SpritesManager.Inst().Find(itemCfg.icon);
                count.text = itc.ToString();
            }
            else
            {
                icon.sprite = null;
                count.text = string.Empty;
            }
        }
        public void OnPointerEnter()
        {
            if (m_moduleId > 0)
            {
                //var script = GuiManager.Inst().ShowFrame<TipFrame>();
                //script.DisplayItem(m_cachedRc, m_moduleId);
            }
        }
        public void OnPointerExit()
        {
            //GuiManager.Inst().HideFrame<TipFrame>();
        }
    }
}
