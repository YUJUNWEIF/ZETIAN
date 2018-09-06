using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public abstract class IGuiScrollList<TItem, TItemDef, TValue> : GuiListBasic<TItem, TItemDef, TValue>, IListLocalizable, IBeginDragHandler, IEndDragHandler, IDragHandler
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
    protected ClipState m_clipState = ClipState.Normal;
    protected float m_velocity;
    protected float m_decelerationRate = 0.14f;
    protected int m_centerAt;
    protected float A = 10f;
    protected Vector2 m_position;
    protected RectTransform m_cachedRc;
    protected Vector2 m_deltaSize;
    protected int m_lastFocus;

    public RectTransform clipWindow;
    public float speed = 36f;
    public float extremum = 30f;
    public Util.Param1Actions<int> onFocus = new Util.Param1Actions<int>();
    public Util.Param1Actions<int> onLoseFocus = new Util.Param1Actions<int>();
    public override void OnInitialize()
    {
        base.OnInitialize();
        if (clipWindow == null) { clipWindow = (RectTransform)transform.parent; }
        m_cachedRc = (RectTransform)transform;
        if (listItem) { m_deltaSize = ((RectTransform)listItem.transform).sizeDelta; }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.activeSelf)
        {
            coroutineHelper.StopAll();
            m_position = m_cachedRc.anchoredPosition;
            m_clipState = ClipState.Dragging;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (gameObject.activeSelf)
        {
            DoDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (gameObject.activeSelf)
        {
            m_clipState = ClipState.Impluse;
            PredicateFocus();
        }
    }
    void Update()
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
            if (focus != m_lastFocus)
            {
                if (m_lastFocus >= 0) { onLoseFocus.Fire(m_lastFocus); }
                m_lastFocus = focus;
                if (m_lastFocus >= 0) { onFocus.Fire(m_lastFocus); }
            }
            transformChanged.Fire();
        }
    }
    public override void SetValues(IList<TValue> values)
    {
        base.SetValues(values);
        m_lastFocus = -1;
    }
    public abstract void SlidePrev();
    public abstract void SlideNext();
    public abstract void SlideTo(int offset);
    public abstract void Goto(int index);

    public abstract void DoDrag(PointerEventData eventData);
    public abstract void DoImpluse();
    public abstract void DoSmooth();
    public abstract int CenterAt();
    public abstract void PredicateFocus();
}