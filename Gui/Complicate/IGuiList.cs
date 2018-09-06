using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public interface IListLocalizable
{
    void SlidePrev();
    void SlideNext();
    void SlideTo(int offset);
    void Goto(int offset);
}

public abstract class IGuiList<TListItem, TItemDef, TValue> : IListBase
    where TListItem : IItemLogic<TItemDef>, IListItemBase<TValue>
    where TItemDef : Component
{
    public abstract void SetValues(IList<TValue> values);
}