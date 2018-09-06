using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ActivityTypeItemPanel : ILSharpScript
    {
//generate code begin
        public Image disactive;
        public Text name;
        public Image tip;
        void __LoadComponet(Transform transform)
        {
            disactive = transform.Find("@disactive").GetComponent<Image>();
            name = transform.Find("@name").GetComponent<Text>();
            tip = transform.Find("@tip").GetComponent<Image>();
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
        int m_type;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
        }
        public int GetValue() { return m_type; }
        public void SetValue(int value)
        {
            m_type = value;
            name.text = tab.messageTip.Inst().Find(m_type).des;
            OnUpdateHighLight(null);
        }
        void OnUpdateHighLight(int[] modules)
        {
            //bool hasAward = HighLightModule.Instance.GetFuncTip(Function.NewPlayer, m_type);
            //Util.UnityHelper.ShowHide(tipRoot, hasAward);
        }
    }
}
