using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class AlphabetDisplay
    {
        public char ch;
        public bool pazzled;
        public bool focus;
        public Color32 color;
    }
    public class QuestionItemPanel : ILSharpScript
    {
//generate code begin
        public Text ch;
        public Image underline;
        void __LoadComponet(Transform transform)
        {
            ch = transform.Find("@ch").GetComponent<Text>();
            underline = transform.Find("@underline").GetComponent<Image>();
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
        AlphabetDisplay m_value;
        Outline m_outline;
        UITweener tweener;
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
            m_outline = ch.GetComponent<Outline>();
            tweener = ch.GetComponent<UITweener>();
        }
        public AlphabetDisplay GetValue() { return m_value; }
        public void SetValue(AlphabetDisplay value)
        {
            m_value = value;
            underline.enabled = m_value.pazzled;
            if (m_value.pazzled)
            {
                if (m_value.focus)
                {
                    tweener.Play();
                }
                else { tweener.SampleAt(0); }
            }
            else
            {
                tweener.SampleAt(0);
            }
            if (m_value.color.a == 0)
            {
                ch.text = m_value.ch > 0 ? m_value.ch.ToString() : string.Empty;
            }
            else
            {
                ch.text = m_value.ch > 0 ? Util.UnityHelper.EncodeColor(m_value.ch.ToString(), m_value.color) : string.Empty;
            }
        }
        public void SetColor(Color color)
        {
            if (m_outline) { m_outline.effectColor = color; }
        }
        public void Press() { RefreshText(true); }
        public void Release() { RefreshText(false); }
        void RefreshText(bool pressed = false)
        {
            var com = api.GetComponent<LSharpItemPanel>();

            var english = QuizSceneManager.mod.english;
            var exist = english.pazzles.FindIndex(it => it.Key == com.index);
            if (exist >= 0)
            {
                var pazzle = english.pazzles[exist];
                underline.enabled = true;
                bool match = (pazzle.Value == english.original[pazzle.Key]);
                if (pressed)
                {
                    m_outline.enabled = true;
                    tweener.Play();
                    ch.text = Util.UnityHelper.EncodeColor(pazzle.Value.ToString(), Color.white);
                }
                else
                {
                    m_outline.enabled = false;
                    tweener.SampleAt(0);
                    ch.text = Util.UnityHelper.EncodeColor(pazzle.Value.ToString(), match ? Color.green : Color.grey);
                }
                ch.text = pazzle.Value.ToString();
                ch.color = Color.grey;
            }
            else
            {
                m_outline.enabled = false;
                underline.enabled = false;
                tweener.SampleAt(0);
                ch.text = english.original[com.index].ToString();
                ch.color = Color.white;
            }
        }
    }
}
