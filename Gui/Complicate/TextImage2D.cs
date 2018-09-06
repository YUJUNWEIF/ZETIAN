using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class TextImage2D : TextImage<TextImage2DRender>, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public RectTransform rectTransform { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        rectTransform = (RectTransform)transform;
    }
    protected override void OnEnable()
    {
        width = Mathf.RoundToInt(rectTransform.sizeDelta.x);
        base.OnEnable();
    }
    public override void AfterProc()
    {
        rectTransform.sizeDelta = new Vector2(width, height);
    }
    protected void LateUpdate()
    {
        int current = Mathf.RoundToInt(rectTransform.sizeDelta.x);
        if (current != width)
        {
            width = current;
            SetAllDirty();
        }
    }
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);
        for (int index = 0; index < m_hrefInfos.Count; ++index)
        {
            var hrefInfo = m_hrefInfos[index];
            var boxes = hrefInfo.boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    onHrefClick.Fire(hrefInfo.content);
                    return;
                }
            }
        }
    }
}
