using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IItemSelectDisplayer
{
    void OnInitialize(IListItem it);
    void OnUnInitialize();
    void Select(bool flag);
}
public class GuiListItemSelector : MonoBehaviour, IItemSelectDisplayer
{
    public Transform[] selectRoots;
    public Transform[] unselectRoots;
    IListItem m_listItem;
    public void OnInitialize(IListItem it)
    {
        //var scripts = GetComponents<MonoBehaviour>();
        //Array.ForEach(scripts, it =>
        //{
        //    var baseInterfaces = it.GetType().GetInterfaces();
        //    for (int bi = 0; bi < baseInterfaces.Length; ++bi)
        //    {
        //        if (baseInterfaces[bi].IsGenericType && baseInterfaces[bi].GetGenericTypeDefinition() == typeof(IListItemBase<>))
        //        {
        //            m_listItem = it as IListItem;
        //            break;
        //        }
        //    }
        //});
        m_listItem = it;
        if (m_listItem != null && m_listItem.ListComponent != null)
        {
            m_listItem.ListComponent.selectListener.Add(OnSelectChanged);
            m_listItem.ListComponent.transformChanged.Add(OnSelectChanged);
        }
    }
    public void OnUnInitialize()
    {
        if (m_listItem != null && m_listItem.ListComponent != null)
        {
            m_listItem.ListComponent.selectListener.Rmv(OnSelectChanged);
            m_listItem.ListComponent.transformChanged.Rmv(OnSelectChanged);
        }
    }
    public void Select(bool flag)
    {
        for (int i = 0; i < selectRoots.Length; ++i)
        {
            Util.UnityHelper.ShowHide(selectRoots[i], flag);
        }
        for (int i = 0; i < unselectRoots.Length; ++i)
        {
            Util.UnityHelper.ShowHide(unselectRoots[i], !flag);
        }
    }
    void OnSelectChanged()
    {
        Select(m_listItem.ListComponent.IsIndexSelect(m_listItem.index));
    }
}