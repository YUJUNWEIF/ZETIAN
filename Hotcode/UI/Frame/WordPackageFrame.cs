using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordPackageFrame : ILSharpScript
    {
//generate code begin
        public DropdownPanel BG_Lc_grade;
        public LSharpListSuper BG_Lc_Clip_areaPanel;
        public RectTransform BG_Lc_bar;
        public Image BG_icon;
        public Text BG_icon_name;
        public Text BG_icon_des;
        public LSharpListSuper BG_Rc_Clip_packagePanel;
        public RectTransform BG_Rc_bar;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_grade = transform.Find("BG/Lc/@grade").GetComponent<DropdownPanel>();
            BG_Lc_Clip_areaPanel = transform.Find("BG/Lc/Clip/@areaPanel").GetComponent<LSharpListSuper>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<RectTransform>();
            BG_icon = transform.Find("BG/@icon").GetComponent<Image>();
            BG_icon_name = transform.Find("BG/@icon/@name").GetComponent<Text>();
            BG_icon_des = transform.Find("BG/@icon/@des").GetComponent<Text>();
            BG_Rc_Clip_packagePanel = transform.Find("BG/Rc/Clip/@packagePanel").GetComponent<LSharpListSuper>();
            BG_Rc_bar = transform.Find("BG/Rc/@bar").GetComponent<RectTransform>();
        }
        void __DoInit()
        {
            BG_Lc_grade.OnInitialize();
            BG_Lc_Clip_areaPanel.OnInitialize();
            BG_Rc_Clip_packagePanel.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Lc_grade.OnUnInitialize();
            BG_Lc_Clip_areaPanel.OnUnInitialize();
            BG_Rc_Clip_packagePanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_grade.OnShow();
            BG_Lc_Clip_areaPanel.OnShow();
            BG_Rc_Clip_packagePanel.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_grade.OnHide();
            BG_Lc_Clip_areaPanel.OnHide();
            BG_Rc_Clip_packagePanel.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);            
            BG_Lc_Clip_areaPanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordAreaItemPanel).Name);
            BG_Rc_Clip_packagePanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(WordPackageItemPanel).Name);
            __DoInit();

            //wrongButton.onClick.AddListener(() =>
            //{
            //    GuiManager.Inst().ShowFrame<WordWrongFrame>();
            //});
            BG_Lc_grade.selectListener.Add(() =>
            {
                if (BG_Lc_grade.m_select >= 0)
                {
                    var gcp = BG_Lc_grade.values[BG_Lc_grade.m_select];
                    KnowledgeModule.Inst().SwitchViewGrade(gcp.As<GAPInfo>().gradeId);
                }
            });
            BG_Lc_Clip_areaPanel.selectListener.Add(() =>
            {
                var now = BG_Lc_Clip_areaPanel.itemSelected.First;
                if (now != null)
                {
                    var remoteLgapInfo = LGAPManager.Inst().Find(KnowledgeModule.Inst().preview.gradeId);
                    var c = remoteLgapInfo.areaInfos[now.Value];
                    KnowledgeModule.Inst().SwitchViewArea(c.areaId);
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
            KnowledgeModule.Inst().onSwitchLang.Add(OnSwitchLang);
            KnowledgeModule.Inst().onSwitchGrade.Add(OnSwitchGrade);
            KnowledgeModule.Inst().onSwitchArea.Add(OnSwitchArea);
            KnowledgeModule.Inst().onPackageSync.Add(OnPackageSync);
            KnowledgeModule.Inst().onSync.Add(OnSync);

            OnSync();
            LGAPManager.Inst().DownloadGAPInfo(LGAPManager.Inst().localLGAPInfo.meteInfoFile, () =>
            {
                KnowledgeModule.Inst().ResetPreview();
                OnSwitchLang();
                OnSwitchGrade();
                OnSwitchArea();
            });
        }
        public override void OnHide()
        {
            KnowledgeModule.Inst().onSwitchLang.Add(OnSwitchLang);
            KnowledgeModule.Inst().onSwitchGrade.Rmv(OnSwitchGrade);
            KnowledgeModule.Inst().onSwitchArea.Rmv(OnSwitchArea);
            KnowledgeModule.Inst().onPackageSync.Rmv(OnPackageSync);
            KnowledgeModule.Inst().onSync.Rmv(OnSync);
            __DoHide();
            base.OnHide();
        }
        void OnSwitchLang()
        {
            var preview = KnowledgeModule.Inst().preview;
            var gaps = LGAPManager.Inst().RecordArray.ConvertAll(it => new DropdownPanel.Data(it.name, it));
            int selectIndex = gaps.FindIndex(it => it.As<GAPInfo>().gradeId == preview.gradeId);
            BG_Lc_grade.Display(gaps, selectIndex);
        }
        void OnSwitchGrade()
        {
            var preview = KnowledgeModule.Inst().preview;

            var gaps = LGAPManager.Inst().RecordArray;
            var gap = gaps.Find(it => it.gradeId == preview.gradeId);
            if (gap != null)
            {
                int selectIndex = 0;
                var xxs = new List<object>(gap.areaInfos.Count);
                for (int index = 0; index < gap.areaInfos.Count; ++index)
                {
                    xxs.Add(gap);
                    if (gap.areaInfos[index].areaId == preview.areaId) { selectIndex = index; }
                }
                BG_Lc_Clip_areaPanel.SetValues(xxs);
                BG_Lc_Clip_areaPanel.SelectIndex(selectIndex);
            }
        }
        void OnSwitchArea()
        {
            LGAPManager.Inst().ParseAfterDownload(KnowledgeModule.Inst().preview);
            OnPackageSync();
        }
        void OnSync()
        {
            (api as LSharpFrame).__close.interactable = KnowledgeModule.Instance.lgapId > 0;
        }
        void OnPackageSync()
        {
            BG_Rc_Clip_packagePanel.SetValues(geniusbaby.tab.__title.Inst().RecordArray.ConvertAll(it => (object)it));
        }
    }
}
