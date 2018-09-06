using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HTMLEngine.UGUI
{
    /// <summary>
    /// Provides image for use with HTMLEngine. Implements abstract class.
    /// </summary>
    
    public interface ISpriteGetter
    {
        Sprite Get(string name);
    }
    public class UGUIImage : HtImage
    {
        IRefAndGetter m_refGetter;
        private bool isTime;
        private HtFont timeFont;
        public string spriteName;
        public bool isAnim;
        public int FPS;
        public void Initialize(IRefAndGetter refGetter, string source, int fps)
        {
            this.m_refGetter = refGetter;
            if ("#time".Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                isTime = true;
                //timeFont = HtEngine.Device.LoadFont("code", HtEngine.DefaultFontSize, false, false);
            }
            else
            {
                string atlasName = source.Substring(0, source.LastIndexOf('/'));
                spriteName = source.Substring(source.LastIndexOf('/') + 1);
                isAnim = fps >= 0;
                FPS = fps;
                //uiAtlas = Resources.Load("atlases/" + atlasName, typeof(UIAtlas)) as UIAtlas;
                //if (uiAtlas == null)
                //{
                //    Debug.LogError("Could not load html image atlas from " + "atlases/" + atlasName);
                //}
            }
        }

        /// <summary>
        /// Returns width of image
        /// </summary>
        public override int Width
        {
            get
            {
                if (isTime) return 120;
                var sprite = m_refGetter.GetSprite(spriteName);
                return sprite != null ? (int)sprite.rect.width : 0;
            }
        }

        /// <summary>
        /// Returns height of image
        /// </summary>
        public override int Height
        {
            get
            {
                if (isTime) return 20;

                var sprite = m_refGetter.GetSprite(spriteName);
                return sprite != null ? (int)sprite.rect.height : 0;
            }
        }

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="rect">Where to draw</param>
        /// <param name="color">Color to use (ignored for now)</param>
        /// <param name="linkText">Link text</param>
        /// <param name="userData">User data</param>
        public override void Draw(string id, HtRect rect, HtColor color, string linkText, object userData)
        {
            if (isTime)
            {
                var now = DateTime.Now;
                timeFont.Draw(
                            "time",
                  rect,
                  color,
                  string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", now.Hour, now.Minute, now.Second, now.Millisecond),
                            false,
                            Core.DrawTextEffect.None,
                            HtColor.white,
                            0,
                            linkText,
                  userData);
            }
            else
            {
                var root = userData as Transform;
                if (root != null)
                {
                    var go = new GameObject(string.IsNullOrEmpty(id) ? "image" : id, typeof(Image));
                    go.layer = root.gameObject.layer;
                    go.transform.parent = root;
                    var spr = go.GetComponent<Image>();
                    spr.rectTransform.anchoredPosition = new Vector3(rect.X + rect.Width / 2, -rect.Y - rect.Height / 2, 0f);
                    spr.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.Width);
                    spr.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.Height);
                    spr.color = new Color32(color.R, color.G, color.B, color.A);
                    spr.type = Image.Type.Simple;

                    if (isAnim)
                    {
                        //var sprAnim = go.AddComponent<UISpriteAnimation>();
                        //sprAnim.framesPerSecond = FPS;
                        //sprAnim.namePrefix = spriteName;
                    }
                    else
                    {
                        spr.sprite = m_refGetter.GetSprite(spriteName);
                    }

                    if (!string.IsNullOrEmpty(linkText))
                    {
                        var button = go.AddComponent<Button>();
                        button.transition = Selectable.Transition.None;
                        button.onClick.AddListener(() =>
                        {
                            Debug.Log(linkText);
                        });
                    }
                }
                else
                {
                    HtEngine.Log(HtLogLevel.Error, "Can't draw without root.");
                }
            }
        }
    }

}