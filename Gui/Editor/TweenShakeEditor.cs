using UnityEditor;
using UnityEngine;
using System.Collections;
namespace GuiEditor
{
    [CustomEditor(typeof(TweenShake))]
    public class TweenShakeEditor : UITweenEditor
    {
        public override void TweenerDisplay(UITweener tweener)
        {
            base.TweenerDisplay(tweener);
            TweenShake tween = target as TweenShake;
            var distance = EditorGUILayout.FloatField("distance", tween.distance);
            var intensity = EditorGUILayout.FloatField("intensity", tween.intensity);
            if (distance != tween.distance) { tween.distance = distance; }
            if (intensity != tween.intensity) { tween.intensity = intensity; }
        }
    }
}