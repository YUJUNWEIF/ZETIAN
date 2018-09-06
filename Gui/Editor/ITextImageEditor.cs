using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace GuiEditor
{
    public abstract class ITextImageEditor<TTIRender> : Editor
        where TTIRender : MonoBehaviour, ITextImageRender
    {
        protected void DoEdit(TextImage<TTIRender> textImage)
        {
            GUILayout.BeginVertical();
            textImage.color = EditorGUILayout.ColorField("Default Color", textImage.color);
            textImage.spaceX = EditorGUILayout.IntField("SpaceX", textImage.spaceX);
            textImage.spaceY = EditorGUILayout.IntField("SpaceY", textImage.spaceY);
            textImage.lineHeight = EditorGUILayout.IntField("LineHeight", textImage.lineHeight);
            textImage.font = (Font)EditorGUILayout.ObjectField("Font", textImage.font, typeof(Font), false);
            textImage.fontSize = EditorGUILayout.IntField("FontSize", textImage.fontSize);
            textImage.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("FontStyle", textImage.fontStyle);
            textImage.underline = EditorGUILayout.Toggle("Underline", textImage.underline);
            textImage.altas = (Texture)EditorGUILayout.ObjectField("Altas", textImage.altas, typeof(Texture), false);
            textImage.imageHeight = EditorGUILayout.IntField("ImageHeight", textImage.imageHeight);
            //textImage.des = EditorGUILayout.TextArea("Des", textImage.des);
            //GUILayout.BeginHorizontal();
            textImage.width = EditorGUILayout.IntField("Width", textImage.width);
            textImage.align = (TextAlignment)EditorGUILayout.EnumPopup("Width", textImage.align);
            EditorGUILayout.TextField("SingleLineWidth", textImage.singleLineWidth.ToString());
            EditorGUILayout.LabelField("Des");
            textImage.text = EditorGUILayout.TextArea(textImage.text);
            textImage.richText = EditorGUILayout.Toggle("richText", textImage.richText);
            EditorUtility.SetDirty(textImage);
            //GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}