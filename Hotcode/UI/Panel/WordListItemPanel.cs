using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordListItemPanel : ILSharpScript
    {
//generate code begin
        public Text text;
        void __LoadComponet(Transform transform)
        {
            text = transform.Find("@text").GetComponent<Text>();
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
            api.GetComponent<Button>().onClick.AddListener(() =>
            {
                var com = api.GetComponent<LSharpItemPanel>();
                com.ListComponent.SelectIndex(com.index);
            });
        }
        string m_word;
        public string GetValue() { return m_word; }
        public void SetValue(string value)
        {
            m_word = value;
            text.text = m_word;
            //wordText.text = m_word;
        }
    }
}
