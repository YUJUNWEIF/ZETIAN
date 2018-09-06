using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FrequentItemPanel : ILSharpScript
    {
//generate code begin
        public Text name;
        public Image inuse;
        void __LoadComponet(Transform transform)
        {
            name = transform.Find("@name").GetComponent<Text>();
            inuse = transform.Find("@inuse").GetComponent<Image>();
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
        KnowledgeFreq m_freq;
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
        public KnowledgeFreq GetValue() { return m_freq; }
        public void SetValue(KnowledgeFreq value)
        {
            m_freq = value;
            name.text = m_freq.name;
            OnSync();
        }
        public override void OnShow()
        {
            base.OnShow();
            KnowledgeModule.Inst().onKnowledgeSync.Add(OnSync);
            KnowledgeModule.Inst().onSync.Add(OnSync);
        }
        public override void OnHide()
        {
            KnowledgeModule.Inst().onKnowledgeSync.Rmv(OnSync);
            KnowledgeModule.Inst().onSync.Rmv(OnSync);
            base.OnHide();
        }
        void OnSync()
        {
            var lgcpId = KnowledgeModule.Inst().lgapId;
            Util.UnityHelper.ShowHide(inuse, lgcpId == m_freq.lgapId);
        }
    }
}
