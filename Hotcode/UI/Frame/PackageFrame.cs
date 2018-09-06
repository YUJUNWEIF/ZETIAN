using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PackageFrame : ILSharpScript
    {
//generate code begin
        public LSharpListSuper BG_Lc_packagePanel;
        public Image BG_Rc_icon;
        public Text BG_Rc_icon_count;
        public Text BG_Rc_icon_name;
        public Text BG_Rc_des;
        public Button BG_Rc_sell;
        public Image BG_Rc_sell_costType;
        public Text BG_Rc_sell_costValue;
        public Button BG_Rc_use;
        public Button BG_Rc_sort;
        public Image BG_Rc_emptyRoot;
        void __LoadComponet(Transform transform)
        {
            BG_Lc_packagePanel = transform.Find("BG/Lc/@packagePanel").GetComponent<LSharpListSuper>();
            BG_Rc_icon = transform.Find("BG/Rc/@icon").GetComponent<Image>();
            BG_Rc_icon_count = transform.Find("BG/Rc/@icon/@count").GetComponent<Text>();
            BG_Rc_icon_name = transform.Find("BG/Rc/@icon/@name").GetComponent<Text>();
            BG_Rc_des = transform.Find("BG/Rc/@des").GetComponent<Text>();
            BG_Rc_sell = transform.Find("BG/Rc/@sell").GetComponent<Button>();
            BG_Rc_sell_costType = transform.Find("BG/Rc/@sell/@costType").GetComponent<Image>();
            BG_Rc_sell_costValue = transform.Find("BG/Rc/@sell/@costValue").GetComponent<Text>();
            BG_Rc_use = transform.Find("BG/Rc/@use").GetComponent<Button>();
            BG_Rc_sort = transform.Find("BG/Rc/@sort").GetComponent<Button>();
            BG_Rc_emptyRoot = transform.Find("BG/Rc/@emptyRoot").GetComponent<Image>();
        }
        void __DoInit()
        {
            BG_Lc_packagePanel.OnInitialize();
        }
        void __DoUninit()
        {
            BG_Lc_packagePanel.OnUnInitialize();
        }
        void __DoShow()
        {
            BG_Lc_packagePanel.OnShow();
        }
        void __DoHide()
        {
            BG_Lc_packagePanel.OnHide();
        }
//generate code end
        //public Toggle p1Toggle;
        //public Toggle p2Toggle;
        //public Toggle p3Toggle;
        //public Toggle p4Toggle;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_Lc_packagePanel.listItem = LSharpItemPanel.LoadPrefab(GamePath.asset.ui.panel, typeof(PackageItemPanel).Name);
            __DoInit();

            BG_Lc_packagePanel.selectListener.Add(() =>
            {
                var no = BG_Lc_packagePanel.itemSelected.First;
                if (no != null)
                {
                    Util.UnityHelper.Hide(BG_Rc_emptyRoot);

                    var select = (PackageItem)BG_Lc_packagePanel.values[no.Value];
                    var itemCfg = geniusbaby.tab.item.Instance.Find(select.mId);
                    BG_Rc_icon_name.text = itemCfg.name;
                    BG_Rc_des.text = itemCfg.des;
                    BG_Rc_icon_count.text = itemCfg.des;
                    var costCfg = geniusbaby.tab.item.Inst().Find(itemCfg.sellPrice.id);
                    var canSell = (itemCfg.sellPrice.count > 0);
                    if (canSell)
                    {
                        BG_Rc_sell_costType.sprite = SpritesManager.Inst().Find(costCfg.icon);
                        BG_Rc_sell_costValue.text = costCfg.sellPrice.count.ToString();
                    }
                    Util.UnityHelper.ShowHide(BG_Rc_sell, canSell);

                    Util.UnityHelper.ShowHide(BG_Rc_use, itemCfg.type == ItemType.Chest);
                    //Util.UnityHelper.ShowHide(eggHatchButton, itemCfg.type == ItemType.Egg);
                }
                else
                {
                    Util.UnityHelper.Show(BG_Rc_emptyRoot);
                }
            });
            //p1Toggle.onValueChanged.AddListener(isOn => { if (isOn) SwitchPage(1); });
            //p2Toggle.onValueChanged.AddListener(isOn => { if (isOn) SwitchPage(2); });
            //p3Toggle.onValueChanged.AddListener(isOn => { if (isOn) SwitchPage(3); });
            //p4Toggle.onValueChanged.AddListener(isOn => { if (isOn) SwitchPage(4); });
            BG_Rc_sell.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                var no = BG_Lc_packagePanel.itemSelected.First;
                if (no != null)
                {
                    var frame = GuiManager.Instance.ShowFrame(typeof(ItemSellFrame).Name);
                    var script = T.As<ItemSellFrame>(frame);
                    script.Sell((PackageItem)BG_Lc_packagePanel.values[no.Value]);
                }
            }));
            BG_Rc_use.onClick.AddListener(() =>
            {
                var no = BG_Lc_packagePanel.itemSelected.First;
                if (no != null)
                {
                    Util.UnityHelper.Hide(BG_Rc_emptyRoot);
                    var select = (PackageItem)BG_Lc_packagePanel.values[no.Value];
                    var itemCfg = geniusbaby.tab.item.Instance.Find(select.mId);
                    if (itemCfg.type == ItemType.Chest)
                    {
                        HttpNetwork.Inst().Communicate(new ItemUseRequest() { stackId = select.stackId, count = 1, targetId = -1 });
                    }
                }
            });
            //eggHatchButton.onClick.AddListener(() =>
            //{
            //    //GuiManager.Inst().ShowFrame<PetHatchFrame>();
            //    var no = packagePanel.itemSelected.First;
            //    if (no != null)
            //    {
            //        var egg = packagePanel.values[no.Value];
            //        HttpNetwork.Inst().Communicate(new pps.PetEggHatchRequest() { stackId = egg.stackId });
            //    }
            //});
            BG_Rc_sort.onClick.AddListener(() => HttpNetwork.Inst().Communicate(new ItemSortRequest()));
            //p1Toggle.isOn = true;
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
            PackageModule.Instance.onSync.Add(OnSync);
            //HighLightModule.Instance.onUpdate.Add(OnUpdateHighLight);
            //OnUpdateHighLight(null);
            OnSync();
        }

        public override void OnHide()
        {
            PackageModule.Instance.onSync.Rmv(OnSync);
            //HighLightModule.Instance.onUpdate.Rmv(OnUpdateHighLight);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            //if (p1Toggle.isOn) SwitchPage(1);
            //if (p2Toggle.isOn) SwitchPage(2);
            //if (p3Toggle.isOn) SwitchPage(3);
            //if (p4Toggle.isOn) SwitchPage(4);
            var items = PackageModule.Inst().items.ConvertAll(it => (object)it);
            BG_Lc_packagePanel.SetValues(items);
            BG_Lc_packagePanel.SelectIndex(0);
            Util.UnityHelper.ShowHide(BG_Rc_emptyRoot, items.Count <= 0);
        }
        //void SwitchPage(int pageId)
        //{
        //    var items = PackageModule.Inst().items.FindAll(it => tab.item.Inst().Find(it.mId).page == pageId);                 
        //    packagePanel.SetValues(items);
        //    packagePanel.SelectIndex(0);
        //    Util.UnityHelper.ShowHide(emptyRoot, items.Count <= 0);
        //}
        void OnFireStackItem(int stackId, int count)
        {
            int select = -1;
            var no = BG_Lc_packagePanel.itemSelected.First;
            if (no != null)
            {
                select = no.Value;
            }

            var items = new List<object>(BG_Lc_packagePanel.values);
            var index = items.FindIndex(it => ((PackageItem)it).stackId == stackId);
            if (index >= 0)
            {
                var stackItem = (PackageItem)BG_Lc_packagePanel.values[index];
                stackItem.count -= count;
                if (stackItem.count <= 0)
                {
                    items.RemoveAt(index);
                }
                else
                {
                    BG_Lc_packagePanel.values[index] = stackItem;
                }
                BG_Lc_packagePanel.SetValues(items);
                if (select >= items.Count) { select = 0; }
                BG_Lc_packagePanel.SelectIndex(select);
            }
        }
        //void OnUpdateHighLight(int[] func)
        //{
        //    bool tipState = HighLightModule.Instance.GetFuncTip(Function.Pacakge);
        //    //Util.UnityHelper.ShowHide(fragTipRoot, tipState);
        //}
    }
}
