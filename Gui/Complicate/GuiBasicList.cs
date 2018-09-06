using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GuiListBasic<TItem, TItemDef, TValue> : IGuiList<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    protected List<TItem> m_listItems = new List<TItem>();
    protected IPrefabFactory<TItem, TItemDef> factory { get; private set; }
    protected ILayout m_layout { get; private set; }
    public TItemDef listItem;
    public FactoryType factoryType { get; set; }
    public LayoutType layoutType { get; set; }
    public int count { get { return m_listItems.Count; } }
    public List<TItem> listItems { get { return m_listItems; } }
    public IList<TValue> values { get; protected set; }
    public Util.ParamActions onValueChanged = new Util.ParamActions();
    public override void OnInitialize()
    {
        base.OnInitialize();
        switch (factoryType)
        {
            case FactoryType.None: break;
            case FactoryType.New: factory = new NewFactory<TItem, TItemDef, TValue>(); break;
            case FactoryType.Pregen: factory = new PregenFactory<TItem, TItemDef, TValue>(); break;
            case FactoryType.Pool: factory = new PoolFactory<TItem, TItemDef, TValue>(); break;
        }
        switch (layoutType)
        {
            case LayoutType.None: break;
            case LayoutType.Hori: m_layout = new HorizontalLayout(true); break;
            case LayoutType.Vert: m_layout = new VerticalLayout(true); break;
            case LayoutType.Grid: m_layout = new TableLayout(); break;
            case LayoutType.CircleClockwise: m_layout = new CircleClockwiseLayout(); break;
            case LayoutType.HoriRight2Left: m_layout = new HorizontalLayout(false); break;
            case LayoutType.VertBottom2Top: m_layout = new VerticalLayout(false); break;
        }
        factory.OnInitialize(this);
    }
    public override void OnUnInitialize()
    {
        Clear();
        factory.OnUnInitialize();
        base.OnUnInitialize();
    }
    public override void OnShow()
    {
        base.OnShow();
        for (int index = 0; index < m_listItems.Count; ++index)
        {
            m_listItems[index].OnShow();
        }
    }
    public override void OnHide()
    {
        for (int index = 0; index < m_listItems.Count; ++index)
        {
            m_listItems[index].OnHide();
        }
        base.OnHide();
    }
    public void Foreach(System.Predicate<TItem> it)
    {
        for (int index = 0; index < m_listItems.Count; ++index) { it(m_listItems[index]); }
    }
    public TItem Find(System.Predicate<TItem> match) { return m_listItems.Find(match); }
    public int FindIndex(System.Predicate<TItem> match) { return m_listItems.FindIndex(match); }

    public override void SetValues(IList<TValue> values)
    {
        this.values = values;
        if (values.Count > m_listItems.Count)
        {
            int loop = values.Count - m_listItems.Count;
            for (int index = 0; index < loop; ++index)
            {
                TItem script = factory.NewItem(listItem);
                m_listItems.Add(script);
            }
        }
        else if (values.Count < m_listItems.Count)
        {
            for (int index = values.Count; index < m_listItems.Count; ++index)
            {
                TItem item = m_listItems[index];
                factory.DeleteItem(item);
            }
            m_listItems.RemoveRange(values.Count, m_listItems.Count - values.Count);
        }
        for (int index = 0; index < values.Count; ++index)
        {
            m_listItems[index].index = index;
            if (factoryType == FactoryType.New || (factoryType == FactoryType.Pool && listItem))
            {
                m_listItems[index].name = listItem.name + "_" + index.ToString();
            }
            m_listItems[index].AttachValue = values[index];
        }
        FireListItemChangeNotify();
        ClearAllSelect();
    }
    public void RmvItem(int startIndex)
    {
        if (ValidIndex(startIndex))
        {
            values.RemoveAt(startIndex);
            factory.DeleteItem(m_listItems[startIndex]);
            m_listItems.RemoveAt(startIndex);
            if (IsIndexSelect(startIndex)) { UnselectIndex(startIndex); }
            RefreshData(startIndex);
            FireListItemChangeNotify();
        }
    }
    public void RmvItems(int startIndex, int count)
    {
        for (int index = 0; index < count; ++index)
        {
            if (ValidIndex(startIndex))
            {
                values.RemoveAt(startIndex);
                var item = m_listItems[startIndex];
                factory.DeleteItem(item);
                m_listItems.RemoveAt(startIndex);
            }
            if (IsIndexSelect(startIndex + index)) { UnselectIndex(startIndex + index); }
            RefreshData(startIndex);
        }
        FireListItemChangeNotify();
    }
    public void AddItem(TValue value) { AddItem(values.Count, value); }
    public void AddItem(int startIndex, TValue value)
    {
        if (startIndex >= 0 && startIndex <= values.Count)
        {
            values.Insert(startIndex, value);
            TItem script = factory.NewItem(listItem);
            m_listItems.Insert(startIndex, script);
            RefreshData(startIndex);
            FireListItemChangeNotify();
        }
    }
    public void AddItems(int startIndex, IList<TValue> ups)
    {
        if (startIndex >= 0 && startIndex <= values.Count)
        {
            for (int index = startIndex; index < startIndex + ups.Count; ++index)
            {
                TItem script = factory.NewItem(listItem);
                values.Insert(index, ups[index - startIndex]);
                m_listItems.Insert(index, script);
            }
            RefreshData(startIndex);
            FireListItemChangeNotify();
        }
    }
    void RefreshData(int startIndex)
    {
        for (int index = startIndex; index < values.Count; ++index)
        {
            m_listItems[index].index = index;
            if (factoryType == FactoryType.New || (factoryType == FactoryType.Pool && listItem))
            {
                m_listItems[index].name = listItem.name + "_" + index.ToString();
            }
            m_listItems[index].AttachValue = values[index];
        }
    }
    public void Clear()
    {
        ClearAllSelect();
        m_listItems.ForEach(it => factory.DeleteItem(it));
        m_listItems.Clear();
        this.values = null;
        FireListItemChangeNotify();
    }
    protected override bool ValidIndex(int index)
    {
        return index >= 0 && index < m_listItems.Count;
    }
    public TItem GetItem(int index)
    {
        return ValidIndex(index) ? m_listItems[index] : null;
    }
    public TValue GetValue(int index)
    {
        return ValidIndex(index) ? values[index] : default(TValue);
    }
    public virtual void FireListItemChangeNotify()
    {
        if (m_layout != null) { m_layout.OnLayout((RectTransform)transform, m_listItems.ConvertAll(it => (IListItem)it)); }
        onValueChanged.Fire();
    }
}

public class ListContainer<TItem, TItemDef, TValue> : GuiListBasic<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    public override void OnInitialize()
    {
        layoutType = LayoutType.None;
        factoryType = FactoryType.Pregen;
        base.OnInitialize();
    }
}

public class ListRandom<TItem, TItemDef, TValue> : GuiListBasic<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    public override void OnInitialize()
    {
        layoutType = LayoutType.None;
        factoryType = FactoryType.New;
        base.OnInitialize();
    }
}