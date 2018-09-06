using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordWrongFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper BG_Lc_Clip_wordlist;
        public RectTransform BG_Lc_bar;
        public Toggle BG_Lc_expand;
        public Text BG_Lc_expand_count;
        public Image BG_Lc_detail;
        public LSharpListSuper BG_Lc_detail_Clip_list;
        public Transform BG_Cc_root;
        public Text BG_Cc_name;
        public RectTransform BG_Cc_star;
        public Button BG_fight;
        public Button BG_close;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_Clip_wordlist = transform.Find("BG/Lc/Clip/@wordlist").GetComponent<LSharpListSuper>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<RectTransform>();
            BG_Lc_expand = transform.Find("BG/Lc/@expand").GetComponent<Toggle>();
            BG_Lc_expand_count = transform.Find("BG/Lc/@expand/@count").GetComponent<Text>();
            BG_Lc_detail = transform.Find("BG/Lc/@detail").GetComponent<Image>();
            BG_Lc_detail_Clip_list = transform.Find("BG/Lc/@detail/Clip/@list").GetComponent<LSharpListSuper>();
            BG_Cc_root = transform.Find("BG/Cc/@root").GetComponent<Transform>();
            BG_Cc_name = transform.Find("BG/Cc/@name").GetComponent<Text>();
            BG_Cc_star = transform.Find("BG/Cc/@star").GetComponent<RectTransform>();
            BG_fight = transform.Find("BG/@fight").GetComponent<Button>();
            BG_close = transform.Find("BG/@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_Lc_Clip_wordlist.OnInitialize();
            BG_Lc_detail_Clip_list.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Lc_Clip_wordlist.OnUnInitialize();
            BG_Lc_detail_Clip_list.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_Clip_wordlist.OnShow();
            BG_Lc_detail_Clip_list.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_Clip_wordlist.OnHide();
            BG_Lc_detail_Clip_list.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_Clip_wordlist.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordPreviewItemPanel).Name);
            BG_Lc_detail_Clip_list.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordWrongItemPanel).Name);
            __DoInit();

            BG_Lc_expand.onValueChanged.AddListener(isOn =>
            {
                BG_Lc_detail.gameObject.SetActive(isOn);
            });
            BG_fight.onClick.AddListener(() =>
            {
                GuiManager.Instance.HideFrame(api.name);
                var bindCfg = tab.mapBind.Inst().Find(it => it.type == 1);       
                var knowledges = new List<ProtoKnowledge>();
                var knowledge = KnowledgeModule.Inst().knowledge;
                knowledge.wrongs.ForEach(it => knowledges.Add(new ProtoKnowledge() { english = it.english, chinese = it.chinese }));
                FightSceneManager.Inst().PVELevelEnter(bindCfg.mapIds[0], 0, knowledges);
            });
            BG_close.onClick.AddListener(() => GuiManager.Inst().HideFrame(api.name));
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

            BG_Lc_expand.isOn = false;

            var knowledge = KnowledgeModule.Inst().knowledge;
            BG_Lc_expand_count.text = knowledge.wrongs.Count.ToString();

            BG_Lc_detail_Clip_list.SetValues(knowledge.wrongs.ConvertAll(it =>(object)it));
            var options = new List<object>();
            if (knowledge.wrongs.Count >= 8)
            {
                var selects = Framework.rand.RandomBetween(0, knowledge.wrongs.Count, 8);
                for (int index = 0; index < selects.Length; ++index)
                {
                    options.Add(knowledge.wrongs[selects[index]]);
                }
            }
            else
            {
                for (int index = 0; index < knowledge.wrongs.Count; ++index)
                {
                    options.Add(knowledge.wrongs[index]);
                }
            }
            BG_Lc_Clip_wordlist.SetValues(options);
            BG_fight.enabled = (knowledge.wrongs.Count >= 8);
        }
        public override void OnHide()
        {
            __DoHide();
            base.OnHide();
        }
    }
}
