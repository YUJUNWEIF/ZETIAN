using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MapClassDetailFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper BG_Lc_Clip_wordlist;
        public RectTransform BG_Lc_bar;
        public Transform BG_Cc_root;
        public Text BG_Cc_name;
        public RectTransform BG_Cc_star;
        public Button BG_fight;
        public Button BG_close;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_Clip_wordlist = transform.Find("BG/Lc/Clip/@wordlist").GetComponent<LSharpListSuper>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<RectTransform>();
            BG_Cc_root = transform.Find("BG/Cc/@root").GetComponent<Transform>();
            BG_Cc_name = transform.Find("BG/Cc/@name").GetComponent<Text>();
            BG_Cc_star = transform.Find("BG/Cc/@star").GetComponent<RectTransform>();
            BG_fight = transform.Find("BG/@fight").GetComponent<Button>();
            BG_close = transform.Find("BG/@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_Lc_Clip_wordlist.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Lc_Clip_wordlist.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_Clip_wordlist.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_Clip_wordlist.OnHide();
        }
//generate code end
        cfg.map m_mapCfg;
        cfg.wordClass m_classCfg;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_Clip_wordlist.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordPreviewItemPanel).Name);
            __DoInit();

            BG_fight.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
                var knowledges = new List<ProtoKnowledge>();
                var w = KnowledgeModule.Inst().FindClass(m_classCfg.id);
                w.words.ForEach(it => knowledges.Add(new ProtoKnowledge() { english = it.english, chinese = it.chinese }));
                FightSceneManager.Inst().PVELevelEnter(m_mapCfg.id, m_classCfg.id, knowledges);
            });
            BG_close.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
            });
        }
        public override void OnUnInitialize()
        {
            __DoUninit();
            base.OnUnInitialize();
        }
        public override void OnShow()
        {
            base.OnShow();
            __DoShow();
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
        public void Display(geniusbaby.cfg.map levelCfg, bool isNew, geniusbaby.cfg.wordClass classCfg)
        {
            m_mapCfg = levelCfg;
            m_classCfg = classCfg;
            BG_Lc_Clip_wordlist.SetValues(classCfg.words.ConvertAll(it =>(object)it));
        }
        public void Display(geniusbaby.cfg.map levelCfg, List<ProtoKnowledge> knowledges)
        {

        }
    }
}
