using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(TweenUV))]
    public class TweenUVEditor : UITweenEditor
    {
        public override void TweenerDisplay(UITweener tweener)
        {
            base.TweenerDisplay(tweener);
            TweenUV tween = target as TweenUV;
            tween.render = (Renderer)EditorGUILayout.ObjectField("render", tween.render, typeof(Renderer), true);
            tween.texName = EditorGUILayout.TextField("texName", tween.texName);

            var from = EditorGUILayout.Vector2Field("from", tween.from);
            var to = EditorGUILayout.Vector2Field("to", tween.to);
            if (from != tween.from) { tween.from = from; }
            if (to != tween.to) { tween.to = to; }
        }
    }
}