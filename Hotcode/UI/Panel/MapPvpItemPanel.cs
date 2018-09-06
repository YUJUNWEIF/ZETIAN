using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MapPvpItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
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
        int m_mapId;
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
        public int GetValue() { return m_mapId; }
        public void SetValue(int value)
        {
            var mapCfg = geniusbaby.tab.map.Inst().Find(m_mapId = value);
            icon.sprite = SpritesManager.Inst().Find(mapCfg.icon);
            name.text = mapCfg.name;
        }
    }
}
