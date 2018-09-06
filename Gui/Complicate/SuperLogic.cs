using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public abstract class ISuperLogic<TItem, TValue>
    //: ISuperLogic<TItem, TValue>
    where TItem : Component
{
    protected List<TItem> m_listItems = new List<TItem>();
    public RectTransform lister { get; protected set; }
    public RectTransform cliper { get; protected set; }
    public float offset { get; protected set; }
    public IList<TValue> values { get; protected set; }
    public List<TItem> listItems { get { return m_listItems; } }
    public int column { get { return impl.column; } }
    public int row { get { return impl.row; } }
    public abstract int cWidth { get; }
    public abstract int cHeight { get; }
    public abstract void UpdateDisplayItem();
    public ISuperDisplayImpl impl;
    protected void _Init(RectTransform lister, RectTransform cliper, float offset = 0f)
    {
        this.lister = lister;
        this.cliper = cliper;
        this.offset = offset;
    }
    public void OnUnInitialize()
    {
        for (int index = 0; index < m_listItems.Count; ++index) { Util.UnityHelper.CallUnInitialize(m_listItems[index]); }
    }
    public void OnShow()
    {
        for (int index = 0; index < m_listItems.Count; ++index) { Util.UnityHelper.CallOnShow(m_listItems[index]); }
    }
    public void OnHide()
    {
        for (int index = 0; index < m_listItems.Count; ++index) { Util.UnityHelper.CallOnHide(m_listItems[index]); }
    }
    public void SetValues(IList<TValue> values, int column)
    {
        this.values = values;
        impl.SetValues(column);
        UpdateDisplayItem();
    }
    public void Goto(int index)
    {
        impl.Goto(index);
    }
}

public class PrefabSuperLogic<TItem, TItemDef, TValue>
    : ISuperLogic<TItem, TValue>, IPrefabSuperLogic<TItem, TItemDef>
    where TItem : Component, IListItemBase<TValue>
    where TItemDef : Component
{
    TItemDef m_def;
    RectTransform m_defRc;
    IPrefabFactory<TItem, TItemDef> m_factory;

    public void OnInitialize(RectTransform lister, RectTransform cliper, IPrefabFactory<TItem, TItemDef> factory, TItemDef listItem, float offset = 0f)
    {
        this._Init(lister, cliper, offset);
        m_def = listItem;
        m_factory = factory;
        m_defRc = (RectTransform)m_def.transform;
    }
    public override int cWidth { get { return Mathf.RoundToInt(m_defRc.rect.size.x); } }
    public override int cHeight { get { return Mathf.RoundToInt(m_defRc.rect.size.y); } }
    public override void UpdateDisplayItem()
    {
        if (m_def == null || values == null) { return; }

        var dc = impl.DisplayCount();
        var actualDisplayCount = dc.max - dc.current;
        if (actualDisplayCount > m_listItems.Count)
        {
            int loop = actualDisplayCount - m_listItems.Count;
            for (int index = 0; index < loop; ++index)
            {
                var script = m_factory.NewItem(m_def);
                m_listItems.Add(script);
            }
        }
        else if (actualDisplayCount < m_listItems.Count)
        {
            for (int index = actualDisplayCount; index < m_listItems.Count; ++index)
            {
                var item = m_listItems[index];
                m_factory.DeleteItem(item);
            }
            m_listItems.RemoveRange(actualDisplayCount, m_listItems.Count - actualDisplayCount);
        }

        for (int index = dc.current; index < dc.max; ++index)
        {
            var it = m_listItems[index - dc.current];
            it.index = index;
            it.name = @"_" + index.ToString();
            it.AttachValue = values[index];
            impl.Layout((RectTransform)it.transform, index);
        }
    }
}

public class CustomSuperLogic<TItem, TValue> :
    ISuperLogic<TItem, TValue>, ICustomSuperLogic<TItem, TValue>
    where TItem : Component, IListItemBase<TValue>
{
    ICustomFactory<TItem, TValue> m_factory;//= new NewFactory<TItem, TItemDef, TValue>();
    public override int cWidth { get { return m_factory.cWidth; } }
    public override int cHeight { get { return m_factory.cHeight; } }
    public void OnInitialize(RectTransform lister, RectTransform cliper, ICustomFactory<TItem, TValue> factory)
    {
        base.lister = lister;
        base.cliper = cliper;
        m_factory = factory;
    }
    public override void UpdateDisplayItem()
    {
        if (values == null) { return; }

        var dc = impl.DisplayCount();
        for (int index = 0; index < m_listItems.Count; ++index)
        {
            m_factory.DeleteItem(m_listItems[index]);
        }
        m_listItems.Clear();
        for (int index = dc.current; index < dc.max; ++index)
        {
            var it = m_factory.NewItem(values[index]);
            it.index = index;
            it.name = @"_" + index.ToString();
            it.AttachValue = values[index];
            m_listItems.Add(it);
            impl.Layout((RectTransform)it.transform, index);
        }
    }
}