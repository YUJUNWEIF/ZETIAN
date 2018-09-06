using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class FuncTipFrame : ILSharpScript
    {
//generate code begin
        public Text des;
        public Text name;
        public Text Image_func;
        void __LoadComponet(Transform transform)
        {
            des = transform.Find("@des").GetComponent<Text>();
            name = transform.Find("@name").GetComponent<Text>();
            Image_func = transform.Find("Image/@func").GetComponent<Text>();
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
        int m_guideId;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                GuiManager.Inst().HideFrame(api.name);
                if (m_guideId > 0)
                {
                    GuideModule.Inst().ScenarioAccept(m_guideId);
                    GuideMaskFrame.RestoreGuide(GuideModule.Inst().guide);
                }
            });
        }
        public void Display(int subFunc, object param)
        {
            m_guideId = 0;
            switch (subFunc)
            {
                case SubFunction.TitleLvUp:
                    {
                        //var title = TitleModule.Inst().Find();
                        //var titleCfg = tab.title.Inst().Find(title.id);
                        //nameText.text = titleCfg.name + titleCfg.subName[title.lv];
                        //desText.text = "title lvl up";
                        //funcText.text = "from cfg";
                    }
                    break;
                case SubFunction.TitleOpen:
                    {
                        //var title = TitleModule.Inst().Find();
                        //var titleCfg = tab.title.Inst().Find(title.id);
                        //nameText.text = titleCfg.name + titleCfg.subName[title.lv];
                        //desText.text = "title open";
                        //funcText.text = "from cfg";
                    }
                    break;
                case SubFunction.GunOpen:
                    {
                        name.text = "unlock";
                        des.text = "congulations, unlock gun";
                        Image_func.text = "from cfg";
                    }
                    break;
                case SubFunction.GunLvUp:
                    {
                        name.text = "gun";
                        des.text = "congulations, gun lv up";
                        Image_func.text = "from cfg";
                    }
                    break;
                case SubFunction.AchieveSatified:
                    {
                        name.text = "achieve";
                        des.text = "congulations, achieve satisfied";
                        Image_func.text = "from cfg";
                    }
                    break;
            }
        }
    }
}
