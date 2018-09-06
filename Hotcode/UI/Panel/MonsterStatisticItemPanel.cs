using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MonsterStatisticItemPanel : ILSharpScript
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
        private Statistic.FishKill m_killedFish;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
        public Statistic.FishKill GetValue() { return m_killedFish; }
        public void SetValue(Statistic.FishKill value)
        {
            m_killedFish = value;
            var fishCfg = geniusbaby.tab.monster.Inst().Find(m_killedFish.moduleId);
            var resCfg = geniusbaby.tab.objRes.Inst().Find(fishCfg.resId);
            icon.sprite = SpritesManager.Inst().Find(resCfg.icon);
            icon.SetNativeSize();
            count.text = m_killedFish.count.ToString();
        }
    }
}
