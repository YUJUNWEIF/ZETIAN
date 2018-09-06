using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(TweenPosition))]
    public class TweenPositionEditor : UITweenEditor
    {
        public override void TweenerDisplay(UITweener tweener)
        {
            base.TweenerDisplay(tweener);
            TweenPosition tween = target as TweenPosition;
            var from = EditorGUILayout.Vector3Field("from", tween.from);
            var to = EditorGUILayout.Vector3Field("to", tween.to);
            if (from != tween.from) { tween.from = from; }
            if (to != tween.to) { tween.to = to; }
        }
    }
}