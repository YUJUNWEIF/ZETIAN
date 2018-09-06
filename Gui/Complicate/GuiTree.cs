using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class NodeData<TValue>
{
    public TValue value;
    public bool expand;
    public bool select;
    public List<NodeData<TValue>> children = new List<NodeData<TValue>>();
}

public interface ITreeItem<TTreeItem, TValue>
    where TTreeItem : ITreeItem<TTreeItem, TValue>
{
    NodeData<TValue> AttachValue { get; set; }
    ITree<TTreeItem, TValue> tree { get; set; }
    void Free();
    void Relay();
}

public interface ITree<TTreeItem, TValue>
    where TTreeItem : ITreeItem<TTreeItem, TValue>
{
    TTreeItem Find(Predicate<TTreeItem> match);
    TTreeItem New(Transform parent);
    void Free(TTreeItem it);
    void Relay();
    void Clear();
}
[RequireComponent(typeof(RectTransform))]
public class TreeItem<ITreeItem, TValue> : BehaviorWrapper, ITreeItem<ITreeItem, TValue>
    where ITreeItem : TreeItem<ITreeItem, TValue>, ITreeItem<ITreeItem, TValue>
{
    NodeData<TValue> m_value;
    List<ITreeItem> m_children = new List<ITreeItem>();
    public List<ITreeItem> children { get { return m_children; } }
    public RectTransform cachedRc { get; private set; }
    public float containerHeight { get; private set; }
    public int shrink = 20;
    void Awake() { cachedRc = (RectTransform)transform; }
    public virtual NodeData<TValue> AttachValue { get { return m_value; } set { m_value = value; } }
    public ITree<ITreeItem, TValue> tree { get; set; }
    public void Free()
    {
        FreeChildren();
        tree.Free((ITreeItem)this);
    }
    void FreeChildren()
    {
        for (int index = 0; index < children.Count; ++index)
        {
            children[index].Free();
        }
        children.Clear();
    }
    public void Relay()
    {
        containerHeight = cachedRc.rect.height;
        if (m_value.expand)
        {
            if (children.Count > m_value.children.Count)
            {
                for (int index = m_value.children.Count; index < children.Count; ++index)
                {
                    children[index].Free();
                }
                children.RemoveRange(m_value.children.Count, children.Count - m_value.children.Count);
            }
            else if (children.Count < m_value.children.Count)
            {
                for (int index = children.Count; index < m_value.children.Count; ++index)
                {
                    children.Add(tree.New(transform));
                }
            }
            for (int index = 0; index < children.Count; ++index)
            {
                var child = children[index];
                child.AttachValue = m_value.children[index];
                child.Relay();

                //child.cachedRc.pivot = new Vector2(0f, 1f);
                child.cachedRc.anchorMin = new Vector2(0f, 1f);
                child.cachedRc.anchorMax = new Vector2(0f, 1f);
                child.cachedRc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, shrink, child.cachedRc.rect.width);
                child.cachedRc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, containerHeight, child.cachedRc.rect.height);
                containerHeight += (child.containerHeight);
            }
        }
        else
        {
            FreeChildren();
        }
    }
}

public class Tree<TTreeItem, TValue> : BehaviorWrapper, ITree<TTreeItem, TValue>
    where TTreeItem : TreeItem<TTreeItem, TValue>
{
    class Factory
    {
        Tree<TTreeItem, TValue> m_tree;
        TTreeItem m_listItem;
        LinkedList<TTreeItem> m_freeItems = new LinkedList<TTreeItem>();
        public void OnInitialize(Tree<TTreeItem, TValue> tree)
        {
            m_tree = tree;
            m_listItem = m_tree.listItem.GetComponent<TTreeItem>();
        }
        public void OnUnInitialize()
        {
            var no = m_freeItems.First;
            while (no != null)
            {
                Util.UnityHelper.CallUnInitialize(no.Value);
                no = no.Next;
            }
            m_freeItems.Clear();
        }
        public void DeleteItem(TTreeItem it)
        {
            Util.UnityHelper.Hide(it);
            //Util.UnityHelper.CallUnInitialize(it);
            if (!m_freeItems.Contains(it))
            {
                m_freeItems.AddLast(it);
            }
        }
        public TTreeItem NewItem(Transform parent)
        {
            var no = m_freeItems.First;
            if (no != null)
            {
                var tmp = no.Value;
                m_freeItems.RemoveFirst();
                //Util.UnityHelper.CallInitialize(tmp);
                Util.UnityHelper.Show(tmp, parent);
                return tmp;
            }
            var item = GameObject.Instantiate<GameObject>(m_listItem.gameObject);
            item.name = m_listItem.name;

            if (!item.gameObject.activeSelf) { item.gameObject.SetActive(true); }
            TTreeItem script = item.GetComponent<TTreeItem>();
            if (script == null) { script = item.AddComponent<TTreeItem>(); }
            script.tree = m_tree;
            Util.UnityHelper.CallInitialize(item.transform);
            Util.UnityHelper.Show(item.transform, parent);
            return script;
        }
    }
    protected List<TTreeItem> m_root = new List<TTreeItem>();
    Factory m_factory = new Factory();
    public List<NodeData<TValue>> m_data { get; private set; }
    public TTreeItem listItem;
    public override void OnInitialize()
    {
        base.OnInitialize();
        m_factory.OnInitialize(this);
    }
    public override void OnUnInitialize()
    {
        m_factory.OnUnInitialize();
        base.OnUnInitialize();
    }
    public void Relay()
    {
        var containerHeight = 0f;
        for (int index = 0; index < m_root.Count; ++index)
        {
            var child = m_root[index];
            child.Relay();

            //child.cachedRc.pivot = new Vector2(0f, 1f);
            child.cachedRc.anchorMin = new Vector2(0f, 1f);
            child.cachedRc.anchorMax = new Vector2(0f, 1f);
            child.cachedRc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, child.cachedRc.rect.width);
            child.cachedRc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, containerHeight, child.cachedRc.rect.height);
            containerHeight += (child.containerHeight);
        }
    }
    public void Clear()
    {
        for (int index = 0; index < m_root.Count; ++index)
        {
            m_root[index].Free();
        }
        m_root.Clear();
    }
    public TTreeItem New(Transform parent) { return m_factory.NewItem(parent); }
    public void Free(TTreeItem it) { m_factory.DeleteItem(it); }
    public void SetData(List<NodeData<TValue>> data)
    {
        Clear();
        m_data = data;
        for (int index = 0; index < data.Count; ++index)
        {
            var child = m_factory.NewItem(transform);
            child.AttachValue = data[index];
            m_root.Add(child);
        }
        Relay();
    }
    public TTreeItem Find(Predicate<TTreeItem> match)
    {
        return Find(m_root, match);
    }
    TTreeItem Find(List<TTreeItem> nodes, Predicate<TTreeItem> match)
    {
        for (int index = 0; index < nodes.Count; ++index)
        {
            var node = nodes[index];
            if (match(node)) { return node; }
            var result = Find(node.children, match);
            if (result != null) { return result; }
        }
        return null;
    }
}