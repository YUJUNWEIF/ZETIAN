using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(TweenTransform))]
    public class TweenTransformEditor : UITweenEditor
    {
        public override void TweenerDisplay(UITweener tweener)
        {
            base.TweenerDisplay(tweener);
            TweenTransform tween = tweener as TweenTransform;
            var from = EditorGUILayout.ObjectField("from", tween.from, typeof(Transform), true) as Transform;
            var to = EditorGUILayout.ObjectField("to", tween.to, typeof(Transform), true) as Transform;
            if (from != tween.from) { tween.from = from; }
            if (to != tween.to) { tween.to = to; }
        }
    }
}