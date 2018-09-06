using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace GuiEditor
{
    [CustomEditor(typeof(GuiTexLine), true)]
    [CanEditMultipleObjects]
    public class GuiTexLineEditor : GraphicEditor
    {
        SerializedProperty m_dots;
        SerializedProperty m_tex;
        SerializedProperty m_uvRect;
        SerializedProperty m_lineWidthPop;

        GUIContent m_texContent;
        GUIContent m_uvRectContent;
        GUIContent m_dotsContent;
        GUIContent m_lineWidthContent;
        private CustomArrayDrawer<Transform> customArrayDrawer;
        private GuiTexLine m_line;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_line = target as GuiTexLine;
            m_texContent = new GUIContent("Tex");
            m_uvRectContent = new GUIContent("UVRect");
            m_dotsContent = new GUIContent("Dots");
            m_lineWidthContent = new GUIContent("LineWidth");
            m_tex = serializedObject.FindProperty("m_tex");
            m_uvRect = serializedObject.FindProperty("m_uvRect");
            m_dots = serializedObject.FindProperty("m_dots");
            m_lineWidthPop = serializedObject.FindProperty("m_lineWidth");
            customArrayDrawer = new CustomArrayDrawer<Transform>(this, OnInspectorChanged, m_line, m_line.dots, "Spline Nodes");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SpriteGUI();

            EditorGUI.BeginChangeCheck();
            var rect = EditorGUILayout.RectField(m_uvRectContent, m_line.rect);
            //if (rect != m_line.rect)
            //{
            //    m_line.rect = rect;
            //}
            if (EditorGUI.EndChangeCheck())
            {
                //m_line.rect = rect;
                m_uvRect.rectValue = rect;
            }

            //var rect = EditorGUILayout.RectField(m_uvRectContent, m_line.rect);
            //if (rect != m_line.rect)
            //{
            //    m_line.rect = rect;
            //}

            AppearanceControlsGUI();
            DotsGUI();
            LineWidthGUI();
            serializedObject.ApplyModifiedProperties();
        }
        protected void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_tex, m_texContent);
            if (EditorGUI.EndChangeCheck())
            {
                m_line.tex = m_tex.objectReferenceValue as Texture2D;
            }
            
        }
        protected void DotsGUI()
        {
            if (m_line.dots != null) { customArrayDrawer.DrawArray(); }
        }
        protected void LineWidthGUI()
        {
            // m_line.lineWidth != EditorGUILayout.IntField(m_lineWidthContent, m_line.lineWidth);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_lineWidthPop, m_lineWidthContent);
            if (EditorGUI.EndChangeCheck())
            {
                m_line.lineWidth = m_lineWidthPop.intValue;
            }
        }
        public override bool HasPreviewGUI() { return true; }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
        }
        public void OnInspectorChanged()
        {
            m_line.dots = m_line.dots;
            //SceneView.RepaintAll();
        }

        //public void ApplyChangesToTarget(Object targetObject)
        //{
        //    Spline spline = targetObject as Spline;

        //    spline.UpdateSpline();

        //    SplineMesh splineMesh = spline.GetComponent<SplineMesh>();

        //    if (splineMesh != null)
        //        splineMesh.UpdateMesh();
        //}
    }
}
