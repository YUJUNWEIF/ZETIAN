﻿/// The modified version of this software is Copyright (C) 2013 ZHing.
/// The original version's copyright as below.

/* Copyright (C) 2012 Ruslan A. Abdrashitov

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE. */

using System;
using UnityEngine;

namespace HTMLEngine.UGUI
{
    public class UGUIDemo : MonoBehaviour, IRefAndGetter
    {
        /// <summary>
        /// Is html text changed?
        /// </summary>
        private bool changed = true;

        /// <summary>
        /// Current html
        /// </summary>
        private string _html = "";

        private const string demo0 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<p id='introduction' align=center>This is based on Profixy's HTMLEngine-Mini.<br>URL:&nbsp;<u>http://html-engine-mini.googlecode.com/</u><br></p>
<br><p><i>Here is no any &lt;html&gt;, &lt;body&gt; etc tags. Only small subset of tags supported yet:</i></p>
<br>
<p><code>- &lt;<font color=yellow>p</font>&nbsp;[id='...'] [align=(left | right | center | justify)]<br>[valign=(top | middle | bottom)]&gt;</code></p>
<p><code>- &lt;<font color=yellow>spin</font>&nbsp;[id='...'] [width=...]&gt;</code></p>
<p><code>- &lt;<font color=yellow>img</font>&nbsp;src='...' [id='...'] [fps=...] [width=...] [height=...]&gt;</code></p>
<p><code>- &lt;<font color=yellow>font</font>&nbsp;[face=...] [size=...] [color=...]&gt;</code></p>
<p><code>- &lt;<font color=yellow>effect</font>&nbsp;name=... [amount=...] [color=...]&gt;</code></p>
<p><code>- &lt;<font color=yellow>br</font>&gt; &lt;<font color=yellow>b</font>&gt; &lt;<font color=yellow>i</font>&gt; &lt;<font color=yellow>u</font>&gt; &lt;<font color=yellow>s</font>&gt; &lt;<font color=yellow>code</font>&gt;</code></p>
<p><code>- &lt;<font color=yellow>a</font>&nbsp;href='...'&gt;</code></p>
<br>
<p><i>Also this demo contains some internal resources (fonts and images) than can be used.</i></p>
<br>
<p><b>Available fonts:&nbsp;</b>'default16', 'default16b', 'default16bi', 'default16i', 'title24'</p>
<p><b>Available atleses:&nbsp;</b>'smiles', 'logos', 'faces'</p>
";

        private const string demo1 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<p align=left>Without effect:</p>
<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>
<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>
<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>
<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>
<p align=left>Shadow effect:</p>
<effect name=shadow color=black>
<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>
<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>
<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>
<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>
</effect>
<p align=left>Outline effect:</p>
<effect name=outline color=black>
<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>
<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>
<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>
<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>
</effect>
";

        private const string demo2 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<font size=24>
<br><spin id='outlined' align=center><effect name=outline color=#FFFFFF80><font color=black>Outlined text</font></effect></spin>
<p align=center><effect name=outline color=yellow><font color=black>Outlined yellow text</font></effect></p>
<p align=center><effect name=outline color=#FFFFFF80 amount=2>Some stuppid effect i got</effect></p>
<p align=center><effect name=shadow>Default shadowed text</effect></p>
<p align=center><effect name=shadow color=black>Strong-shadowed text</effect></p>
<p align=center><effect name=shadow color=#FFFFFF80 amount=2>Some shadowed text</effect></p>
</font>
    ";

        private const string demo3 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<br><p align=justify>Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text.</p>
<br><p align=center><font color=gray>Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text.</font></p>
<br><p align=right>Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text.</p>
<br><p align=left><font color=gray>Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text.</font></p>
        ";

        private const string demo4 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<br><p align=center valign=top>Picture <img src='smiles/sad'> with &lt;p valign=top&gt;</p>
<br><p align=center valign=middle>Picture <img src='smiles/smile'> with &lt;p valign=middle&gt; much better than others in this case <img src='smiles/cool'></p>
<br><p align=center valign=bottom>Picture <img src='smiles/sad'> with &lt;p valign=bottom&gt;</p>
<br><p align=justify valign=bottom><img src='logos/unity'> is a feature rich, fully integrated development engine for the creation of interactive 3D content. It provides complete, out-of-the-box functionality to assemble high-quality, high-performing content and publish to multiple platforms.</p>
<br><p align=center><img src='logos/unity2'></p>
        ";

        private const string demo5 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<br><p align=center>Now we try to make something dynamic inside text markup...</p>
<br><p align=center valign=middle>Due to performance we can not parse text every frame <img src='smiles/sad'>, but we can reserve some place to draw things inside compiled html! <img src='smiles/cool'></p>
<br><p align=center>It's possible with img tag. Look at source.</p>
<br><p align=center><img src='#time'></p>
<br><p align=center>With same technique we can render animated pictures and even results from render targets.</p>
";

        private const string demo6 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br>
<br><p align=center>Links support</p>
<br><p align=left valign=middle>1)&nbsp;<a href='plaintextlink' id='simple'>Simple plain text link.</a></p>
<br><p align=left valign=middle>2)&nbsp;<a href='textandimage'>Simple text and <img src='smiles/smile'> image link.</a></p>
<br><p align=left valign=middle>3)&nbsp;<a href='biglink1'>Multiline link <img src='smiles/smile'>.</a>&nbsp;<a href='biglink2'>Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>.</a></p>
<br><br><p align=center>Try to click around and see to left-bottom corner for results</p>
<br><br><p align=center>At last we have some basic stuff to interact with player</p>
";

        private const string demo7 =
            @"<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>
<br><br><p align=center>Some stuff using Unity3DGUI.Label here</p>
<br><br><p align=center>Below is totally dynamic html text, and statistics for compile and drawing</p>
";
        
        
        /// <summary>
        /// our html compiler
        /// </summary>
        private HtCompiler compiler;

        /// <summary>
        /// html view x, y
        /// </summary>
        private const int x = 5;
        private const int y = 50;

        /// <summary>
        /// html view width, height (calculated by compiler)
        /// </summary>
        private readonly int width = Screen.width / 2 - x;
        private int height;

        /// <summary>
        /// setting text here will raise _changed flag
        /// </summary>
        public string html
        {
            get { return this._html; }
            set
            {
                this._html = value;
                this.changed = true;
            }
        }
        public Action<string> linkRefClick { set; private get; }
        public Func<string, Sprite> spriteGetter { set; private get; }
        public Func<string, Font> fontGetter { set; private get; }
        public void OnLinkRefClick(string linkText) { if (linkRefClick != null) linkRefClick(linkText); }
        public Sprite GetSprite(string sprite) { return spriteGetter != null ? spriteGetter(sprite) : null; }
        public Font GetFont(string face) { return fontGetter != null ? fontGetter(face) : null; }

        HtDevice dev;
        public void Awake()
        {
            Debug.Log("Initializing Demo");
            if (HtEngine.Logger == null)
            {
                HtEngine.RegisterLogger(new HTMLEngine.Unity3D.Unity3DLogger());
            }
            dev = new UGUIDevice(this);

            // our compiler
            this.compiler = HtEngine.GetCompiler();
            // set up first html
            //this.html = demo0;

        }

        public void Update()
        {
            if (this.changed)
            {
                // we have new html text, so compile it
                this.compiler.Compile(dev, this._html, this.width);
                foreach (Transform childTr in transform)
                    Destroy(childTr.gameObject);
                this.compiler.Draw(dev, Time.deltaTime, (RectTransform)transform);
                // store height of compiled html
                this.height = this.compiler.CompiledHeight;
                // release changed flag
                this.changed = false;
            }
        }
        
        public void OnDestroy()
        {
            // we need to dispose compiler to prevent GC
            if (compiler != null)
            {
                compiler.Dispose();
                compiler = null;
            }
            if (dev != null)
            {
                dev.OnRelease();
                dev = null;
            }
        }
    }
}