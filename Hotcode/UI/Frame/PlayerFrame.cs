using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class PlayerFrame : ILSharpScript
    {
//generate code begin
        public Button BG_diamond;
        public Text BG_diamond_value;
        public Text BG_coin_value;
        public Image BG_icon;
        public Text BG_icon_lv;
        public Text BG_icon_name;
        public Image BG_pvp;
        public Text BG_pvp_winRate;
        public Slider BG_music;
        public Slider BG_sound;
        public Text BG_qq;
        public Text BG_qq_value;
        public Text BG_weixin;
        public Text BG_weixin_value;
        public Button BG_system;
        public Button BG_comunication;
        public Button BG_close;
        void __LoadComponet(Transform transform)
        {
            BG_diamond = transform.Find("BG/@diamond").GetComponent<Button>();
            BG_diamond_value = transform.Find("BG/@diamond/@value").GetComponent<Text>();
            BG_coin_value = transform.Find("BG/coin/@value").GetComponent<Text>();
            BG_icon = transform.Find("BG/@icon").GetComponent<Image>();
            BG_icon_lv = transform.Find("BG/@icon/@lv").GetComponent<Text>();
            BG_icon_name = transform.Find("BG/@icon/@name").GetComponent<Text>();
            BG_pvp = transform.Find("BG/@pvp").GetComponent<Image>();
            BG_pvp_winRate = transform.Find("BG/@pvp/@winRate").GetComponent<Text>();
            BG_music = transform.Find("BG/@music").GetComponent<Slider>();
            BG_sound = transform.Find("BG/@sound").GetComponent<Slider>();
            BG_qq = transform.Find("BG/@qq").GetComponent<Text>();
            BG_qq_value = transform.Find("BG/@qq/@value").GetComponent<Text>();
            BG_weixin = transform.Find("BG/@weixin").GetComponent<Text>();
            BG_weixin_value = transform.Find("BG/@weixin/@value").GetComponent<Text>();
            BG_system = transform.Find("BG/@system").GetComponent<Button>();
            BG_comunication = transform.Find("BG/@comunication").GetComponent<Button>();
            BG_close = transform.Find("BG/@close").GetComponent<Button>();
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
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            BG_music.onValueChanged.AddListener(value =>
            {
                var p = new FEFixed(value);
                var setting = Framework.Inst().setting;
                if (p != FEFixed.InnerNew(setting.music))
                {
                    setting.music = (int)p.GetInner();
                    Framework.Inst().UpSet();
                }
            });
            BG_sound.onValueChanged.AddListener(value =>
            {
                var p = new FEFixed(value);
                var setting = Framework.Inst().setting;
                if (p != FEFixed.InnerNew(setting.sound))
                {
                    setting.sound = (int)p.GetInner();
                    Framework.Inst().UpSet();
                }
            });
            BG_close.onClick.AddListener(() => GuiManager.Inst().HideFrame(api));
        }
        public override void OnShow()
        {
            base.OnShow();
            var player = PlayerModule.Inst().player;
            BG_icon_name.text = player.name;
            BG_icon_lv.text = player.lvl.ToString();

            var setting = Framework.Inst().setting;
            BG_music.value = (float)FEFixed.InnerNew(setting.music);
            BG_sound.value = (float)FEFixed.InnerNew(setting.sound);
        }
    }
}
