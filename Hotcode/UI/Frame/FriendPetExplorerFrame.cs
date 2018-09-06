using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FriendPetExplorerFrame : ILSharpScript
    {
//generate code begin
        public Image building_icon;
        public Text building_name;
        public Text building_lv;
        public RectTransform building_Pet_list;
        public RectTransform building_empty;
        public RectTransform building_doing;
        public Image building_doing_bookIcon;
        public Text building_doing_bookName;
        public LSharpListSuper building_doing_Ld_clip_drop;
        public Text building_doing_leftTime;
        public Button building_doing_robbey;
        public Button building_close;
        void __LoadComponet(Transform transform)
        {
            building_icon = transform.Find("building/@icon").GetComponent<Image>();
            building_name = transform.Find("building/@name").GetComponent<Text>();
            building_lv = transform.Find("building/@lv").GetComponent<Text>();
            building_Pet_list = transform.Find("building/Pet/@list").GetComponent<RectTransform>();
            building_empty = transform.Find("building/@empty").GetComponent<RectTransform>();
            building_doing = transform.Find("building/@doing").GetComponent<RectTransform>();
            building_doing_bookIcon = transform.Find("building/@doing/@bookIcon").GetComponent<Image>();
            building_doing_bookName = transform.Find("building/@doing/@bookName").GetComponent<Text>();
            building_doing_Ld_clip_drop = transform.Find("building/@doing/Ld/clip/@drop").GetComponent<LSharpListSuper>();
            building_doing_leftTime = transform.Find("building/@doing/@leftTime").GetComponent<Text>();
            building_doing_robbey = transform.Find("building/@doing/@robbey").GetComponent<Button>();
            building_close = transform.Find("building/@close").GetComponent<Button>();
        }
        void __DoInit()
        {
            building_doing_Ld_clip_drop.OnInitialize();
        }
        void __DoUninit()
        {
            building_doing_Ld_clip_drop.OnUnInitialize();
        }
        void __DoShow()
        {
            building_doing_Ld_clip_drop.OnShow();
        }
        void __DoHide()
        {
            building_doing_Ld_clip_drop.OnHide();
        }
//generate code end
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }

        //PetExplorer m_explorer;
        //Image[] m_petImages;
        ////Spline[] m_splines;
        ////Spline m_spline;
        //public override void OnInitialize()
        //{
        //    base.OnInitialize();
        //    dropPanel.OnInitialize();
        //    m_petImages = petRoot.GetComponentsInChildren<Image>();
        //    //m_splines = GetComponentsInChildren<Spline>();
        //    petButton.onClick.AddListener(() =>
        //    {
        //        if (m_explorer.bookId > 0) { return; }

        //        var options = new List<SelectValue<Pet>>();
        //        var alreadySelects = new List<Pet>();
        //        for (int index = 0; index < PetModule.Inst().pets.Count; ++index)
        //        {
        //            var pet = PetModule.Inst().pets[index];
        //            var used = PetModule.Inst().equipId == pet.uniqueId ||
        //            PetModule.Inst().exploreres.Exists(it => it.slotId != m_explorer.slotId && it.petIds.Contains(pet.uniqueId));

        //            options.Add(new SelectValue<Pet>() { value = pet, canSelect = !used });
        //            if (m_explorer.petIds.Contains(pet.uniqueId))
        //            {
        //                alreadySelects.Add(pet);
        //            }
        //        }
        //        var script = GuiManager.Inst().ShowFrame<PetSelectFrame>();
        //        script.Display(selects =>
        //        {
        //            var slotUp = new pps.PetExplorerSlotUseRequest() { slotId = m_explorer.slotId };
        //            for (int index = 0; index < selects.Count; ++index) { slotUp.petIds.Add(selects[index].uniqueId); }
        //            HttpNetwork.Inst().Communicate(slotUp);
        //        }, options, 3, alreadySelects);
        //    });
        //    bookAddButton.onClick.AddListener(() =>
        //    {
        //        if (m_explorer.petIds.Count > 0)
        //        {
        //            var script = GuiManager.Inst().ShowFrame<PetExplorerAddFrame>();
        //            script.Display(m_explorer);
        //        }
        //        else
        //        {
        //            var script = GuiManager.Inst().ShowFrame<MessageBoxFrame>();
        //            script.SetDesc(GlobalString.Get(cfg.CodeDefine.Pet_ExplorerNoPet), MsgBoxType.Mbt_Ok);
        //        }
        //    });
        //    getButton.onClick.AddListener(() =>
        //    {
        //        HttpNetwork.Inst().Communicate(new pps.PetExplorerGetRequest() { slotId = m_explorer.slotId });
        //    });
        //}
        //public override void OnUnInitialize()
        //{
        //    dropPanel.OnUnInitialize();
        //    base.OnUnInitialize();
        //}
        //public override void OnShow()
        //{
        //    base.OnShow();
        //    dropPanel.OnShow();
        //    PetModule.Inst().onExplorerUpdate.Add(OnExplorerUpdate);
        //    Util.TimerManager.Inst().Add(OnTimer, 1000);
        //}
        //public override void OnHide()
        //{
        //    Util.TimerManager.Inst().Remove(OnTimer);
        //    PetModule.Inst().onExplorerUpdate.Rmv(OnExplorerUpdate);
        //    dropPanel.OnHide();
        //    base.OnHide();
        //}
        //public void Display(PetExplorer explorer)
        //{
        //    OnExplorerUpdate(m_explorer = explorer);
        //}
        //void OnExplorerUpdate(PetExplorer explorer)
        //{
        //    if (m_explorer.slotId == explorer.slotId)
        //    {
        //        m_explorer = explorer;
        //        //m_spline = m_splines[0];
        //        for (int index = 0; index < m_petImages.Length; ++index)
        //        {
        //            m_petImages[index].enabled = index < m_explorer.petIds.Count;
        //            if (index < m_explorer.petIds.Count)
        //            {
        //                var pet = PetModule.Inst().Find(m_explorer.petIds[index]);
        //                var heroCfg = tab.hero.Inst().Find(pet.mId);
        //                var resCfg = tab.objRes.Inst().Find(heroCfg.star.resId);
        //                m_petImages[index].sprite = SpritesManager.Inst().Find(resCfg.icon);
        //            }
        //        }
        //        Util.UnityHelper.ShowHide(emptyRoot, m_explorer.bookId <= 0);
        //        Util.UnityHelper.ShowHide(doingRoot, m_explorer.bookId > 0);

        //        if (m_explorer.bookId > 0)
        //        {
        //            var itemCfg = tab.item.Inst().Find(m_explorer.bookId);
        //            bookImage.sprite = SpritesManager.Inst().Find(itemCfg.icon);
        //            bookText.text = itemCfg.name;
        //        }
        //        OnTimer();
        //    }
        //}
        //void OnTimer()
        //{
        //    if (m_explorer.bookId <= 0) { return; }

        //    var itemCfg = tab.item.Inst().Find(m_explorer.bookId);
        //    var bookCfg = tab.heroExplorerMap.Inst().Find(int.Parse(itemCfg.param));
        //    var ms = m_explorer.endAt * 1000 - Util.TimerManager.Inst().RealTimeMS();
        //    if (ms < 0) { ms = 0; }
        //    bool complete = (ms <= 0);
        //    Util.UnityHelper.ShowHide(getButton, complete);
        //    Util.UnityHelper.ShowHide(leftTimeText, !complete);
        //    if (!complete)
        //    {
        //        var ts = TimeSpan.FromMilliseconds(ms);
        //        leftTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        //    }
        //    //var percent = (1f - ms * 1.0f / bookCfg.timeMs);
        //    //boatRoot.transform.position = m_spline.GetPositionOnSpline(percent);
        //    //boatRoot.transform.rotation = m_spline.GetOrientationOnSpline(percent);

        //    var sb = new StringBuilder();
        //    //for (int index = 0; index < bookCfg.randAwardCount; ++index)
        //    //{
        //    //    var award = m_explorer.randAwards[index];
        //    //    if (bookCfg.timeMs - ms >= award.timeMs) { sb.Append(bookCfg.randDrop[award.index].des).Append("\n"); }
        //    //}
        //    progressText.text = sb.ToString();
        //}
    }
}
