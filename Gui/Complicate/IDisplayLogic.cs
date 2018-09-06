using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

//public interface ISuperLogic<TItem, TValue>
//{
//    RectTransform lister { get; }
//    RectTransform cliper { get; }
//    IList<TValue> values { get; }
//    List<TItem> listItems { get; }
//    float offset { get; }
//    int row { get; }
//    int column { get; }
//    int cWidth { get; }
//    int cHeight { get; }
//    //void OnInitialize(RectTransform cachedRc, TItemDef listItem, RectTransform clipWindow, float offset = 0f);
//    void OnUnInitialize();
//    void OnShow();
//    void OnHide();
//    void SetValues(IList<TValue> values, int column);
//    void Goto(int index);
//    void UpdateDisplayItem();
//}

public interface IPrefabSuperLogic<TItem, TItemDef>
{
    void OnInitialize(RectTransform cachedRc, RectTransform clipWindow, IPrefabFactory<TItem, TItemDef> factory, TItemDef listItem, float offset = 0f);
}

public interface ICustomSuperLogic<TItem, TValue>
{
    void OnInitialize(RectTransform cachedRc, RectTransform clipWindow, ICustomFactory<TItem, TValue> m_factory);
}