using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GuiListSuper<TItem, TItemDef, TValue> : GuiBasicListSuper<TItem, TItemDef, TValue>, IListLocalizable
    where TItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            logic.UpdateDisplayItem();
            transformChanged.Fire();
        }
    }
    protected override float offset { get { return 0f; } }
    public void SlidePrev() { }
    public void SlideNext() { }
    public void SlideTo(int offset) { }
    public void Goto(int index)
    {
        coroutineHelper.StopAll();
        logic.Goto(index);
    }
    public void SetCustomDisplayLogic(ISuperLogic<TItem, TValue> log, ICustomFactory<TItem, TValue> fac)
    {
        if (logic == null)
        {
            logic = log;
            factory = fac;
        }
    }
}