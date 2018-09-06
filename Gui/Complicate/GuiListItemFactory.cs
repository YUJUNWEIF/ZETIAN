using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum FactoryType
{
    None,
    New,
    Pregen,
    Pool,
}
public interface IFactory<TItem>
{
    void OnInitialize(IListBase list);
    void OnUnInitialize();
    void DeleteItem(TItem item);
}
public interface IPrefabFactory<TItem, TDef> : IFactory<TItem>
{
    TItem NewItem(TDef listItem);
}
public interface ICustomFactory<TItem, TValue> : IFactory<TItem>
{
    int cWidth { get; }
    int cHeight { get; }
    TItem NewItem(TValue value);
}

public class NewFactory<TItem, TDef, TValue> : IPrefabFactory<TItem, TDef>
    where TItem : Component, IListItemBase<TValue>
    where TDef : Component
{
    IListBase m_list;
    public void OnInitialize(IListBase list) { m_list = list; }
    public void OnUnInitialize() { }
    public void DeleteItem(TItem item)
    {
        Util.UnityHelper.Hide(item);
        Util.UnityHelper.CallUnInitialize(item);
        Util.UnityHelper.DestroyGameObjectNoUnInit(item.gameObject);
    }
    public TItem NewItem(TDef listItem)
    {
        var go = GameObject.Instantiate(listItem.gameObject);
        go.name = listItem.name;
        if (!go.gameObject.activeSelf) { go.gameObject.SetActive(true); }
        var comp = go.GetComponent<TDef>();

        TItem script = go.GetComponent<TItem>();
        if (script == null) { script = go.AddComponent<TItem>(); }
        script.ListComponent = m_list;
        Util.UnityHelper.CallInitialize(script);
        Util.UnityHelper.Show(script, m_list.transform);
        return script;
    }
}

public class PregenFactory<TItem, TDef, TValue> : IPrefabFactory<TItem, TDef>
    where TItem : Component, IListItemBase<TValue>
    where TDef : Component
{
    IListBase m_list;
    TItem[] m_listItems;
    LinkedList<TItem> m_freeItems = new LinkedList<TItem>();
    public void OnInitialize(IListBase list)
    {
        m_list = list;
        var comps = list.GetComponentsInChildren<TDef>(true);
        m_listItems = new TItem[comps.Length];
        for (int index = 0; index < comps.Length; ++index)
        {
            var comp = comps[index];
            var script = comp.gameObject.GetComponent<TItem>();
            if (script == null) { script = comp.gameObject.AddComponent<TItem>(); }
            script.ListComponent = m_list;
            comp.gameObject.SetActive(false);
            Util.UnityHelper.CallInitialize(script);
            m_freeItems.AddLast(script);
            m_listItems[index] = script;
        }
    }
    public void OnUnInitialize()
    {
        for (int index = 0; index < m_listItems.Length; ++index)
        {
            Util.UnityHelper.CallUnInitialize(m_listItems[index]);
        }
        m_freeItems.Clear();
    }
    public void DeleteItem(TItem it)
    {
        Util.UnityHelper.Hide(it);
        if (!m_freeItems.Contains(it)) { m_freeItems.AddLast(it); }
    }
    public TItem NewItem(TDef listItem)
    {
        for (int index = 0; index < m_listItems.Length; ++index)
        {
            var tmp = m_listItems[index];
            var no = m_freeItems.Find(tmp);
            if (no != null)
            {
                m_freeItems.Remove(no);
                Util.UnityHelper.Show(tmp);
                return tmp;
            }
        }
        return null;
    }
}

public class PoolFactory<TItem, TDef, TValue> : IPrefabFactory<TItem, TDef>
    where TItem : Component, IListItemBase<TValue>
    where TDef : Component
{
    IListBase m_list;
    LinkedList<TItem> m_freeItems = new LinkedList<TItem>();
    public void OnInitialize(IListBase list)
    {
        m_list = list;
    }
    public void OnUnInitialize() { }
    public void DeleteItem(TItem it)
    {
        Util.UnityHelper.Hide(it);
        Util.UnityHelper.CallUnInitialize(it);
        if (!m_freeItems.Contains(it))
        {
            m_freeItems.AddLast(it);
        }
    }
    public TItem NewItem(TDef listItem)
    {
        var no = m_freeItems.First;
        if (no != null)
        {
            var tmp = no.Value;
            m_freeItems.RemoveFirst();
            Util.UnityHelper.CallInitialize(tmp);
            Util.UnityHelper.Show(tmp);
            return tmp;
        }
        var item = GameObject.Instantiate(listItem.gameObject) as GameObject;
        item.name = listItem.name;

        if (!item.gameObject.activeSelf) { item.gameObject.SetActive(true); }
        TItem script = item.GetComponent<TItem>();
        if (script == null) { script = item.AddComponent<TItem>(); }
        script.ListComponent = m_list;
        Util.UnityHelper.CallInitialize(item.transform);
        Util.UnityHelper.Show(item.transform, m_list.transform);
        return script;
    }
}