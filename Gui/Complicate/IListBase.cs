using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IListItem
{
    int index { get; set; }
    IListBase ListComponent { get; set; }
}

public interface IListItemBase<TValue> : IListItem
{
    TValue AttachValue { get; set; }
}

public abstract class IItemLogic<TItemDef> : BehaviorWrapper
    where TItemDef : Component
{
    public TItemDef def { get; private set; }
    public IItemSelectDisplayer disp { get; private set; }
    public override void OnInitialize()
    {
        def = GetComponent<TItemDef>();
        if (!def) { def = gameObject.AddComponent<TItemDef>(); }
        disp = GetComponent<GuiListItemSelector>();
        if (disp != null) { disp.OnInitialize(this as IListItem); }
    }
    public override void OnUnInitialize()
    {
        if (disp != null) { disp.OnUnInitialize(); }
        base.OnUnInitialize();
    }
}

public abstract class IListBase : BehaviorWrapper
{
    public bool singleSelect = true;
    public bool forceSelect = false;
    public int maxSelectCount = 8;
    public readonly LinkedList<int> itemSelected = new LinkedList<int>();
    public readonly Util.ParamActions selectListener = new Util.ParamActions();
    public readonly Util.ParamActions transformChanged = new Util.ParamActions();//patch for supper list
    public void SwitchSelectIndex(int index)
    {
        if (IsIndexSelect(index)) { UnselectIndex(index); }
        else { SelectIndex(index); }
    }
    public void SelectIndex(int index)
    {
        if (!ValidIndex(index)) { return; }

        if (!forceSelect)
        {
            if (IsIndexSelect(index)) { return; }

            if (singleSelect)
            {
                itemSelected.Clear();
                itemSelected.AddLast(index);
                FireSelectChangeNotify();
            }
            else
            {
                if (itemSelected.Count < maxSelectCount)
                {
                    itemSelected.AddLast(index);
                    FireSelectChangeNotify();
                }
            }
        }
        else
        {
            if (singleSelect)
            {
                if (!IsIndexSelect(index))
                {
                    itemSelected.Clear();
                    itemSelected.AddLast(index);
                }
                FireSelectChangeNotify();
            }
            else
            {
                if (IsIndexSelect(index))
                {
                    FireSelectChangeNotify();
                }
                else if (itemSelected.Count < maxSelectCount)
                {
                    itemSelected.AddLast(index);
                    FireSelectChangeNotify();
                }
            }
        }
    }
    public void UnselectIndex(int index)
    {
        if (!singleSelect)
        {
            var no = itemSelected.Find(index);
            if (no != null)
            {
                itemSelected.Remove(no);
                FireSelectChangeNotify();
            }
        }
    }
    public bool IsIndexSelect(int index)
    {
        return itemSelected.Contains(index);
    }
    public void FireSelectChangeNotify()
    {
        selectListener.Fire();
    }
    public void ClearAllSelect()
    {
        if (itemSelected.Count > 0)
        {
            itemSelected.Clear();
            FireSelectChangeNotify();
        }
    }
    protected abstract bool ValidIndex(int index);
}
