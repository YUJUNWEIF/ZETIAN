using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace GuiEditor
{
    [CustomEditor(typeof(TextImage2D), true)]
    [CanEditMultipleObjects]
    public class TextImage2DEditor : ITextImageEditor<TextImage2DRender>
    {
        private TextImage2D m_textImage;
        public override void OnInspectorGUI()
        {
            DoEdit(target as TextImage2D);
        }
        public override bool HasPreviewGUI() { return true; }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
        }
        public void OnInspectorChanged()
        {
        }
    }
}
