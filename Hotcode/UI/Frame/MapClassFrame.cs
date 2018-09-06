using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class MapClassFrame : ILSharpScript
    {
//generate code begin
        public Text BG_Lc_switcher_packageName;
        public Button BG_Lc_switcher_open;
        public LSharpListContainer BG_Lc_frequent;
        public Image BG_Lc_map;
        public Text BG_Lc_map_name;
        public LSharpListSuper BG_Cc_Clip_classPanel;
        public Button BG_enter;
        public Button BG_wrong;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_switcher_packageName = transform.Find("BG/Lc/switcher/@packageName").GetComponent<Text>();
            BG_Lc_switcher_open = transform.Find("BG/Lc/switcher/@open").GetComponent<Button>();
            BG_Lc_frequent = transform.Find("BG/Lc/@frequent").GetComponent<LSharpListContainer>();
            BG_Lc_map = transform.Find("BG/Lc/@map").GetComponent<Image>();
            BG_Lc_map_name = transform.Find("BG/Lc/@map/@name").GetComponent<Text>();
            BG_Cc_Clip_classPanel = transform.Find("BG/Cc/Clip/@classPanel").GetComponent<LSharpListSuper>();
            BG_enter = transform.Find("BG/@enter").GetComponent<Button>();
            BG_wrong = transform.Find("BG/@wrong").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_Lc_frequent.OnInitialize();
            BG_Cc_Clip_classPanel.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Lc_frequent.OnUnInitialize();
            BG_Cc_Clip_classPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_frequent.OnShow();
            BG_Cc_Clip_classPanel.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_frequent.OnHide();
            BG_Cc_Clip_classPanel.OnHide();
        }
//generate code end
        geniusbaby.cfg.map mapCfg;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Cc_Clip_classPanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordClassItemPanel).Name);
            __DoInit();

            BG_Lc_frequent.selectListener.Add(() =>
            {
                var no = BG_Lc_frequent.itemSelected.First;
                if (no != null)
                {
                    var freq = (KnowledgeFreq)BG_Lc_frequent.values[no.Value];
                    if (freq.lgapId != KnowledgeModule.Inst().lgapId)
                    {
                        HttpNetwork.Inst().Communicate(new KnowledgeSwitchRequest() { lgapId = freq.lgapId, name = freq.name });
                    }
                }
            });
            BG_Lc_switcher_open.onClick.AddListener(() =>
            {
                GuiManager.Instance.PushFrame(typeof(WordPackageFrame).Name);
            });
            BG_wrong.onClick.AddListener(() =>
            {
                GuiManager.Inst().ShowFrame(typeof(WordWrongFrame).Name);
            });
            BG_enter.onClick.AddListener(() =>
            {
                var classNo = BG_Cc_Clip_classPanel.itemSelected.First;
                if (classNo != null)
                {
                    var frame = GuiManager.Inst().ShowFrame(typeof(MapClassDetailFrame).Name);
                    var script = T.As<MapClassDetailFrame>(frame);
                    var value = (geniusbaby.cfg.wordClass)BG_Cc_Clip_classPanel.values[classNo.Value];
                    bool isNew = value.id > KnowledgeModule.Instance.classId;
                    script.Display(mapCfg, isNew, value);
                }
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
            //dropPanel.OnShow();
            KnowledgeModule.Inst().onSync.Add(OnSync);
            KnowledgeModule.Inst().onFrequentSync.Add(OnFreqSync);
            OnSync();
            OnFreqSync();
            Display();

            //mapPanel.SelectIndex(0);
        }
        public override void OnHide()
        {
            KnowledgeModule.Inst().onSync.Rmv(OnSync);
            KnowledgeModule.Inst().onFrequentSync.Rmv(OnFreqSync);
            __DoHide();
            base.OnHide();
        }
        void OnFreqSync()
        {
            var freqs = KnowledgeModule.Inst().freqs;
            BG_Lc_frequent.SetValues(T.L(freqs));
            var select = freqs.FindIndex(it => it.lgapId == KnowledgeModule.Inst().lgapId);
            if (select >= 0)
            {
                BG_Lc_frequent.SelectIndex(select);
            }
        }
        void OnSync()
        {
            var classId = KnowledgeModule.Instance.classId;
            var classCfgs = KnowledgeModule.Inst().FindLGAP();
            BG_Cc_Clip_classPanel.SetValues(T.L(classCfgs));

            var exist = classCfgs.FindIndex(it => it.id == classId);
            if (exist >= 0)
            {
                BG_Cc_Clip_classPanel.SelectIndex(exist + 1 < classCfgs.Count ? exist + 1 : exist);
            }
            else
            {
                BG_Cc_Clip_classPanel.SelectIndex(0);
            }
        }
        void Display()
        {
            if (GuideModule.Inst().tutoring)
            {
                var guideCfg = GuideModule.Inst().guide.guideCfg;
                mapCfg = geniusbaby.tab.map.Inst().Find(guideCfg.taskType.mapId);
            }
            else
            {
                var bindCfg = geniusbaby.tab.mapBind.Inst().Find(it => it.type == GlobalDefine.GTypeFightSingle);
                mapCfg = geniusbaby.tab.map.Inst().Find(bindCfg.mapIds[0]);
            }
            BG_Lc_map.sprite = SpritesManager.Inst().Find(mapCfg.icon);
            BG_Lc_map_name.text = mapCfg.name;
        }
    }
}
