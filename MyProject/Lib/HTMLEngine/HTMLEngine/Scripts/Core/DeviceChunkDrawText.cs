/// The modified version of this software is Copyright (C) 2013 ZHing.
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

using System.Diagnostics;

namespace HTMLEngine.Core
{
    internal class DeviceChunkDrawText : DeviceChunk
    {
        public DrawTextDeco Deco = DrawTextDeco.None;
        public bool DecoStop;
        public HtColor Color;
        public string Text;
        public string Id = null;
        public bool PrevIsWord = false;

        public override void Draw(HtDevice dev, float deltaTime, string linkText, object userData)
        {
            if (0 != (this.Deco & DrawTextDeco.Underline))
            {
                dev.FillRect(new HtRect(Rect.X, Rect.Bottom - 2, DecoStop ? Rect.Width : this.TotalWidth, 1), this.Color, userData);
            }
            if (0 != (this.Deco & DrawTextDeco.Strike))
            {
                dev.FillRect(new HtRect(Rect.X, Rect.Bottom - Rect.Height / 2 - 1, DecoStop ? Rect.Width : this.TotalWidth, 1), this.Color, userData);
            }
            this.Font.Draw(this.Id, this.Rect, this.Color, this.Text, false, Core.DrawTextEffect.None, HtColor.white, 0, linkText, userData);
        }

        public override void MeasureSize()
        {
            Debug.Assert(this.Font != null, " font is not assigned");
            Debug.Assert(string.IsNullOrEmpty(this.Text) == false, "text is not assigned");

            HtSize size = this.Font.Measure(this.Text);
            Rect.Width = size.Width;
            Rect.Height = Font.LineSpacing; //size.Height;
        }

        public override string ToString()
        {
            return Text ?? "(null)";
        }
    }
}