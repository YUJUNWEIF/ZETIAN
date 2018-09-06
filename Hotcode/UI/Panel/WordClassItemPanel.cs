using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordClassItemPanel : ILSharpScript
    {
//generate code begin
        public Text name;
        public Text newly;
        public Image locked;
        public RectTransform star;
        void __LoadComponet(Transform transform)
        {
            name = transform.Find("@name").GetComponent<Text>();
            newly = transform.Find("@newly").GetComponent<Text>();
            locked = transform.Find("@locked").GetComponent<Image>();
            star = transform.Find("@star").GetComponent<RectTransform>();
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
        geniusbaby.cfg.wordClass m_classCfg;
        List<Graphic> m_stars = new List<Graphic>();
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
            star.GetComponentsInChildren<Graphic>(true, m_stars);
        }
        public geniusbaby.cfg.wordClass GetValue() { return m_classCfg; }
        public void SetValue(geniusbaby.cfg.wordClass value)
        {
            m_classCfg = value;
            name.text = m_classCfg.name;

            var classId = KnowledgeModule.Instance.classId;
            var classCfgs = KnowledgeModule.Inst().FindLGAP();
            var exist = classCfgs.FindIndex(it => it.id == classId);
            var com = api.GetComponent<LSharpItemPanel>();
            Util.UnityHelper.ShowHide(newly, com.index == exist + 1);
            Util.UnityHelper.ShowHide(locked, com.index > exist + 1);
            m_stars.ForEach(it => it.enabled = com.index < exist + 1);
        }
    }
}
