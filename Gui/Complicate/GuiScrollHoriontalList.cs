using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GuiScrollHoriontalList<TItem, TItemDef, TValue> : IGuiScrollList<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    public override void OnInitialize()
    {
        layoutType = LayoutType.Hori;
        base.OnInitialize();
    }
    public override void DoDrag(PointerEventData eventData)
    {
        m_position.x += eventData.delta.x;
        m_cachedRc.anchoredPosition = m_position;
        Constraint();
        m_velocity = eventData.delta.x * 2;
    }
    public override void DoImpluse()
    {
        float deltaTime = Time.unscaledDeltaTime;
        m_velocity *= Mathf.Pow(m_decelerationRate, deltaTime);
        if (Mathf.Abs(m_velocity) < extremum)
        {
            m_velocity = Mathf.Sign(m_velocity) * extremum;
        }     
        float distance = m_velocity * deltaTime * A;
        if (Mathf.Abs(m_position.x + m_centerAt * m_deltaSize.x) < Mathf.Abs(distance))
        {
            m_position.x = -m_centerAt * m_deltaSize.x;
            m_clipState = ClipState.Normal;
        }
        else
        {
            m_position.x += distance;
        }

        m_cachedRc.anchoredPosition = m_position;
        Constraint();
    }
    public override void DoSmooth()
    {
        float deltaTime = Time.unscaledDeltaTime;
        float distance = m_velocity * deltaTime * A;
        if (Mathf.Abs(m_position.x + m_centerAt * m_deltaSize.x) < Mathf.Abs(distance))
        {
            m_position.x = -m_centerAt * m_deltaSize.x;
            m_clipState = ClipState.Normal;
        }
        else
        {
            m_position.x += distance;
        }
        m_cachedRc.anchoredPosition = m_position;
        Constraint();
    }
    void Constraint()
    {
        if (m_position.x < -m_cachedRc.sizeDelta.x + clipWindow.sizeDelta.x)
        {
            m_position.x = -m_cachedRc.sizeDelta.x + clipWindow.sizeDelta.x;
            m_cachedRc.anchoredPosition = m_position;
        }
        else if (m_position.x > 0)
        {
            m_position.x = 0;
            m_cachedRc.anchoredPosition = m_position;
        }
    }
    public override void PredicateFocus()
    {
        var dt = Mathf.Pow(m_decelerationRate, 1f / Application.targetFrameRate);
        var dst = m_position;
        dst.x += m_velocity * A * (1f / Application.targetFrameRate) * dt / (1 - dt);
        if (m_velocity < -10)
        {
            m_centerAt = Mathf.CeilToInt(-dst.x / m_deltaSize.x);
        }
        else if (m_velocity > 10)
        {
            m_centerAt = Mathf.FloorToInt(-dst.x / m_deltaSize.x);
        }
        else
        {
            m_centerAt = Mathf.RoundToInt(-dst.x / m_deltaSize.x);
        }
        m_centerAt = Mathf.Clamp(m_centerAt, 0, count - 1);
    }
    public override void SlidePrev()
    {
        coroutineHelper.StopAll();
        var centerAt = CenterAt();
        if (centerAt > 0)
        {
            m_centerAt = centerAt - 1;
            m_velocity = -speed;
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
            m_velocity = speed;
            m_clipState = ClipState.Smooth;
        }
    }
    public override void SlideTo(int offset)
    {
        coroutineHelper.StopAll();
        m_centerAt = offset;
        m_velocity = Mathf.Sign(m_centerAt - CenterAt()) * speed;
        m_clipState = ClipState.Smooth;
    }
    public override void Goto(int offset)
    {
        coroutineHelper.StopAll();
        var position = m_cachedRc.anchoredPosition;
        position.x = m_deltaSize.x * Mathf.Clamp(offset, 0, count - 1);
        m_cachedRc.anchoredPosition = position;
    }
    public override int CenterAt()
    {
        int index = (int)((clipWindow.sizeDelta.x * 0.5f - m_cachedRc.anchoredPosition.x) / m_deltaSize.x);
        return (index >= 0 && index < count) ? index : -1;
    }
}