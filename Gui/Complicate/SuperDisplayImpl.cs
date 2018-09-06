using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public interface ISuperDisplayImpl
{
    int column { get; }
    int row { get; }
    void SetValues(int column);
    void Goto(int index);
    RangeValue DisplayCount();
    void Layout(RectTransform rc, int index);
}

public class VerticalSuperDisplayImpl<TItem, TValue> : ISuperDisplayImpl
    where TItem : Component
{
    ISuperLogic<TItem, TValue> logic;
    public int column { get; private set; }
    public int row { get { return (logic.values.Count + column - 1) / column; } }

    public ISuperDisplayImpl Attach(ISuperLogic<TItem, TValue> displayer)
    {
        this.logic = displayer;
        this.logic.impl = this;
        return this;
    }
    public void SetValues(int column)
    {
        if (column <= 0)
        {
            column = Mathf.FloorToInt(logic.cliper.rect.width / logic.cWidth);
        }
        if (column <= 0)
        {
            column = 1;
        }
        this.column = column;
        logic.lister.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, row * logic.cHeight);
    }
    public void Goto(int index)
    {
        index = Mathf.Clamp(index, 0, logic.values.Count - 1);
        //var row = (index + column - 1) / column;
        var row = index / column;
        logic.lister.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -row * logic.cHeight, logic.lister.rect.height);
    }
    public RangeValue DisplayCount()
    {
        int maxCount = ((int)(logic.cliper.rect.height / logic.cHeight) + 2) * column;
        int startIndex = Mathf.Max(0, (int)((logic.lister.anchoredPosition.y - logic.offset) / logic.cHeight)) * column;
        int actualCount = Mathf.Max(Mathf.Min(logic.values.Count - startIndex, maxCount), 0);
        return new RangeValue(startIndex, startIndex + actualCount);
    }
    public void Layout(RectTransform rc, int index)
    {
        if (column > 1 || rc.anchorMin == rc.anchorMax)
        {
            Vector2 anchor = new Vector2(0, 1);
            rc.anchorMin = anchor;
            rc.anchorMax = anchor;
            //rc.pivot = anchor;
            var offset = new Vector2(rc.pivot.x * logic.cWidth, (rc.pivot.y - 1) * logic.cHeight);

            rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, index % column * logic.cWidth, logic.cWidth);
            rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, index / column * logic.cHeight, logic.cHeight);
        }
        else
        {
            //if (rc.anchorMin != rc.anchorMax)
            //{
            //    rc.offsetMin = m_prefabRc.offsetMin;
            //    rc.offsetMax = m_prefabRc.offsetMax;
            //}

            Vector2 anchorMin = rc.anchorMin;
            anchorMin.y = 1f;
            rc.anchorMin = anchorMin;

            Vector2 anchorMax = rc.anchorMax;
            anchorMax.y = 1f;
            rc.anchorMax = anchorMax;

            //rc.pivot = new Vector2(0.5f, 1f);
            //var offset = new Vector2(rc.pivot.x * cellWidth, (rc.pivot.y - 1) * cellHeight);
            //rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, offset.x, cellWidth);
            rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, index / column, logic.cHeight);
        }
    }
}

public class HoriontalSuperDisplayImpl<TItem, TValue> : ISuperDisplayImpl
    where TItem : Component
{
    ISuperLogic<TItem, TValue> logic;
    public int row { get; private set; }
    public int column { get { return (logic.values.Count + row - 1) / row; } }

    public ISuperDisplayImpl Attach(ISuperLogic<TItem, TValue> displayer)
    {
        this.logic = displayer;
        this.logic.impl = this;
        return this;
    }
    //public HoriontalDisplayLogicImpl(ISuperLogic<TItem, TValue> displayer) { this.displayer = displayer; }
    public void SetValues(int row = 1)
    {
        //this.row = row > 1 ? row : 1;
        if (row <= 0)
        {
            row = Mathf.FloorToInt(logic.cliper.rect.height / logic.cHeight);
        }
        if (row <= 0)
        {
            row = 1;
        }
        this.row = row;

        logic.lister.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, column * logic.cWidth);
        //UpdateDisplayItem();
    }
    public void Goto(int index)
    {
        index = Mathf.Clamp(index, 0, logic.values.Count - 1);
        //var column = (index + row - 1) / row;
        var column = index / row;
        logic.lister.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -column * logic.cWidth, logic.lister.rect.width);
    }
    public RangeValue DisplayCount()
    {
        int maxCount = ((int)(logic.cliper.rect.width / logic.cWidth) + 2) * row;
        int startIndex = Mathf.Max(0, (int)((-logic.lister.anchoredPosition.x - logic.offset) / logic.cWidth)) * row;
        int actualCount = Mathf.Max(Mathf.Min(logic.values.Count - startIndex, maxCount), 0);
        return new RangeValue(startIndex, startIndex + actualCount);
    }
    public void Layout(RectTransform rc, int index)
    {
        if (row > 1 || rc.anchorMin == rc.anchorMax)
        {
            //Vector2 anchor = new Vector2(0, 1);
            //rc.anchorMin = anchor;
            //rc.anchorMax = anchor;
            //rc.pivot = anchor;
            rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (index / row) * logic.cWidth, logic.cWidth);
            rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (index % row) * logic.cHeight, logic.cHeight);
        }
        else
        {
            Vector2 anchorMin = rc.anchorMin;
            anchorMin.x = 0f;
            rc.anchorMin = anchorMin;

            Vector2 anchorMax = rc.anchorMax;
            anchorMax.x = 0f;
            rc.anchorMax = anchorMax;

            //rc.pivot = new Vector2(0, 0.5f);
            rc.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (index / row) * logic.cWidth, logic.cWidth);
        }
    }
}
