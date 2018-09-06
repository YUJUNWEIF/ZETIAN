using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GuiScrollVerticalList<TItem, TItemDef, TValue> : IGuiScrollList<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{    
    public override void OnInitialize()
    {
        factoryType = FactoryType.New;
        layoutType = LayoutType.Vert; 
        base.OnInitialize();
    }
    public override void DoDrag(PointerEventData eventData)
    {
        m_position.y += eventData.delta.y;
        m_cachedRc.anchoredPosition = m_position;
        Constraint();
        m_velocity = eventData.delta.y * 2;
    }
    public override void DoImpluse()
    {
        float deltaTime = Time.deltaTime;
        m_velocity *= Mathf.Pow(m_decelerationRate, deltaTime);
        if (Mathf.Abs(m_velocity) < extremum)
        {
            m_velocity = Mathf.Sign(m_velocity) * extremum;
        }
        DoSmooth();
    }
    public override void DoSmooth()
    {
        float deltaTime = Time.deltaTime;
        float distance = m_velocity * deltaTime * A;
        var oldY = m_position.y;
        var newY = m_position.y + distance;

        if ((oldY - m_centerAt * m_deltaSize.y) * (newY - m_centerAt * m_deltaSize.y) <= 0f)
        {
            m_position.y = m_centerAt * m_deltaSize.y;
            m_clipState = ClipState.Normal;
        }
        else
        {
            m_position.y = newY;
        }
        m_cachedRc.anchoredPosition = m_position;
        Constraint();
    }
    public void Constraint()
    {
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
    }
    public override void PredicateFocus()
    {
        var predictDeltaTime = 1f / Application.targetFrameRate;
        var dt = Mathf.Pow(m_decelerationRate, predictDeltaTime);
        var dst = m_position;
        dst.y += m_velocity * A * predictDeltaTime * dt / (1 - dt);
        if (m_velocity < -10f)
        {
            m_centerAt = Mathf.CeilToInt(dst.y / m_deltaSize.y);
        }
        else if (m_velocity > 10f)
        {
            m_centerAt = Mathf.FloorToInt(dst.y / m_deltaSize.y);
        }
        else
        {
            m_centerAt = Mathf.RoundToInt(dst.y / m_deltaSize.y);
        }
        m_centerAt = Mathf.Clamp(m_centerAt, 0, count - 1);
        m_velocity = Mathf.Abs(m_velocity) * Mathf.Sign(m_centerAt * m_deltaSize.y - m_position.y);
    }
    public override void SlidePrev()
    {
        coroutineHelper.StopAll();
        var centerAt = CenterAt();
        if (centerAt > 0)
        {
            m_centerAt = centerAt - 1;
            m_velocity = speed;
            m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Smooth;
        }
    }
    public override void SlideNext()
    {
        coroutineHelper.StopAll();
        var centerAt = CenterAt();
        if (centerAt < count)
        {
            m_centerAt = centerAt + 1;
            m_velocity = -speed;
            m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Smooth;
        }
    }
    public override void SlideTo(int offset)
    {
        coroutineHelper.StopAll();
        m_centerAt = offset;
        var current = CenterAt();
        if (m_centerAt != current)
        {
            m_velocity = Mathf.Sign(m_centerAt - CenterAt()) * speed;
            m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Smooth;
        }
    }
    public override void Goto(int index)
    {
        coroutineHelper.StopAll();
        index = Mathf.Clamp(index, 0, count - 1);
        float y = 0;
        for (int row = 0; row < index; ++row)
        {
            var it = (RectTransform)GetItem(row).transform;
            y += it.rect.height;
        }
        var me = (RectTransform)transform;
        me.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -y, me.rect.height);
    }
    public override int CenterAt()
    {
        var me = (RectTransform)transform;
        int index = Mathf.RoundToInt(me.anchoredPosition.y / m_deltaSize.y);
        return (index >= 0 && index < count) ? index : -1;
    }
}