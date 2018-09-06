using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace geniusbaby.ui
{
    [RequireComponent(typeof(Button))]
    public class BoatFightItemPanel : IItemLogic<BoatFightItemPanel>, IListItemBase<int>
    {
        public Image iconImage;
        int m_value;
        public int AttachValue
        {
            get { return m_value; }
            set
            {
                m_value = value;

            }
        }
        public int index { get; set; }
        public IListBase ListComponent { get; set; }
        public RectTransform cachedRc;
        public override void OnInitialize()
        {
            base.OnInitialize();
            cachedRc = GetComponent<RectTransform>();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                ListComponent.SelectIndex(index);
            });
        }
    }
}