using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HTMLEngine.UGUI
{
    public interface IRefAndGetter
    {
        void OnLinkRefClick(string linkRef);
        Sprite GetSprite(string sprite);
        Font GetFont(string face);
    }
    public class UGUIDevice : HtDevice
    {
        private readonly Dictionary<string, UGUIFont> fonts = new Dictionary<string, UGUIFont>();
        private readonly Dictionary<string, UGUIImage> images = new Dictionary<string, UGUIImage>();
        private static Texture2D whiteTex;
        IRefAndGetter m_refGetter;
        public UGUIDevice(IRefAndGetter refGetter)
        {
            m_refGetter = refGetter;
        }
        public override HtFont LoadFont(string face, int size, bool bold, bool italic)
        {
            string key = string.Format("{0}{1}{2}{3}", face, size, bold ? "b" : "", italic ? "i" : "");
            UGUIFont ret;
            if (!fonts.TryGetValue(key, out ret)) { fonts.Add(key, ret = new UGUIFont()); }
            ret.Initialize(m_refGetter, face, size, bold, italic);
            return ret;
        }
        public override HtImage LoadImage(string src, int fps)
        {
            UGUIImage ret;
            if (!images.TryGetValue(src, out ret)) { images.Add(src, ret = new UGUIImage()); }
            ret.Initialize(m_refGetter, src, fps);
            return ret;
        }
        
        public override void FillRect(HtRect rect, HtColor color, object userData)
        {
            var root = userData as Transform;
            if (root != null)
            {
                var go = new GameObject("fill", typeof(Image));
                go.layer = root.gameObject.layer;
                go.transform.parent = root;
                var spr = go.GetComponent<Image>();
                spr.rectTransform.anchoredPosition = new Vector3(rect.X + rect.Width / 2, -rect.Y - rect.Height / 2 - 2, -1f);
                spr.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.Width);
                spr.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.Height);
                spr.rectTransform.localScale = Vector3.one;
                spr.sprite  = null;
                spr.color = new Color32(color.R, color.G, color.B, color.A);
            }
            else
            {
                HtEngine.Log(HtLogLevel.Error, "Can't draw without root.");
            }
        }
        public override void OnRelease()
        {
            foreach (var pair in fonts)
            {
                pair.Value.OnRelease();
            }
        }
    }
}
