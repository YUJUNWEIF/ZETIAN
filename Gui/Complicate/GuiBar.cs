using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class GuiBar : UIBehaviour
{
    RectTransform m_cachedRc;
    [SerializeField]
    private RectTransform m_fillRect;
    [SerializeField]
    private RectTransform m_barRect;
    public RectTransform fillRect
    {
        get { return m_fillRect; }
        set { m_fillRect = value; UpdateVisuals(); }
    }
    public RectTransform bar
    {
        get { return m_barRect; }
        set { m_barRect = value; UpdateVisuals(); }
    }
    [SerializeField]
    private float m_Value;
    public float value
    {
        get { return m_Value; }
        set { Set(value); }
    }
    protected GuiBar()
    { }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
        if (IsActive())
        {
            Set(m_Value, false);
            // Update rects since other things might affect them even if value didn't change.
            UpdateVisuals();
        }
    }

#endif // if UNITY_EDITOR

    protected override void OnEnable()
    {
        base.OnEnable();
        m_cachedRc = (RectTransform)transform;
        Set(m_Value, false);
        // Update rects since they need to be initialized correctly.
        UpdateVisuals();
    }


    // Set the valueUpdate the visible Image.
    void Set(float input)
    {
        Set(input, true);
    }

    protected virtual void Set(float input, bool sendCallback)
    {
        // Clamp the input
        float newValue = Mathf.Clamp01(input);

        // If the stepped value doesn't match the last one, it's time to update
        if (m_Value == newValue)
            return;

        m_Value = newValue;
        UpdateVisuals();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
        if (!IsActive())
            return;

        UpdateVisuals();
    }

    // Force-update the slider. Useful if you've changed the properties and want it to update visually.
    private void UpdateVisuals()
    {
        if (!IsActive())
            return;
        var width = (m_cachedRc.rect.width * m_Value);
        if (fillRect)
        {
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            var ap = fillRect.rect;
            if (float.IsNaN(ap.x) || float.IsNaN(ap.y) || float.IsNaN(ap.width) || float.IsNaN(ap.height))
            {
                fillRect.anchoredPosition = Vector2.zero;
                fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
            }
        }
        if (bar)
        {
            var sizeDelta = m_barRect.sizeDelta;
            m_barRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, width - sizeDelta.x * 0.5f, sizeDelta.x);
        }
    }
}