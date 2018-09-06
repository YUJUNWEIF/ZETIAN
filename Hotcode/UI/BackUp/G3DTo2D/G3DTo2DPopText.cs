using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby.ui
{
    public class G3DTo2DPopText : MonoBehaviour
    {
        public Text text;
        static Font m_bfont1;
        static Font m_bfont2;
        static Font m_bfont3;
        void Awake()
        {
            if (!m_bfont1) { m_bfont1 = BundleManager.Instance.LoadSync<Font>(GamePath.asset.font, "BFont1"); }
            if (!m_bfont2) { m_bfont2 = BundleManager.Instance.LoadSync<Font>(GamePath.asset.font, "BFont2"); }
            if (!m_bfont3) { m_bfont3 = BundleManager.Instance.LoadSync<Font>(GamePath.asset.font, "BFont3"); }
        }
        //public IEnumerator PopText(RectTransform rc, TextArea step)
        //{
        //    text.text = step.text;
        //    Color color = Color.white;
        //    switch (step.textType)
        //    {
        //        case TextArea.TextType.Gain: text.font = m_bfont2; break;
        //        case TextArea.TextType.BlueDamage: text.font = m_bfont3; break;
        //        case TextArea.TextType.RedDamage: text.font = m_bfont1; break;
        //    }
        //    const float duration = 2f;
        //    float timePassed = 0f;
        //    while (timePassed < duration)
        //    {
        //        color.a = Mathf.Sqrt(1 - timePassed / duration);
        //        timePassed += Time.deltaTime;
        //        yield return null;
        //    }
        //}
    }
}