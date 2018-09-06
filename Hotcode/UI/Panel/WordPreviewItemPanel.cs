using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordPreviewItemPanel : ILSharpScript
    {
//generate code begin
        public Text english;
        public Text chinese;
        void __LoadComponet(Transform transform)
        {
            english = transform.Find("@english").GetComponent<Text>();
            chinese = transform.Find("@chinese").GetComponent<Text>();
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
        geniusbaby.cfg.word m_word;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                var frame = GuiManager.Inst().ShowFrame(typeof(WordDetailFrame).Name);
                var script = T.As<WordDetailFrame>(frame);
                script.Display(0, m_word.english, m_word.chinese);
            }));
        }
        public geniusbaby.cfg.word GetValue() { return m_word; }
        public void SetValue(geniusbaby.cfg.word value)
        {
            m_word = value;
            english.text = m_word.english;
            chinese.text = m_word.chinese;
        }
    }
}
