using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace GuiEditor
{
    [CustomEditor(typeof(GuiSpriteLine), true)]
    [CanEditMultipleObjects]
    public class LineEditor : GraphicEditor
    {
        SerializedProperty m_dots;
        SerializedProperty m_sprite;
        SerializedProperty m_spriteType;
        SerializedProperty m_uvOffset;
        SerializedProperty m_lineWidthPop;

        GUIContent m_spriteContent;
        GUIContent m_spriteTypeContent;
        GUIContent m_uvOffsetContent;
        GUIContent m_dotsContent;
        GUIContent m_lineWidthContent;
        private CustomArrayDrawer<Transform> customArrayDrawer;
        private GuiSpriteLine m_line;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_line = target as GuiSpriteLine;
            m_spriteContent = new GUIContent("Sprite");
            m_spriteTypeContent = new GUIContent("SpriteType");
            m_uvOffsetContent = new GUIContent("UVOffset");
            m_dotsContent = new GUIContent("Dots");
            m_lineWidthContent = new GUIContent("LineWidth");
            m_sprite = serializedObject.FindProperty("m_sprite");
            m_spriteType = serializedObject.FindProperty("m_spriteType");
            m_uvOffset = serializedObject.FindProperty("m_uvOffset");
            m_dots = serializedObject.FindProperty("m_dots");
            m_lineWidthPop = serializedObject.FindProperty("m_lineWidth");
            customArrayDrawer = new CustomArrayDrawer<Transform>(this, OnInspectorChanged, m_line, m_line.trans, "Spline Nodes");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SpriteGUI();
            //UVOffsetGUI();
            AppearanceControlsGUI();
            DotsGUI();
            LineWidthGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_sprite, m_spriteContent);
            EditorGUILayout.PropertyField(m_spriteType, m_spriteTypeContent);
            if (EditorGUI.EndChangeCheck())
            {
                GuiSpriteLine line = target as GuiSpriteLine;
                line.sprite = m_sprite.objectReferenceValue as Sprite;
                line.type = (GuiSpriteLine.Type)m_spriteType.intValue;
            }
        }
        //protected void UVOffsetGUI()
        //{
        //    EditorGUI.BeginChangeCheck();
        //    EditorGUILayout.Vector2Field(m_uvOffset, m_uvOffsetContent);
        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        Line line = target as Line;
        //        line.uvOffset = m_uvOffset.vector2Value;
        //    }
        //}
        protected void DotsGUI()
        {
            if (m_line.trans != null) { customArrayDrawer.DrawArray(); }
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
            m_line.trans = m_line.trans;
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
