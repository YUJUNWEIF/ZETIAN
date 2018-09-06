using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum LayoutType
{
    None,
    Hori,
    Vert,
    Grid,
    CircleClockwise,
    HoriRight2Left,
    VertBottom2Top,
}
public interface ILayout
{
    void OnLayout(RectTransform container, List<IListItem> elements);
}
public class VerticalLayout : ILayout
{
    bool m_top2Bottom;
    public VerticalLayout(bool top2Bottom) { m_top2Bottom = top2Bottom; }
    public void OnLayout(RectTransform container, List<IListItem> elements)
    {
        float y = 0;
        for (int index = 0; index < elements.Count; ++index)
        {
            var it = (elements[index] as MonoBehaviour).GetComponent<RectTransform>();
            if (m_top2Bottom)
            {
                //it.pivot = new Vector2(0.5f, 1f);
                it.anchorMin = new Vector2(0.5f, 1f);
                it.anchorMax = new Vector2(0.5f, 1f);
                it.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, it.rect.height);
            }
            else
            {
                it.anchorMin = new Vector2(0.5f, 0f);
                it.anchorMax = new Vector2(0.5f, 0f);
                it.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, y, it.rect.height);
            }
            y += it.rect.height;
        }
        container.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(y));
    }
    //public virtual void Goto(int index)
    //{
    //    index = Mathf.Clamp(index, 0, count - 1);
    //    float y = 0;
    //    for (int row = 0; row < index; ++row)
    //    {
    //        var it = (RectTransform)GetItem(row).transform;
    //        y += it.rect.height;
    //    }
    //    var me = (RectTransform)transform;
    //    me.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -y, me.rect.height);
    //}
}
public class HorizontalLayout : ILayout
{
    bool m_leftRight;
    public HorizontalLayout(bool left2Right) { m_leftRight = left2Right; }
    public void OnLayout(RectTransform container, List<IListItem> elements)
    {
        float x = 0;
        for (int index = 0; index < elements.Count; ++index)
        {
            var it = (elements[index] as MonoBehaviour).GetComponent<RectTransform>();
            if (m_leftRight)
            {
                //it.pivot = new Vector2(0f, 0.5f);
                it.anchorMin = new Vector2(0f, 0.5f);
                it.anchorMax = new Vector2(0f, 0.5f);
                it.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x, it.rect.width);
            }
            else
            {
                it.anchorMin = new Vector2(1f, 0.5f);
                it.anchorMax = new Vector2(1f, 0.5f);
                it.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, x, it.rect.width);
            }
            x += it.rect.width;
        }
        container.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
    }
    //public virtual void Goto(int index)
    //{
    //    index = Mathf.Clamp(index, 0, count - 1);

    //    float x = 0;
    //    for (int col = 0; col < index; ++col)
    //    {
    //        var it = (RectTransform)GetItem(col).transform;
    //        x += it.rect.width;
    //    }
    //    var me = (RectTransform)transform;
    //    me.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x, me.rect.width);
    //}
}
public class TableLayout : ILayout
{
    public void OnLayout(RectTransform container, List<IListItem> elements)
    {
        if (elements.Count <= 0)
        {
            container.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        }
        else
        {
            int column = 0;
            for (int index = 0; index < elements.Count; ++index)
            {
                var it = (elements[index] as MonoBehaviour).GetComponent<RectTransform>();
                if (index == 0)
                {
                    column = (int)(container.sizeDelta.x / it.sizeDelta.x);
                    if (column <= 0) { column = 1; }
                    container.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (elements.Count + column - 1) / column * it.sizeDelta.y);
                }

                //it.pivot = new Vector2(0f, 1f);
                it.anchorMin = new Vector2(0f, 1f);
                it.anchorMax = new Vector2(0f, 1f);

                float x = index % column * it.sizeDelta.x;
                float y = index / column * it.sizeDelta.y;
                it.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x, it.rect.width);
                it.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, it.rect.height);
            }
        }
    }
}
public class CircleClockwiseLayout : ILayout
{
    public float offset { get; set; }
    public float deltaRadian { get; set; }
    public float radius { get; private set; }
    public void OnLayout(RectTransform container, List<IListItem> elements)
    {
        radius = Mathf.Max(container.sizeDelta.x, container.sizeDelta.y) * 0.5f;
        for (int index = 0; index < elements.Count; ++index)
        {
            var it = (elements[index] as MonoBehaviour).GetComponent<RectTransform>();
            float itRadius = Mathf.Max(it.sizeDelta.x, it.sizeDelta.y) * 0.5f;
            it.pivot = new Vector2(0.5f, 0.5f);
            it.anchorMin = new Vector2(0.5f, 0.5f);
            it.anchorMax = new Vector2(0.5f, 0.5f);
            var radian = elements[index].index * deltaRadian;
            it.anchoredPosition = new Vector2(Mathf.Cos(offset - radian), Mathf.Sin(offset - radian)) * (radius - itRadius);
        }
        container.hasChanged = true;
    }
}