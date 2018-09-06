using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class WordWrongItemPanel : ILSharpScript
    {
//generate code begin
        public Text english;
        public Text Image_statistic;
        void __LoadComponet(Transform transform)
        {
            english = transform.Find("@english").GetComponent<Text>();
            Image_statistic = transform.Find("Image/@statistic").GetComponent<Text>();
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
        geniusbaby.archive.Knowledge.Wrong m_wrong;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            api.GetComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                var frame = GuiManager.Inst().ShowFrame(typeof(WordDetailFrame).Name);
                var script = T.As<WordDetailFrame>(frame);
                script.Display(0, m_wrong.english, m_wrong.chinese);
            }));
        }
        public geniusbaby.archive.Knowledge.Wrong GetValue() { return m_wrong; }
        public void SetValue(geniusbaby.archive.Knowledge.Wrong value)
        {
            m_wrong = value;
            english.text = m_wrong.english;
            Image_statistic.text = m_wrong.wrongs.ToString();
        }
    }
}
