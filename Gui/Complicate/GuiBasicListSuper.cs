using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class GuiBasicListSuper<TItem, TItemDef, TValue> : IGuiList<TItem, TItemDef, TValue>
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    public TItemDef listItem;
    public RectTransform clipWindow;
    public int column = 1;
    public LayoutType layout = LayoutType.Vert;
    public IList<TValue> values { get { return logic.values; } }
    public List<TItem> listItems { get { return logic.listItems; } }
    public int displayRow { get { return logic.row; } }
    public int displayColumn { get { return logic.column; } }

    protected abstract float offset { get; }

    protected ISuperLogic<TItem, TValue> logic;// = new SuperVerticalDisplayLogic<TItem, TValue>();
    protected IFactory<TItem> factory;
    protected RectTransform m_cachedRc;
    public override void OnInitialize()
    {
        base.OnInitialize();
        m_cachedRc = (RectTransform)transform;
        if (clipWindow == null) { clipWindow = (RectTransform)transform.parent; }

        if (logic != null)
        {
            factory.OnInitialize(this);
            (logic as ICustomSuperLogic<TItem, TValue>).OnInitialize(m_cachedRc, clipWindow, factory as ICustomFactory<TItem, TValue>);
        }
        else
        {
            factory = new NewFactory<TItem, TItemDef, TValue>();
            var prefab = new PrefabSuperLogic<TItem, TItemDef, TValue>();
            logic = prefab;
            switch (layout)
            {
                case LayoutType.None: break;
                case LayoutType.Hori: new HoriontalSuperDisplayImpl<TItem, TValue>().Attach(prefab); break;
                case LayoutType.Vert: new VerticalSuperDisplayImpl<TItem, TValue>().Attach(prefab); break;
                case LayoutType.Grid: break;
            }
            factory.OnInitialize(this);
            prefab.OnInitialize(m_cachedRc, clipWindow, factory as IPrefabFactory<TItem, TItemDef>, listItem, offset);
        }
    }
    public override void OnUnInitialize()
    {
        logic.OnUnInitialize();
        factory.OnUnInitialize();
        base.OnUnInitialize();
    }
    public override void OnShow()
    {
        base.OnShow();
        logic.OnShow();
    }
    public override void OnHide()
    {
        logic.OnHide();
        base.OnHide();
    }
    public override void SetValues(IList<TValue> values)
    {
        ClearAllSelect();
        logic.SetValues(values, column);
    }
    protected override bool ValidIndex(int index)
    {
        return index >= 0 && index < values.Count;
    }
    public int count { get { return listItems.Count; } }
    public TItem GetItem(int index)
    {
        if (ValidIndex(index))
        {
            for (int displayIndex = 0; displayIndex < listItems.Count; ++displayIndex)
            {
                if (listItems[displayIndex].index == index) { return listItems[displayIndex]; }
            }
        }
        return null;
    }
}