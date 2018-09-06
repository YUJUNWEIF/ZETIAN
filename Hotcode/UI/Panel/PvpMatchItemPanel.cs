using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PvpMatchItemPanel : ILSharpScript
    {
//generate code begin
        public Text Player_name;
        public Text Player_lv;
        public Image Player_pvp;
        public Transform Pet_root;
        public Text Pet_name;
        public RectTransform Pet_star;
        void __LoadComponet(Transform transform)
        {
            Player_name = transform.Find("Player/@name").GetComponent<Text>();
            Player_lv = transform.Find("Player/@lv").GetComponent<Text>();
            Player_pvp = transform.Find("Player/@pvp").GetComponent<Image>();
            Pet_root = transform.Find("Pet/@root").GetComponent<Transform>();
            Pet_name = transform.Find("Pet/@name").GetComponent<Text>();
            Pet_star = transform.Find("Pet/@star").GetComponent<RectTransform>();
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
        Image[] m_stars;
        ProtoDetailPlayer m_value;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            m_stars = Pet_star.GetComponentsInChildren<Image>(false);
        }
        public ProtoDetailPlayer GetValue() { return m_value; }
        public void SetValue(ProtoDetailPlayer value)
        {
            m_value = value;
            Player_name.text = m_value.name;
            Player_lv.text = m_value.lv.ToString();
            //var lgcp = LGCP.Decode(m_value.brief.packageId);
            //var packageCfg = tab.wordPackageSub.Inst().Find(lgcp.packageId);
            //wordPackText.text = packageCfg.packageName;            
        }
    }
}
