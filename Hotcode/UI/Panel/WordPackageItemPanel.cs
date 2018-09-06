using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordPackageItemPanel : ILSharpScript
    {
//generate code begin
        public Button icon;
        public Image icon_using;
        public Text name;
        public RectTransform bar;
        void __LoadComponet(Transform transform)
        {
            icon = transform.Find("@icon").GetComponent<Button>();
            icon_using = transform.Find("@icon/@using").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            bar = transform.Find("@bar").GetComponent<RectTransform>();
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
        geniusbaby.cfg.__title m_cfg;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
            icon.onClick.AddListener(() =>
            {
                var packageId = LGAP.PackageId(KnowledgeModule.Inst().lgapId);
                if (packageId != m_cfg.id)
                {
                    var lgap = KnowledgeModule.Inst().preview;
                    lgap.packageId = (byte)m_cfg.id;
                    HttpNetwork.Inst().Communicate(new KnowledgeSwitchRequest() { lgapId = lgap.Encode(), name = m_cfg.name });
                }
            });
        }
        public void SetValue(geniusbaby.cfg.__title value)
        {
            m_cfg = value;
            name.text = m_cfg.name;
            OnSync();
        }
        public geniusbaby.cfg.__title GetValue()
        {
            return m_cfg;
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
            var packageId = LGAP.PackageId(KnowledgeModule.Inst().lgapId);
            Util.UnityHelper.ShowHide(icon_using, packageId == m_cfg.id);
        }
    }
}
