using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GuiScrollSpanList<TItem, TItemDef, TValue> : IGuiScrollList<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    CircleClockwiseLayout m_circle;
    public float deltaAngle = 60f;
    public float angleStartAt = 0f;
    public override void OnInitialize()
    {
        factoryType = FactoryType.Pregen;
        layoutType = LayoutType.CircleClockwise;
        base.OnInitialize();
        m_circle = (CircleClockwiseLayout)m_layout;
        //m_circle.rotStartAt = Mathf.Deg2Rad * angleStartAt;
        m_circle.offset = 0f;
        m_circle.deltaRadian = Mathf.Deg2Rad * deltaAngle;
    }
    public override void DoDrag(PointerEventData eventData)
    {
       // if (RectTransformUtility.RectangleContainsScreenPoint(m_cachedRc, eventData.position, eventData.enterEventCamera))
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_cachedRc, eventData.position, eventData.pressEventCamera, out localMousePos);
            {
                Vector3 offset = (m_cachedRc.rect.center - localMousePos) / m_circle.radius;
                float rot = Vector3.Dot(Vector3.Cross(eventData.delta, offset), Vector3.forward) / m_circle.radius;
                m_circle.offset += rot;
                FireListItemChangeNotify();
                Constraint();
                m_velocity = rot * 2;
            }
        }
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
        float offset = m_centerAt * m_circle.deltaRadian;
        if (Mathf.Abs(m_circle.offset - offset) < Mathf.Abs(distance))
        {
            m_circle.offset = offset;
            m_clipState = ClipState.Normal;
        }
        else
        {
            m_circle.offset += distance;
        }
        FireListItemChangeNotify();
        Constraint();
    }
    public void Constraint()
    {
        if (m_circle.offset < 0f || m_circle.offset > m_circle.deltaRadian * listItems.Count )// || m_circle.offset > 2 * Mathf.PI)
        {
            m_circle.offset = Mathf.Clamp(m_circle.offset, 0, m_circle.deltaRadian * listItems.Count);
            FireListItemChangeNotify();
        }
    }
    public override void PredicateFocus()
    {
        var predictDeltaTime = 1f / Application.targetFrameRate;
        var dt = Mathf.Pow(m_decelerationRate, predictDeltaTime);
        var dst = m_circle.offset;
        dst += m_velocity * A * predictDeltaTime * dt / (1 - dt);
        m_centerAt = CenterAt(dst);
        m_centerAt = Mathf.Clamp(m_centerAt, 0, count - 1);
        dst = m_centerAt * m_circle.deltaRadian;
        m_velocity = Mathf.Abs(m_velocity) * Mathf.Sign(dst - m_circle.offset);
    }
    public override void SlidePrev()
    {
        coroutineHelper.StopAll();
        var centerAt = CenterAt();
        if (centerAt > 0)
        {
            m_centerAt = centerAt - 1;
            m_velocity = speed;
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
    public override void Goto(int index)
    {
        coroutineHelper.StopAll();
        m_circle.offset = m_circle.deltaRadian * Mathf.Clamp(index, 0, count - 1);
        FireListItemChangeNotify();
    }
    public override int CenterAt()
    {
        return CenterAt(m_circle.offset);
    }
    int CenterAt(float offset)
    {
        var radian = Mathf.Clamp(offset, 0, m_circle.deltaRadian * listItems.Count);
        return Mathf.RoundToInt(radian / m_circle.deltaRadian);
    }
}