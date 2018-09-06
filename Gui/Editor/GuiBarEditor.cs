using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(GuiBar))]
    public class GuiBarEditor : Editor
    {
        GuiBar slider;
        public override void OnInspectorGUI()
        {
            slider = target as GuiBar;

            GUILayout.BeginVertical();
            slider.fillRect = EditorGUILayout.ObjectField("fillRect", slider.fillRect, typeof(RectTransform), true) as RectTransform;
            slider.bar = EditorGUILayout.ObjectField("bar", slider.bar, typeof(RectTransform), true) as RectTransform;
            slider.value = EditorGUILayout.FloatField("value", slider.value);
            EditorUtility.SetDirty(slider);
            GUILayout.Space(70f);
            GUILayout.EndVertical();
        }
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
        }
        public virtual void TweenerDisplay(UITweener tweener)
        {

        }
        void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
        }
        void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }
        void EditorUpdate()
        {
            //if (slider && slider.enabled && slider.playing)
            //{
            //    System.Reflection.BindingFlags flag = System.Reflection.BindingFlags.Public |
            //       System.Reflection.BindingFlags.NonPublic |
            //       System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance;
            //    var method = slider.GetType().GetMethod("Advance", flag);
            //    method.Invoke(slider, null);
            //}
        }
    }
}