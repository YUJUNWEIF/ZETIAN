using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public interface ITextImage
{
    int width { get; }
    int height { get; }
    void AfterProc();
}
public enum ContentType
{
    Unknown,
    Text,
    HRef,
    Image,
}
public struct Content
{
    public ContentType type;
    public string value;
    public string display;
    public bool defaultColor;
    public Color color;
    public static readonly Regex hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);// 超链接正则
    public static readonly Regex imageRegex = new Regex(@"<e (.*?)/>", RegexOptions.Singleline);// 表情正则

    static void Normal(string txt, List<Content> contents)
    {
        if (!string.IsNullOrEmpty(txt))
        {
            contents.Add(new Content() { type = ContentType.Text, display = txt, defaultColor = true });
        }
    }
    static void HRef(string txt, List<Content> contents)
    {
        if (!string.IsNullOrEmpty(txt))
        {
            var matches = hrefRegex.Matches(txt);
            if (matches.Count > 0)
            {
                int jStart = 0;
                for (int j = 0; j < matches.Count; ++j)
                {
                    var match = matches[j];
                    Normal(txt.Substring(jStart, match.Index - jStart), contents);
                    contents.Add(new Content() { type = ContentType.HRef, value = match.Groups[1].Value, display = match.Groups[2].Value, color = Color.green });
                    jStart = match.Index + match.Length;
                }
                if (jStart < txt.Length) { Normal(txt.Substring(jStart), contents); }
            }
            else
            {
                Normal(txt, contents);
            }
        }
    }
    static void Emotion(string txt, List<Content> contents)
    {
        if (!string.IsNullOrEmpty(txt))
        {
            var matches = imageRegex.Matches(txt);
            if (matches.Count > 0)
            {
                var jStart = 0;
                for (int j = 0; j < matches.Count; ++j)
                {
                    var match = matches[j];
                    HRef(txt.Substring(jStart, match.Index - jStart), contents);
                    contents.Add(new Content() { type = ContentType.Image, display = match.Groups[1].Value });
                    jStart = match.Index + match.Length;
                }
                if (jStart < txt.Length) { HRef(txt.Substring(jStart), contents); }
            }
            else
            {
                HRef(txt, contents);
            }
        }
    }
    public static void Decode(string txt, List<Content> contents)
    {
        Emotion(txt, contents);
    }
}

[ExecuteInEditMode]
public abstract class TextImage<TTIRender> : MonoBehaviour, ITextImage
    where TTIRender : MonoBehaviour, ITextImageRender
{
    public interface IFactory : IDisposable
    {
        Sprite Get(string name);
        TTIRender NewRender(string name, Texture tex);
        void DeleteRender(TTIRender script);
    }
    public class HrefInfo
    {
        public Content content;
        public readonly List<Rect> boxes = new List<Rect>();
    }
    class DefaultFactory : IFactory
    {
        private Queue<TTIRender> m_cachedRenders = new Queue<TTIRender>();
        public Sprite Get(string name) { return null; }
        public TTIRender NewRender(string name, Texture tex)
        {
            TTIRender script = null;
            if (m_cachedRenders.Count > 0)
            {
                script = m_cachedRenders.Dequeue();
                Util.UnityHelper.Show(script);
            }
            else
            {
                var go = new GameObject(name);
                go.hideFlags = HideFlags.DontSave;
                script = go.AddComponent<TTIRender>();
            }
            script.Initialize(name, tex);
            return script;
        }
        public void DeleteRender(TTIRender script)
        {
            script.UnInitialize();
            Util.UnityHelper.Hide(script);
            m_cachedRenders.Enqueue(script);
        }
        public void Dispose()
        {
            while (m_cachedRenders.Count > 0)
            {
                var script = m_cachedRenders.Dequeue();
                if (script)
                {
                    Util.UnityHelper.SafeRelease(script.gameObject);
                }
            }
        }
    }
    //public static readonly Regex hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);// 超链接正则
    //public static readonly Regex imageRegex = new Regex(@"<e (.*?)/>", RegexOptions.Singleline);// 表情正则

    [HideInInspector]
    [SerializeField]
    private Color m_color = Color.white;
    [HideInInspector]
    [SerializeField]
    private int m_spaceX = 0;
    [HideInInspector]
    [SerializeField]
    private int m_spaceY = 2;
    [HideInInspector]
    [SerializeField]
    private int m_lineHeight = 32;
    [HideInInspector]
    [SerializeField]
    private Font m_font;
    [HideInInspector]
    [SerializeField]
    private int m_fontSize = 20;
    [HideInInspector]
    [SerializeField]
    private FontStyle m_fontStyle = FontStyle.Normal;
    [HideInInspector]
    [SerializeField]
    private bool m_underline = false;
    [HideInInspector]
    [SerializeField]
    private Texture m_altas;
    [HideInInspector]
    [SerializeField]
    private int m_imageHeight = 28;
    [HideInInspector]
    [SerializeField]
    private string m_text;
    [HideInInspector]
    [SerializeField]
    private int m_width;
    [HideInInspector]
    [SerializeField]
    private TextAlignment m_align = TextAlignment.Center;
    [HideInInspector]
    [SerializeField]
    private bool m_richText = true;

    IFactory m_factory;
    static DefaultFactory m_defaultFactory = new DefaultFactory();
    static CharacterInfo m_temp;
    protected List<HrefInfo> m_hrefInfos = new List<HrefInfo>();
    private bool m_DisableFontTextureChangedCallback = false;
    private List<Content> m_contents;
    private List<TTIRender> m_renders = new List<TTIRender>();
    public Util.Param1Actions<Content> onHrefClick = new Util.Param1Actions<Content>();
    public Texture GetTex(ITextImageRender script) { return null; }
    public int singleLineWidth { get; private set; }
    RichTextParser parser = new RichTextParser();
    Outline m_outline;
    protected virtual void Awake()
    {
        if (!m_font) { m_font = Resources.GetBuiltinResource<Font>("Arial.ttf"); }
    }
    protected virtual void OnDestroy()
    {
    }
    protected virtual void OnEnable()
    {
        Font.textureRebuilt += TextureRebuilt;
        string temp = m_text;
        m_text = string.Empty;
        text = temp;
        m_outline = GetComponent<Outline>();
    }
    protected virtual void OnDisable()
    {
        Font.textureRebuilt -= TextureRebuilt;
        for (int index = 0; index < m_renders.Count; ++index) { GetFactory().DeleteRender(m_renders[index]); }
        m_renders.Clear();
        GetFactory().Dispose();
    }
    public void SetFactory(IFactory factory) { m_factory = factory; }
    public IFactory GetFactory() { return m_factory != null ? m_factory : m_defaultFactory; }

    void TextureRebuilt(Font font)
    {
        if (font == m_font) { FontTextureChanged(); }
    }
    public void FontTextureChanged()
    {
        if (m_DisableFontTextureChangedCallback) return;

        if (!gameObject.activeInHierarchy) return;
        if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
            SetAllDirty();
        else
            SetAllDirty();
    }
    void NewLine(ref int x, ref int y)
    {
        ProcLine(x);
        x = 0;
        y += (m_lineHeight + m_spaceY);
    }
    void ProcLine(int x)
    {
        float offsetX = 0;
        switch (m_align)
        {
            case TextAlignment.Left: offsetX = 0; break;
            case TextAlignment.Center: offsetX = -x * 0.5f; break;
            case TextAlignment.Right: offsetX = -x; break;
        }
        for (int index = 0; index < lines.Count; ++index) { lines[index].Fill(offsetX); }
        lines.Clear();
    }
    public abstract void AfterProc();
    protected void SetAllDirty()
    {
        m_DisableFontTextureChangedCallback = true;
        singleLineWidth = 0;
        if (m_font && m_contents != null)
        {
            for (int index = 0; index < m_renders.Count; ++index) { GetFactory().DeleteRender(m_renders[index]); }
            m_renders.Clear();

            int x = 0;
            int y = m_lineHeight;
            ProcessContent(ref x, ref y);
            ProcLine(x);

            height = y;
            singleLineWidth = x;
            AfterProc();
            m_renders.ForEach(it => it.AfterProc());
        }
        m_DisableFontTextureChangedCallback = false;
    }
    void ProcessContent(ref int x, ref int y)
    {
        m_hrefInfos.Clear();
        var sv = new Segment.Value()
        {
            bold = (m_fontStyle & FontStyle.Bold) == FontStyle.Bold,
            italic = (m_fontStyle & FontStyle.Italic) == FontStyle.Italic,
            underline = m_underline,
            size = m_fontSize,
            color = m_color,
        };

        for (int index = 0; index < contents.Count; ++index)
        {
            var content = contents[index];
            switch (content.type)
            {
                case ContentType.Text:
                case ContentType.HRef:
                    {
                        TTIRender script = m_renders.Find(it => it.name == m_font.name);
                        if (script == null)
                        {
                            script = GetFactory().NewRender(m_font.name, m_font.material.mainTexture);
                            script.textImage = this;
                            script.Outline((m_outline && m_outline.enabled) ? m_outline : null);
                            m_renders.Add(script);
                        }
                        int start = script.GetVertCount();
                        //sv.color = content.defaultColor ? m_color : content.color;
                        ProcessRegexText(script, content.display, sv, ref x, ref y);

                        if (content.type == ContentType.HRef)
                        {
                            HrefInfo hrefInfo = new HrefInfo();
                            hrefInfo.content = content;
                            for (int vi = start; vi < script.GetVertCount(); vi += 4)
                            {
                                var vertex = script.GetVertPosition(vi + 0);
                                var bounds = new Bounds(vertex, Vector3.zero);

                                vertex = script.GetVertPosition(vi + 1);
                                bounds.Encapsulate(vertex);

                                vertex = script.GetVertPosition(vi + 2);
                                bounds.Encapsulate(vertex);

                                vertex = script.GetVertPosition(vi + 3);
                                bounds.Encapsulate(vertex);

                                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                            }
                            m_hrefInfos.Add(hrefInfo);
                        }
                    }
                    break;
                case ContentType.Image: ProcessEmotion(content.display, m_color, ref x, ref y); break;
            }
        }
    }
    void ProcessRegexText(TTIRender script, string text, Segment.Value sv, ref int x, ref int y)
    {
        if (m_richText)
        {
            var segs = parser.Parser(text, sv);
            for (int index = 0; index < segs.Count; ++index)
            {
                ProcessText(script, segs[index].text, segs[index].value, ref x, ref y);
            }
        }
        else
        {
            ProcessText(script, text, sv, ref x, ref y);
        }
    }
    void ProcessText(TTIRender script, string text, Segment.Value sv, ref int x, ref int y)
    {
        var style = FontStyle.Normal;
        if (sv.bold) { style |= FontStyle.Bold; }
        if (sv.italic) { style |= FontStyle.Italic; }
        m_font.RequestCharactersInTexture(text, sv.size, style);
        bool initUnderline = false;
        for (int at = 0; at < text.Length; ++at)
        {
            var c = text[at];
            if (c == '\n')
            {
                NewLine(ref x, ref y);
                continue;
            }
            if (c < ' ') { continue; }
            bool hasChar = m_font.GetCharacterInfo(c, out m_temp, sv.size, style);
            if (x > 0 && x + m_temp.maxX > width)
            {
                NewLine(ref x, ref y);
            }
            //Fill(script, hasChar, sv, x, y);
            var bl = new Vector3(x + m_temp.minX, -y + m_temp.minY);
            var tr = new Vector3(x + m_temp.maxX, -y + m_temp.maxY);
            Fill(script, hasChar, sv, bl, tr);
            var w = m_spaceX + m_temp.advance;
            //bool hasChar = m_font.GetCharacterInfo(c, out m_temp, m_fontSize, FontStyle.Normal);
            //if (x > 0 && x + m_temp.maxX > width)
            //{
            //    NewLine(ref x, ref y);
            //}
            //Vector3 v0 = hasChar ? new Vector3(x + m_temp.minX, -y + m_temp.minY) : Vector3.zero;
            //Vector3 v1 = hasChar ? new Vector3(x + m_temp.maxX, -y + m_temp.maxY) : Vector3.zero;
            //Vector2 u0 = hasChar ? m_temp.uvBottomLeft : Vector2.zero;
            //Vector2 u1 = hasChar ? m_temp.uvTopRight : Vector2.zero;
            //Fill(script, v0, v1, u0, u1, sv.color, m_temp.flipped);
            if (sv.underline)
            {
                if (!initUnderline)
                {
                    m_font.RequestCharactersInTexture("_", sv.size, style);
                }
                hasChar = m_font.GetCharacterInfo('_', out m_temp, sv.size, style);
                bl.y = -y + m_temp.minY;
                tr.y = -y + m_temp.maxY;
                Fill(script, hasChar, sv, bl, tr);
            }
            x += w;
        }
    }
    void Fill(TTIRender script, bool hasChar, Segment.Value sv, Vector3 bl, Vector3 tr)
    {
        //Vector3 v0 = hasChar ? bl : Vector3.zero;
        //Vector3 v1 = hasChar ? tr : Vector3.zero;
        //Vector2 u0 = hasChar ? m_temp.uvBottomLeft : Vector2.zero;
        //Vector2 u1 = hasChar ? m_temp.uvTopRight : Vector2.zero;
        //Fill(script, v0, v1, u0, u1, sv.color, m_temp.flipped);
        if (hasChar)
        {
            Fill(script, bl, tr, m_temp.uvBottomLeft, m_temp.uvTopRight, sv.color, m_temp.flipped);
        }
        else
        {
            Fill(script, Vector3.zero, Vector3.zero, Vector2.zero, Vector2.zero, sv.color, m_temp.flipped);
        }
    }
    struct Vert
    {
        public TTIRender render;
        public Vector3 pos;
        public Color color;
        public Vector2 uv;
        public Vert(TTIRender render, Vector3 pos, Color color, Vector2 uv)
        {
            this.render = render;
            this.pos = pos;
            this.color = color;
            this.uv = uv;
        }
        public void Fill(float x)
        {
            pos.x += x;
            render.AddVert(pos, color, uv);
        }
    }
    List<Vert> lines = new List<Vert>();
    void Fill(TTIRender render, Vector3 v0, Vector3 v1, Vector2 u0, Vector2 u1, Color clr, bool flipped = false)
    {
        lines.Add(new Vert(render, new Vector3(v1.x, v0.y), clr, flipped ? new Vector2(u0.x, u1.y) : new Vector2(u1.x, u0.y)));
        lines.Add(new Vert(render, new Vector3(v0.x, v0.y), clr, flipped ? new Vector2(u0.x, u0.y) : new Vector2(u0.x, u0.y)));
        lines.Add(new Vert(render, new Vector3(v0.x, v1.y), clr, flipped ? new Vector2(u1.x, u0.y) : new Vector2(u0.x, u1.y)));
        lines.Add(new Vert(render, new Vector3(v1.x, v1.y), clr, flipped ? new Vector2(u1.x, u1.y) : new Vector2(u1.x, u1.y)));
        //renderer.AddVert(new Vector3(v1.x, v0.y), clr, flipped ? new Vector2(u0.x, u1.y) : new Vector2(u1.x, u0.y));
        //renderer.AddVert(new Vector3(v0.x, v0.y), clr, flipped ? new Vector2(u0.x, u0.y) : new Vector2(u0.x, u0.y));
        //renderer.AddVert(new Vector3(v0.x, v1.y), clr, flipped ? new Vector2(u1.x, u0.y) : new Vector2(u0.x, u1.y));
        //renderer.AddVert(new Vector3(v1.x, v1.y), clr, flipped ? new Vector2(u1.x, u1.y) : new Vector2(u1.x, u1.y));        
    }
    void ProcessEmotion(string emotion, Color clr, ref int x, ref int y)
    {
        Sprite sprite = GetFactory().Get(emotion);
        var uv = (sprite != null) ? DataUtility.GetOuterUV(sprite) : Vector4.zero;
        var rc = (sprite != null) ? sprite.rect : new Rect(0, 0, 1, 1);
        Vector2 u0 = new Vector2(uv.x, uv.y);
        Vector2 u1 = new Vector2(uv.z, uv.w);

        int height = m_imageHeight;
        int width = Mathf.RoundToInt(rc.width * m_imageHeight / rc.height);

        if (x > 0 && x + width > this.width)
        {
            NewLine(ref x, ref y);
        }
        Vector3 v0 = new Vector3(x, -y);
        Vector3 v1 = new Vector3(v0.x + width, v0.y + height);

        TTIRender script = m_renders.Find(it => it.name == string.Empty);
        if (script == null)
        {
            script = GetFactory().NewRender(string.Empty, sprite ? sprite.texture : null);
            script.textImage = this;
            script.Outline(null);
            m_renders.Add(script);
        }
        Fill(script, v0, v1, u0, u1, clr);
        x += m_spaceX + width;
    }
    public string text
    {
        get { return m_text; }
        set
        {
            if (m_text != value)
            {
                m_text = value;
                if (contents == null) { contents = new List<Content>(); }
                else { contents.Clear(); }
                Content.Decode(m_text, contents);
                SetAllDirty();
            }
        }
    }
    public List<Content> contents
    {
        get { return m_contents; }
        set { m_contents = value; SetAllDirty(); }
    }
    public Color color
    {
        get { return m_color; }
        set { if (m_color != value) { m_color = value; SetAllDirty(); } }
    }
    public int spaceX
    {
        get { return m_spaceX; }
        set { if (m_spaceX != value) { m_spaceX = value; SetAllDirty(); } }
    }
    public int spaceY
    {
        get { return m_spaceY; }
        set { if (m_spaceY != value) { m_spaceY = value; SetAllDirty(); } }
    }
    public int lineHeight
    {
        get { return m_lineHeight; }
        set { if (m_lineHeight != value) { m_lineHeight = value; SetAllDirty(); } }
    }
    public Font font
    {
        get { return m_font; }
        set { if (m_font != value) { m_font = value; SetAllDirty(); } }
    }
    public int fontSize
    {
        get { return m_fontSize; }
        set { if (m_fontSize != value) { m_fontSize = value; SetAllDirty(); } }
    }
    public FontStyle fontStyle
    {
        get { return m_fontStyle; }
        set { if (m_fontStyle != value) { m_fontStyle = value; SetAllDirty(); } }
    }
    public bool underline
    {
        get { return m_underline; }
        set { if (m_underline != value) { m_underline = value; SetAllDirty(); } }
    }
    public Texture altas
    {
        get { return m_altas; }
        set { if (m_altas != value) { m_altas = value; SetAllDirty(); } }
    }
    public int imageHeight
    {
        get { return m_imageHeight; }
        set { if (m_imageHeight != value) { m_imageHeight = value; SetAllDirty(); } }
    }
    public int width
    {
        get { return m_width; }
        set { if (m_width != value) { m_width = value; SetAllDirty(); } }
    }
    public TextAlignment align
    {
        get { return m_align; }
        set { if (m_align != value) { m_align = value; SetAllDirty(); } }
    }
    public bool richText
    {
        get { return m_richText; }
        set { if (m_richText != value) { m_richText = value; SetAllDirty(); } }
    }
    public int height { get; private set; }
}