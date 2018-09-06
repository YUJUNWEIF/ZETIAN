using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace HTMLEngine.UGUI
{

    public interface ILinkRefClick
    {
        void OnClick(string txt);
    }

    public class UGUIFont : HtFont
    {
        private int whiteSize;
        private FontStyle m_fontStyle;
        private Font m_font;
        private  const int m_lineSpacing = 6;
        private GameObject go;
        static CharacterInfo temp;
        IRefAndGetter m_refGetter;
        public void Initialize(IRefAndGetter refGetter, string face, int size, bool bold, bool italic)           
        {
            base.Initialize(face, size, bold, italic);
            m_refGetter = refGetter;
            m_fontStyle = FontStyle.Normal;
            if (bold) { m_fontStyle |= FontStyle.Bold; }
            if (italic) { m_fontStyle |= FontStyle.Italic; }
            m_font = m_refGetter.GetFont(face);
            if (!m_font)
            {
                m_font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            if (m_font.GetCharacterInfo(' ', out temp, size, m_fontStyle))
            {
                whiteSize = Mathf.RoundToInt(temp.advance);
            }
            else
            {
                whiteSize = size;
            }
        }
        public override int LineSpacing { get { return Size + m_lineSpacing; } }
        public override int WhiteSize { get { return this.whiteSize; } }
        public float pixelsPerUnit { get { return 1; } }

        public TextGenerationSettings GetGenerationSettings(Vector2 extents)
        {
            var settings = new TextGenerationSettings();
            settings.generationExtents = extents;
            if (m_font != null && m_font.dynamic)
            {
                settings.fontSize = Size;
                settings.resizeTextMinSize = Size;
                settings.resizeTextMaxSize = Size;
            }
            // Other settings
            settings.textAnchor = TextAnchor.MiddleCenter;
            settings.scaleFactor = pixelsPerUnit;
            settings.color = Color.white;
            settings.font = m_font;
            settings.pivot = Vector2.one * 0.5f;
            settings.richText = true;
            settings.lineSpacing = LineSpacing;
            settings.fontStyle = m_fontStyle;
            settings.resizeTextForBestFit = false;
            settings.updateBounds = false;
            settings.horizontalOverflow = HorizontalWrapMode.Overflow;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            return settings;
        }
        public override HtSize Measure(string text)
        {
            var textGenerator = new TextGenerator();
            var width = textGenerator.GetPreferredWidth(text, GetGenerationSettings(Vector2.zero)) / pixelsPerUnit;
            var height = textGenerator.GetPreferredHeight(text, GetGenerationSettings(new Vector2(width, 0f))) / pixelsPerUnit;
            return new HtSize((int)width, (int)height);
        }
        enum TxtType
        {
            None = 0,
            Word = 1,
            Speech = 2,
            Content = 3,
        }
        const string wordFlag = "`1`";
        const string speechFlag = "`2`";
        const string contentFlag = "`3`";
        string Decode(TxtType txtType, string text, bool isChinese)
        {
            switch (txtType)
            {
                case TxtType.Word: return Util.UnityHelper.EncodeColor(text, Color.red);
                case TxtType.Speech: return Util.UnityHelper.EncodeColor('[' + (isChinese ? text : PhoneticMap(text)) + ']', Color.blue);
                case TxtType.Content: return text;
            }
            return string.Empty;
        }
        public override void Draw(string id, HtRect rect, HtColor color, string text, bool isEffect, Core.DrawTextEffect effect, HtColor effectColor, int effectAmount, string linkText, object userData)
        {
            if (isEffect) return;
            var root = userData as Transform;
            if (root != null)
            {
                go = new GameObject(string.IsNullOrEmpty(id) ? "label" : id, typeof(Text));
                go.layer = root.gameObject.layer;

                var lab = go.GetComponent<Text>();
                var rc = lab.rectTransform;
                rc.SetParent(root);
                go.transform.localScale = new Vector3(1f, 1f, 1f);
                rc.pivot = new Vector2(0.5f, 1f);
                rc.anchorMax = new Vector2(0f, 1f);
                rc.anchorMin = new Vector2(0f, 1f);
                rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, rect.X, rect.Width);
                rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, rect.Y, rect.Height);

                //判断中文还是英文单词
                if (text.StartsWith(wordFlag))
                {
                    text = text.Substring(wordFlag.Length);
                    var isChinese = IsContainChineseLetter(text);
                    lab.text = Decode(TxtType.Word, text, isChinese);
                    if (!isChinese)
                    {
                        linkText = text;
                    }
                }
                else if (text.StartsWith(speechFlag))
                {
                    text = text.Substring(speechFlag.Length);
                    if (text.StartsWith(contentFlag))//特殊处理有些英文没有音标的情况
                    {
                        text = text.Substring(contentFlag.Length);
                        lab.text = Decode(TxtType.Content, text, false);                        
                    }
                    else
                    {
                        lab.text = Decode(TxtType.Speech, text, false);
                    }
                }
                else if(text.StartsWith(contentFlag))
                {
                    text = text.Substring(contentFlag.Length);
                    lab.text = Decode(TxtType.Content, text, false);
                }
                else
                {
                    lab.text = text;
                }
                //lab.text = Decode(text,isChinese);
                lab.font = m_font;
                lab.fontSize = Size;
                lab.fontStyle = m_fontStyle;
                lab.lineSpacing = m_lineSpacing;
                lab.supportRichText = true;
                lab.alignment = TextAnchor.MiddleLeft;
                lab.color = new Color32(color.R, color.G, color.B, color.A);
                lab.horizontalOverflow = HorizontalWrapMode.Overflow;
                
                switch (effect)
                {
                    case Core.DrawTextEffect.None:
                        break;
                    case Core.DrawTextEffect.Outline:
                        {
                            var script = go.AddComponent<Outline>();
                            script.effectColor = new Color32(effectColor.R, effectColor.G, effectColor.B, effectColor.A);
                            script.effectDistance = new Vector2(effectAmount, effectAmount);
                            script.useGraphicAlpha = true;
                        }
                        break;
                    case Core.DrawTextEffect.Shadow:
                        {
                            var script = go.AddComponent<Shadow>();
                            script.effectColor = new Color32(effectColor.R, effectColor.G, effectColor.B, effectColor.A);
                            script.effectDistance = new Vector2(effectAmount, effectAmount);
                            script.useGraphicAlpha = true;
                        }
                        break;
                }

                // build link.
                if (!string.IsNullOrEmpty(linkText))
                {
                    var button = go.AddComponent<Button>();
                    button.transition = Selectable.Transition.None;
                    button.onClick.AddListener(() =>
                    {
                        m_refGetter.OnLinkRefClick(linkText);
                    });
                }
            }
            else
            {
                HtEngine.Log(HtLogLevel.Error, "Can't draw without root.");
            }
        }

        /// <summary>
        /// on the font be released.
        /// </summary>
        public void OnRelease()
        {
            if (go)
            {
                Util.UnityHelper.SafeRelease(go);
                go = null;
            }
        }

        public string PhoneticMap(string src)
        {
            char[] chars = src.ToCharArray();
            for (int index = 0;index<chars.Length;++index)
            {
                var ch = chars[index];
                switch (ch)
                {
                    case '5': chars[index] = 'ˈ'; break;
                    case '7': chars[index] = 'ˌ'; break;
                    case '9': chars[index] = 'ˌ'; break;
                    case 'A': chars[index] = 'æ'; break;
                    case 'B': chars[index] = 'ɑ'; break;
                    case 'C': chars[index] = 'ɔ'; break;
                    case 'E': chars[index] = 'ə'; break;
                    case 'F': chars[index] = 'ʃ'; break;
                    case 'I': chars[index] = 'ɪ'; break;
                    case 'J': chars[index] = 'ʊ'; break;
                    case 'N': chars[index] = 'ŋ'; break;
                    case 'Q': chars[index] = 'ʌ'; break;
                    case 'R': chars[index] = 'ɔ'; break;
                    case 'T': chars[index] = 'ð'; break;
                    case 'U': chars[index] = 'u'; break;
                    case 'V': chars[index] = 'ʒ'; break;
                    case 'W': chars[index] = 'θ'; break;
                    case '\\': chars[index] = 'ɜ'; break;
                    case '^': chars[index] = 'ɡ'; break;
                    default : break;
                }
            }
            string mapedStr = new string(chars);
            return mapedStr;
        }

        public bool IsContainChineseLetter(string input)
        {
            if (input == "")
                return false;
            int code = 0;
            int chfrom = System.Convert.ToInt32("4e00", 16); //范围（0x4e00～0x9fff）
            int chend = System.Convert.ToInt32("9fff", 16);
            for (int index = 0; index < input.Length; ++index)
            {
                code = System.Char.ConvertToUtf32(input, index); 
                if (code >= chfrom && code <= chend)
                {
                    return true; 
                }
            }
            return false;
        }

    }
}