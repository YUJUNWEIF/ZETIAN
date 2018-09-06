using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GuiScrollListSuper<TItem, TItemDef, TValue> : GuiBasicListSuper<TItem, TItemDef, TValue>, IListLocalizable, IBeginDragHandler, IEndDragHandler, IDragHandler
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    protected enum ClipState
    {
        Normal = 0,
        Dragging = 1,
        Impluse = 2,
        Smooth = 3,
    }
    public RectTransform maskWindow;
    public float speed = 360f;
    public float extremum = 30f;

    public Util.Param1Actions<int> onFocus = new Util.Param1Actions<int>();
    public Util.Param1Actions<int> onLoseFocus = new Util.Param1Actions<int>();

    protected ClipState m_clipState = ClipState.Normal;
    protected float m_velocity;
    protected float m_decelerationRate = 0.14f;
    protected int m_centerAt;
    protected float A = 10f;
    //protected Vector2 m_position;
    protected int m_lastFocus;
    public bool shieldDrag { get; set; }
    float m_offset;
    protected override float offset { get { return m_offset; } }
    public override void OnInitialize()
    {
        var delta = (maskWindow.sizeDelta - clipWindow.sizeDelta) * 0.5f;
        switch (layout)
        {
            case LayoutType.Vert: m_offset = delta.y; break;
            case LayoutType.Hori: m_offset = delta.x; break;
        }
        base.OnInitialize();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.activeSelf && !shieldDrag)
        {
            coroutineHelper.StopAll();
            //m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Dragging;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (gameObject.activeSelf && !shieldDrag)
        {
            DoDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (gameObject.activeSelf && !shieldDrag)
        {
            m_clipState = ClipState.Impluse;
            PredicateFocus();
        }
    }
    void FixedUpdate()
    {
        switch (m_clipState)
        {
            case ClipState.Normal: break;
            case ClipState.Dragging: break;
            case ClipState.Impluse: DoImpluse(); break;
            case ClipState.Smooth: DoSmooth(); break;
        }
        if (transform.hasChanged)
        {
            transform.hasChanged = false;

            var focus = CenterAt();
            bool forceUpdate = true;//fix unity bug
            if (forceUpdate || focus != m_lastFocus)
            {
                if (m_lastFocus >= 0) { onLoseFocus.Fire(m_lastFocus); }
            }

            logic.UpdateDisplayItem();
            transformChanged.Fire();

            if (forceUpdate || focus != m_lastFocus)
            {
                m_lastFocus = focus;
                if (m_lastFocus >= 0) { onFocus.Fire(m_lastFocus); }
            }
        }
    }
    public override void SetValues(IList<TValue> values)
    {
        base.SetValues(values);
        m_lastFocus = -1;
    }
    public void DoDrag(PointerEventData eventData)
    {
        Vector2 m_position = m_cachedRc.anchoredPosition;
        switch (layout)
        {
            case LayoutType.Vert:
                m_position.y += eventData.delta.y;
                m_velocity = eventData.delta.y * 2;
                break;
            case LayoutType.Hori:
                m_position.x += eventData.delta.x;
                m_velocity = eventData.delta.x * 2;
                break;
        }
        m_cachedRc.anchoredPosition = m_position;
        Constraint();
    }
    public void DoImpluse()
    {
        m_velocity *= Mathf.Pow(m_decelerationRate, Time.deltaTime);
        if (Mathf.Abs(m_velocity) < extremum)
        {
            m_velocity = Mathf.Sign(m_velocity) * extremum;
        }
        DoSmooth();
    }
    public void DoSmooth()
    {
        Vector2 m_position = m_cachedRc.anchoredPosition;

        float distance = m_velocity * Time.deltaTime * A;
        switch (layout)
        {
            case LayoutType.Vert:
                if (Mathf.Abs(m_position.y - m_centerAt * logic.cHeight) < Mathf.Abs(distance))
                {
                    m_position.y = m_centerAt * logic.cHeight;
                    m_clipState = ClipState.Normal;
                }
                else { m_position.y += distance; }
                break;
            case LayoutType.Hori:
                if (Mathf.Abs(m_position.x + m_centerAt * logic.cWidth) < Mathf.Abs(distance))
                {
                    m_position.x = -m_centerAt * logic.cWidth;
                    m_clipState = ClipState.Normal;
                }
                else { m_position.x += distance; }
                break;
        }
        m_cachedRc.anchoredPosition = m_position;
        Constraint();
    }
    public void PredicateFocus()
    {
        Vector2 position = m_cachedRc.anchoredPosition;
        var predictDeltaTime = 1f / Application.targetFrameRate;
        var dt = Mathf.Pow(m_decelerationRate, predictDeltaTime);
        switch (layout)
        {
            case LayoutType.Vert:
                float dstY = position.y + m_velocity * A * predictDeltaTime * dt / (1 - dt);
                m_centerAt = Mathf.RoundToInt(dstY / logic.cHeight);
                m_velocity = Mathf.Abs(m_velocity) * Mathf.Sign(m_centerAt * logic.cHeight - position.y);
                break;
            case LayoutType.Hori:
                float dstX = position.x + m_velocity * A * predictDeltaTime * dt / (1 - dt);
                m_centerAt = Mathf.RoundToInt(-dstX / logic.cWidth);
                m_velocity = Mathf.Abs(m_velocity) * Mathf.Sign(-m_centerAt * logic.cWidth - position.x);
                break;
        }
        m_centerAt = Mathf.Clamp(m_centerAt, 0, GetCount() - 1);
    }
    void Constraint()
    {
        Vector2 m_position = m_cachedRc.anchoredPosition;
        switch (layout)
        {
            case LayoutType.Vert:
                if (m_position.y > m_cachedRc.rect.height - clipWindow.rect.height)
                {
                    m_position.y = m_cachedRc.rect.height - clipWindow.rect.height;
                    m_cachedRc.anchoredPosition = m_position;
                }
                else if (m_position.y < 0)
                {
                    m_position.y = 0;
                    m_cachedRc.anchoredPosition = m_position;
                }
                break;
            case LayoutType.Hori:
                if (m_position.x < -m_cachedRc.rect.width + clipWindow.rect.width)
                {
                    m_position.x = -m_cachedRc.rect.width + clipWindow.rect.width;
                    m_cachedRc.anchoredPosition = m_position;
                }
                else if (m_position.x > 0)
                {
                    m_position.x = 0;
                    m_cachedRc.anchoredPosition = m_position;
                }
                break;
        }
    }
    int GetCount()
    {
        int count = 0;
        switch (layout)
        {
            case LayoutType.Vert: count = displayRow; break;
            case LayoutType.Hori: count = displayColumn; break;
        }
        return count;
    }
    public int CenterAt()
    {
        int index = 0;
        switch (layout)
        {
            case LayoutType.Vert:
                index = Mathf.FloorToInt((clipWindow.rect.height * 0.5f + m_cachedRc.anchoredPosition.y) / logic.cHeight);
                break;
            case LayoutType.Hori:
                index = Mathf.FloorToInt((clipWindow.rect.width * 0.5f - m_cachedRc.anchoredPosition.x) / logic.cWidth);
                break;
        }
        return (index >= 0 && index < GetCount()) ? index : -1;
    }
    public void SlidePrev()
    {
        float dirSpeed = 0f;
        switch (layout)
        {
            case LayoutType.Vert: dirSpeed = 1f; break;
            case LayoutType.Hori: dirSpeed = -1f; break;
        }
        coroutineHelper.StopAll();
        var centerAt = CenterAt();
        if (centerAt > 0)
        {
            m_centerAt = centerAt - 1;
            m_velocity = speed * dirSpeed;
            //m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Smooth;
        }
    }
    public void SlideNext()
    {
        float dirSpeed = 0f;
        switch (layout)
        {
            case LayoutType.Vert: dirSpeed = -1f; break;
            case LayoutType.Hori: dirSpeed = 1f; break;
        }
        coroutineHelper.StopAll();
        var centerAt = CenterAt();
        if (centerAt < values.Count)
        {
            m_centerAt = centerAt + 1;
            m_velocity = speed * dirSpeed;
            //m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Smooth;
        }
    }
    public void SlideTo(int offset)
    {
        coroutineHelper.StopAll();
        m_centerAt = Mathf.Clamp(offset, 0, GetCount() - 1); ;
        var current = CenterAt();
        if (m_centerAt != current)
        {
            m_velocity = Mathf.Sign(m_centerAt - CenterAt()) * speed;
            //m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Smooth;
        }
    }
    public void Goto(int index)
    {
        coroutineHelper.StopAll();
        logic.Goto(index);
    }
}