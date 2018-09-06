using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(UITweener))]
public class UITweenEditor : Editor
{
    UITweener tweener;
    public override void OnInspectorGUI()
    {
        tweener = target as UITweener;

        GUILayout.BeginVertical();
        tweener.method = (UITweener.Method)EditorGUILayout.EnumPopup("method", tweener.method);
        tweener.style = (UITweener.Style)EditorGUILayout.EnumPopup("style", tweener.style);
        tweener.ignoreTimeScale = EditorGUILayout.Toggle("ignoreTimeScale", tweener.ignoreTimeScale);
        tweener.useCurve = EditorGUILayout.Toggle("useCurve", tweener.useCurve);
        if (tweener.useCurve)
        {
            var curve = EditorGUILayout.CurveField("curve", tweener.animationCurve);
            tweener.animationCurve = curve;
        }
        tweener.playing = EditorGUILayout.Toggle("play", tweener.playing);
        tweener.duration = EditorGUILayout.FloatField("duration", tweener.duration);
        tweener.delay = EditorGUILayout.FloatField("delay", tweener.delay);

        TweenerDisplay(tweener);
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
        if (tweener && tweener.enabled && tweener.playing)
        {
            System.Reflection.BindingFlags flag = System.Reflection.BindingFlags.Public |
               System.Reflection.BindingFlags.NonPublic |
               System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance;
            var method = tweener.GetType().GetMethod("Advance", flag);
            method.Invoke(tweener, null);
        }
    }
}