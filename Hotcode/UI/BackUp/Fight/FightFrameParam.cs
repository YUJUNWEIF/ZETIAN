using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby.ui
{
    public interface IFireable
    {
        void Fire(Vector2 screenPos);
        void Release();
    }
    public class FightFrameParam : MonoBehaviour
    {
        public AnimationCurve coinCurve;
        public AnimationCurve charCurve;
        public float flyTime = 1f;
        public float speed = 80;
        public Color aOutlineColor = new Color32(0x00, 0xFF, 0x00, 0x80);
        public Color bOutlineColor = new Color32(0xFF, 0x00, 0x00, 0x80);
        public Color aFontColor = new Color32(0x00, 0xFF, 0x00, 0xFF);
        public Color bFontColor = new Color32(0xFF, 0x00, 0x00, 0xFF);
    }
}