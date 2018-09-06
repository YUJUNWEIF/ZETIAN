using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class HomePageFrame : ILSharpScript
    {
//generate code begin
        public Button center_pet;
        public Button center_petEntry;
        public Button center_Mode_pve;
        public Button center_Mode_pvp;
        public Button center_Mode_activity;
        public Button center_knowledge;
        public Button Rt_dict;
        public Button Rb_package;
        public Image Rb_package_tip;
        public Button Rb_shop;
        public Image Rb_shop_tip;
        public Button Rb_achieve;
        public Image Rb_achieve_tip;
        public Button chat;
        public Text chat_text_msg;
        public LSharpAPI MsgBroadcastPanel;
        public Button info;
        public Image info_icon;
        public Text info_name;
        public Text info_lv;
        public RectTransform info_bar_exp;
        public Button info_diamond;
        public Text info_diamond_value;
        public Text info_coin_value;
        void __LoadComponet(Transform transform)
        {
            center_pet = transform.Find("center/@pet").GetComponent<Button>();
            center_petEntry = transform.Find("center/@petEntry").GetComponent<Button>();
            center_Mode_pve = transform.Find("center/Mode/@pve").GetComponent<Button>();
            center_Mode_pvp = transform.Find("center/Mode/@pvp").GetComponent<Button>();
            center_Mode_activity = transform.Find("center/Mode/@activity").GetComponent<Button>();
            center_knowledge = transform.Find("center/@knowledge").GetComponent<Button>();
            Rt_dict = transform.Find("Rt/@dict").GetComponent<Button>();
            Rb_package = transform.Find("Rb/@package").GetComponent<Button>();
            Rb_package_tip = transform.Find("Rb/@package/@tip").GetComponent<Image>();
            Rb_shop = transform.Find("Rb/@shop").GetComponent<Button>();
            Rb_shop_tip = transform.Find("Rb/@shop/@tip").GetComponent<Image>();
            Rb_achieve = transform.Find("Rb/@achieve").GetComponent<Button>();
            Rb_achieve_tip = transform.Find("Rb/@achieve/@tip").GetComponent<Image>();
            chat = transform.Find("@chat").GetComponent<Button>();
            chat_text_msg = transform.Find("@chat/text/@msg").GetComponent<Text>();
            MsgBroadcastPanel = transform.Find("@MsgBroadcastPanel").GetComponent<LSharpAPI>();
            info = transform.Find("@info").GetComponent<Button>();
            info_icon = transform.Find("@info/@icon").GetComponent<Image>();
            info_name = transform.Find("@info/@name").GetComponent<Text>();
            info_lv = transform.Find("@info/@lv").GetComponent<Text>();
            info_bar_exp = transform.Find("@info/bar/@exp").GetComponent<RectTransform>();
            info_diamond = transform.Find("@info/@diamond").GetComponent<Button>();
            info_diamond_value = transform.Find("@info/@diamond/@value").GetComponent<Text>();
            info_coin_value = transform.Find("@info/coin/@value").GetComponent<Text>();
        }
        void __DoInit()
        {
            MsgBroadcastPanel.OnInitialize("geniusbaby.LSharpScript.MsgBroadcastPanel");
        }
        void __DoUninit()
        {
            MsgBroadcastPanel.OnUnInitialize();
        }
        void __DoShow()
        {
            MsgBroadcastPanel.OnShow();
        }
        void __DoHide()
        {
            MsgBroadcastPanel.OnHide();
        }
//generate code end
        Image[] m_expsRoot;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            __DoInit();
            m_expsRoot = info_bar_exp.GetComponentsInChildren<Image>(true);
            center_Mode_pve.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(MapClassFrame).Name));
            center_Mode_pvp.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(MapPvpFrame).Name));
            center_Mode_activity.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(ActivityFrame).Name));
            center_pet.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(PetFrame).Name));
            chat.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(CommunicateFrame).Name));
            Rb_achieve.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(AchieveFrame).Name));
            Rb_package.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(PackageFrame).Name));
            Rb_shop.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(ShopFrame).Name));
            center_petEntry.onClick.AddListener(() =>
            {
                int funcId = Function.PetEntry;
                if (FunctionEntry.IsFuncOpen(funcId))
                {
                    //FunctionEntry.Enter(funcId);
                }
                else
                {
                    var frame = GuiManager.Instance.ShowFrame(typeof(MessageBoxFrame).Name);
                    var script = T.As<MessageBoxFrame>(frame);
                    script.SetDesc("func lock", MsgBoxType.Mbt_Ok);
                }
            });           
            center_knowledge.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(WordPackageFrame).Name));
            Rt_dict.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(WordDictFrame).Name));
            info.onClick.AddListener(() => GuiManager.Inst().ShowFrame(typeof(PlayerFrame).Name));
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
            PetModule.Inst().onEquipedSync.Add(OnPetEquipedSync);
            KnowledgeModule.Inst().onKnowledgeSync.Add(OnKnowledgeSync);
            PlayerModule.Instance.onSync.Add(OnSync);
            TitleModule.Instance.onTitleSync.Add(OnSync);
            OnSync();
            OnPetEquipedSync();
            OnKnowledgeSync();
        }
        public override void OnHide()
        {
            PetModule.Inst().onEquipedSync.Rmv(OnPetEquipedSync);
            KnowledgeModule.Inst().onKnowledgeSync.Rmv(OnKnowledgeSync);
            PlayerModule.Instance.onSync.Rmv(OnSync);
            TitleModule.Instance.onTitleSync.Rmv(OnSync);
            __DoHide();
            base.OnHide();
        }
        void OnSync()
        {
            var player = PlayerModule.Instance.player;
            info_name.text = player.name;
            info_lv.text = player.lvl.ToString();
        }
        void OnPetEquipedSync()
        {
            var pet = PetModule.Inst().GetEquiped();
            var heroCfg = tab.hero.Inst().Find(pet.mId);
            var resCfg = tab.objRes.Inst().Find(heroCfg.star.resId);
        }
        void OnKnowledgeSync()
        {
            var lgapId = KnowledgeModule.Inst().lgapId;
            if (lgapId == 0)
            {
                GuiManager.Inst().ShowFrame(typeof(WordPackageFrame).Name);
            }
            else
            {
                LGAPManager.Inst().ParseAfterDownload(LGAP.Decode(lgapId));
            }
        }
    }
}
