using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(TweenRotation))]
    public class TweenRotationEditor : UITweenEditor
    {
        public override void TweenerDisplay(UITweener tweener)
        {
            base.TweenerDisplay(tweener);
            var tween = target as TweenRotation;
            var from = EditorGUILayout.Vector3Field("from", tween.from);
            var to = EditorGUILayout.Vector3Field("to", tween.to);
            if (from != tween.from) { tween.from = from; }
            if (to != tween.to) { tween.to = to; }
        }
    }
}