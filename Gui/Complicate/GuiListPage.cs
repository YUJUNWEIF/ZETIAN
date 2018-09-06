using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class Page
{
    RangeValue m_page;
    public RangeValue page { get { return m_page; } }
    public void SetPageCount(int count)
    {
        m_page.max = count;
        if (m_page.current >= count - 1)
        {
            m_page.current = count - 1;
        }
        if (m_page.current < 0)
        {
            m_page.current = 0;
        }
    }
    public bool PreviousPage()
    {
        if (m_page.current > 0)
        {
            --m_page.current;
            return true;
        }
        return false;
    }
    public bool NextPage()
    {
        if (m_page.current < m_page.max - 1)
        {
            ++m_page.current;
            return true;
        }
        return false;
    }
    public bool GotoPage(int pageIndex)
    {
        if (m_page.current != pageIndex && pageIndex >= 0 && pageIndex < m_page.max)
        {
            m_page.current = pageIndex;
            return true;
        }
        return false;
    }
}

public class AutoPageSize<T> : Page
{
    private IList<T> m_values;
    public readonly Util.Param1Actions<object> autoPageEvent = new Util.Param1Actions<object>();
    public int PageSize { get; private set; }
    //public Comparison<T> SortFunc;

    public AutoPageSize(int pageSize)
    {
        PageSize = pageSize;
    }
    public IList<T> Values
    {
        get { return m_values; }
        set
        {
            m_values = value;
            //if (SortFunc != null) { Values.Sort(SortFunc); }
            SetPageCount(PageSize > 0 ? (Values.Count + PageSize - 1) / PageSize : 1);
        }
    }
    public IList<T> CurrentPage()
    {
        if (PageSize > 0)
        {
            int startIndex = page.current * PageSize;
            int count = PageSize;
            if (startIndex + count > Values.Count)
            {
                count = Values.Count - startIndex;
            }
            var result = new List<T>();
            for (int index = 0; index < count; ++index) { result.Add(Values[startIndex + index]); }
            return result;
        }
        return Values;
    }
    public void FireAutoPageEvent()
    {
        autoPageEvent.Fire(this);
    }
}

public enum ResetType
{
    ImmedMove,
    WholeMove,
    OrderMove,
    CrossMove,
};
public enum GuiMovingState
{
    Normal = 0,
    Dragging = 1,
    Impluse = 2,
    Smooth = 3,
}

public class GuiListPage<TItem, TItemDef, TValue> : GuiListBasic<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    public AutoPageSize<TValue> autoPage { get; private set; }
    public int PageSize = 10;
    public ResetType rt = ResetType.CrossMove;
    public float tweenDuration = 0.5f;
    public Util.Param2Actions<object, RangeValue> pageListener = new Util.Param2Actions<object, RangeValue>();
    public override void OnInitialize()
    {
        factoryType = FactoryType.New;
        layoutType = LayoutType.Grid;
        base.OnInitialize();
        autoPage = new AutoPageSize<TValue>(PageSize > 0 ? PageSize : -1);
        autoPage.autoPageEvent.Add(sender =>
            {
                IList<TValue> values = autoPage.CurrentPage();
                if (values != null)
                {
                    base.SetValues(values);
                    int startIndex = autoPage.page.current * autoPage.PageSize;
                    for (int index = 0; index < values.Count; ++index)
                    {
                        var it = base.GetItem(index);
                        it.index = startIndex + index;
                    }
                }
                else
                {
                    base.Clear();
                }
                pageListener.Fire(this, new RangeValue(autoPage.page.current, autoPage.page.max));
            });
    }
    public override void SetValues(IList<TValue> values)
    {
        ClearAllSelect();
        autoPage.Values = values;
        autoPage.FireAutoPageEvent();
    }
    public void SetValues(List<TValue> values, int pageIndex)
    {
        autoPage.GotoPage(pageIndex);
        SetValues(values);
    }
    protected override bool ValidIndex(int index)
    {
        return index >= 0 && index < autoPage.Values.Count;
    }
    public void TweenReset(bool fromRightToLeft = true)
    {
        transform.localPosition = Vector2.zero;
        switch (rt)
        {
            case ResetType.ImmedMove: break;
            case ResetType.WholeMove:
            case ResetType.OrderMove:
            case ResetType.CrossMove:
                {
                    int num = Mathf.Min(count, 10);
                    for (int index = 0; index < num; ++index)
                    {
                        var it = GetItem(index);
                        var loc = it.transform.localPosition;
                        loc.x = CaculateOffset(fromRightToLeft, index, num);
                        it.transform.localPosition = loc;
                        loc.x = 0;
                        TweenPosition.Begin(it.gameObject, tweenDuration, loc).Play();
                    }
                }
                break;
        }
    }
    float CaculateOffset(bool fromRightToLeft, int index, int num)
    {
        const int moveDistance = 500;
        switch (rt)
        {
            case ResetType.ImmedMove: break;
            case ResetType.WholeMove: return moveDistance * (fromRightToLeft ? 1 : -1);
            case ResetType.OrderMove: return moveDistance * (fromRightToLeft ? 1 : -1) * (num - index) / num;
            case ResetType.CrossMove: return moveDistance * (index % 2 == 0 ? 1 : -1);
        }
        return 0f;
    }
    public bool SlideLeft()
    {
        if (autoPage.PreviousPage())
        {
            autoPage.FireAutoPageEvent();
            TweenReset(false);
            return true;
        }
        return false;
    }
    public bool SlideRight()
    {
        if (autoPage.NextPage())
        {
            autoPage.FireAutoPageEvent();
            TweenReset(true); 
            return true;
        }
        return false;
    }
    public bool XSlideTo(int offset)
    {
        if (autoPage.GotoPage(offset))
        {
            autoPage.FireAutoPageEvent();
            TweenReset(); 
            return true;
        }
        return false;
    }
}