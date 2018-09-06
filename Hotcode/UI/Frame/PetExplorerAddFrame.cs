using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PetExplorerAddFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper clip_bookList;
        public Button confirm;
        public Button close;
        public Text MapDetail_Text_typeNeed;
        public Text MapDetail_des;
        public Text MapDetail_Text_time;
        public LSharpListSuper Ld_clip_drop;
        void __LoadComponet(Transform transform)
        {
            clip_bookList = transform.Find("clip/@bookList").GetComponent<LSharpListSuper>();
            confirm = transform.Find("@confirm").GetComponent<Button>();
            close = transform.Find("@close").GetComponent<Button>();
            MapDetail_Text_typeNeed = transform.Find("MapDetail/Text/@typeNeed").GetComponent<Text>();
            MapDetail_des = transform.Find("MapDetail/@des").GetComponent<Text>();
            MapDetail_Text_time = transform.Find("MapDetail/Text/@time").GetComponent<Text>();
            Ld_clip_drop = transform.Find("Ld/clip/@drop").GetComponent<LSharpListSuper>();
        }
        void __DoInit()
        {
            clip_bookList.OnInitialize();
            Ld_clip_drop.OnInitialize();
        }
        void __DoUninit()
        {
            clip_bookList.OnUnInitialize();
            Ld_clip_drop.OnUnInitialize();
        }
        void __DoShow()
        {
            clip_bookList.OnShow();
            Ld_clip_drop.OnShow();
        }
        void __DoHide()
        {
            clip_bookList.OnHide();
            Ld_clip_drop.OnHide();
        }
//generate code end
        PetExplorer m_explorer;
        PackageItem m_packageItem;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            clip_bookList.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(PetBookItemPanel).Name);
            Ld_clip_drop.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(DropItemPanel).Name);
            __DoInit();
            clip_bookList.selectListener.Add(() =>
            {
                var no = clip_bookList.itemSelected.First;
                if (no != null)
                {
                    m_packageItem = (PackageItem)clip_bookList.values[no.Value];
                    m_explorer.bookId = m_packageItem.stackId;
                    var itemCfg = geniusbaby.tab.item.Inst().Find(m_packageItem.mId);
                    var bookCfg = geniusbaby.tab.heroExplorerMap.Inst().Find(int.Parse(itemCfg.param));
                    //FEMath.SortFixUnityBugAndNotStable(bookCfg.needs, (x, y) =>
                    //{
                    //    if (x.petType == y.petType) { return 0; }
                    //    else if (x.petType == cfg.petType._) { return 1; }
                    //    else if (y.petType == cfg.petType._) { return -1; }
                    //    return x.petType.CompareTo(y.petType);
                    //});

                    var ts = TimeSpan.FromSeconds(bookCfg.timeSec);
                    MapDetail_Text_time.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                    MapDetail_des.text = itemCfg.des;
                }
                else
                {
                    m_explorer.bookId = 0;
                }
            });
            confirm.onClick.AddListener(() =>
            {
                if (m_explorer.bookId <= 0) { return; }

                GuiManager.Inst().HideFrame(api.name);
                HttpNetwork.Inst().Communicate(new PetExplorerRequest() { slotId = m_explorer.slotId, stackId = m_explorer.bookId });
            });
            close.onClick.AddListener(() => GuiManager.Inst().HideFrame(api.name));
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
        public void Display(PetExplorer explorer)
        {
            m_explorer = explorer;
            var books = new List<object>();
            var items = PackageModule.Inst().items;
            for (int index = 0; index < items.Count; ++index)
            {
                if(geniusbaby.tab.item.Inst().Find(items[index].mId).type == ItemType.Explorer)
                {
                    books.Add(items[index]);
                }
            }
            clip_bookList.SetValues(books);
            clip_bookList.SelectIndex(0);
        }
    }
}
