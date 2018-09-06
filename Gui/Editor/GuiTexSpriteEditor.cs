using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(GuiTexSprite))]
    public class GuiTexSpriteEditor : Editor
    {
        GuiTexSprite m_tex;
        //public virtual void OnSceneGUI()
        //{
        //    m_textEffect = target as GuiTextEffect;
        //}
        public override void OnInspectorGUI()
        {
            m_tex = target as GuiTexSprite;
            //PrefabType type = PrefabUtility.GetPrefabType(widget);
            GUILayout.BeginVertical();
            var sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", m_tex.sprite, typeof(Sprite), false);
            var color = EditorGUILayout.ColorField("Color", m_tex.color);
            var size = EditorGUILayout.Vector2Field("Size", m_tex.size);
            m_tex.sprite = sprite;
            m_tex.size = size;
            m_tex.color = color;
            if(GUILayout.Button("SetNativeSize"))
            {
                if(m_tex.sprite && m_tex.sprite.texture)
                {
                    m_tex.SetNativeSize();
                }
            }
            GUILayout.EndVertical();
        }
        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
        }
    }
}