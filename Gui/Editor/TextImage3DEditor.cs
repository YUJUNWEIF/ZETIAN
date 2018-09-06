using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace GuiEditor
{
    [CustomEditor(typeof(TextImage3D), true)]
    [CanEditMultipleObjects]
    public class TextImage3DEditor : ITextImageEditor<TextImage3DRender>
    {
        private TextImage3D m_textImage;
        public override void OnInspectorGUI()
        {
            if (targets != null)
            {
                for (int index = 0; index < targets.Length; ++index)
                {
                    DoEdit(targets[index] as TextImage3D);
                }
            }
            else
            {
                DoEdit(target as TextImage3D);
            }
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