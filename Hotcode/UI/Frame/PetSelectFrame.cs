using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetSelectFrame : Selector
    {
//generate code begin
        public LSharpListSuper BG_Lc_clip_petPanel;
        public GuiBar BG_Lc_bar;
        public LSharpAPI BG_PetDetailPanel;
        public Button BG_operate_cancel;
        public Button BG_operate_confirm;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_clip_petPanel = transform.Find("BG/Lc/clip/@petPanel").GetComponent<LSharpListSuper>();
            BG_Lc_bar = transform.Find("BG/Lc/@bar").GetComponent<GuiBar>();
            BG_PetDetailPanel = transform.Find("BG/@PetDetailPanel").GetComponent<LSharpAPI>();
            BG_operate_cancel = transform.Find("BG/operate/@cancel").GetComponent<Button>();
            BG_operate_confirm = transform.Find("BG/operate/@confirm").GetComponent<Button>();
        }
        void __DoInit()
        {
            BG_Lc_clip_petPanel.OnInitialize();
            BG_PetDetailPanel.OnInitialize("geniusbaby.LSharpScript.PetDetailPanel");
        }
        void __DoUninit()
        {
            BG_Lc_clip_petPanel.OnUnInitialize();
            BG_PetDetailPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_clip_petPanel.OnShow();
            BG_PetDetailPanel.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_clip_petPanel.OnHide();
            BG_PetDetailPanel.OnHide();
        }
//generate code end
        Action<IList<PetBase>> m_confirm;
        ListSelector<SelectValue<PetBase>> m_selector = new ListSelector<SelectValue<PetBase>>();
        Func<IList<PetBase>, bool> m_confirmChecker;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_clip_petPanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(PetSelectItemPanel).Name);
            __DoInit();
            BG_Lc_clip_petPanel.selectListener.Add(() =>
            {
                var no = BG_Lc_clip_petPanel.itemSelected.Last;
                if (no != null)
                {
                    var pet = (PetBase)BG_Lc_clip_petPanel.values[no.Value];
                    T.As<PetDetailPanel>(BG_PetDetailPanel).Display(pet);
                }
                var selects = new List<PetBase>();
                no = BG_Lc_clip_petPanel.itemSelected.First;
                while (no != null)
                {
                    selects.Add((PetBase)BG_Lc_clip_petPanel.values[no.Value]);
                    no = no.Next;
                }
                if (m_confirmChecker != null)
                {
                    BG_operate_confirm.interactable = m_confirmChecker(selects);
                }
            });
            m_selector.confirm = (selectIndexs) =>
            {
                if (selectIndexs.Count > 0)
                {
                    var selects = new List<PetBase>();
                    for (int index = 0; index < selectIndexs.Count; ++index)
                    {
                        selects.Add((PetBase)BG_Lc_clip_petPanel.values[selectIndexs[index]]);
                    }
                    m_confirm(selects);
                }
            };
        }
        public override Button GetConfirm() { return BG_operate_confirm; }
        public override Button GetCancel() { return BG_operate_cancel; }
        
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
        public void Display(Action<IList<PetBase>> confirm, IList<SelectValue<PetBase>> options, int maxSelectCount, IList<PetBase> alreadySelects)
        {
            m_confirmChecker = null;

            var selects = new List<int>();
            for (int j = 0; j < alreadySelects.Count; ++j)
            {
                var select = alreadySelects[j];
                for (int index = 0; index < options.Count; ++index)
                {
                    if (options[index].value.uniqueId == select.uniqueId) { selects.Add(index); }
                }
            }
            m_selector.alreadySelect = selects;

            m_confirm = confirm;
            m_selector.optionsSelect = options;
            m_selector.singleSelect = maxSelectCount <= 1;
            m_selector.maxSelectCount = maxSelectCount;
            //SetSelector<LSharpItemPanel, LSharpItemPanel, object>(m_selector, BG_Lc_clip_petPanel);
            BG_operate_confirm.interactable = true;
        }
        public void Display(Action<IList<PetBase>> confirm, IList<SelectValue<PetBase>> options, Func<IList<PetBase>, bool> confirmChecker, int maxSelectCount)
        {
            m_confirm = confirm;
            m_confirmChecker = confirmChecker;
            m_selector.optionsSelect = options;
            m_selector.singleSelect = maxSelectCount <= 1;
            m_selector.maxSelectCount = maxSelectCount;
            //SetSelector(m_selector, BG_Lc_clip_petPanel);

            BG_operate_confirm.interactable = m_confirmChecker == null;
        }
    }
}
