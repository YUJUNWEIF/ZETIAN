using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class AchieveItemPanel : ILSharpScript
    {
//generate code begin
        public Image icon;
        public Text name;
        public Text progress;
        public Text des;
        public Button award;
        public Text finishRoot;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            progress = transform.Find("@progress").GetComponent<Text>();
            des = transform.Find("@des").GetComponent<Text>();
            award = transform.Find("@award").GetComponent<Button>();
            finishRoot = transform.Find("@finishRoot").GetComponent<Text>();
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
        private Achieve m_achieve;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            award.onClick.AddListener(() => HttpNetwork.Inst().Communicate(new AchieveGetAwardRequest() { id = m_achieve.id }));
        }
        public Achieve GetValue() { return m_achieve; }
        public void SetValue(Achieve value)
        {
            OnAchieveUpdate(m_achieve = value, value);
        }
        public override void OnShow()
        {
            base.OnShow();
            AchieveModule.Inst().onAchieveUpdate.Add(OnAchieveUpdate);
        }
        public override void OnHide()
        {
            AchieveModule.Inst().onAchieveUpdate.Rmv(OnAchieveUpdate);
            base.OnHide();
        }
        void OnAchieveUpdate(Achieve up, Achieve old)
        {
            if (m_achieve.id == up.id)
            {
                m_achieve = up;

                var achieveCfg = geniusbaby.tab.achievement.Inst().Find(m_achieve.id);
                var doing = m_achieve.lv < achieveCfg.subs.Count;

                var subAchieveCfg = achieveCfg.subs[doing ? m_achieve.lv : achieveCfg.subs.Count - 1];
                //iconImage.sprite = SpritesManager.Inst().Find(subAchieveCfg.icon);
                name.text = subAchieveCfg.name;
                des.text = subAchieveCfg.des;

                var v = new RangeValue(m_achieve.value, subAchieveCfg.condValue);
                progress.text = doing ? v.ToStyle1() : string.Empty;
                finishRoot.enabled = !doing;
                Util.UnityHelper.ShowHide(award, doing && v.current >= v.max);
            }
        }
    }
}
