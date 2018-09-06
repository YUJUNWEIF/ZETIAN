using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class DragGridCell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Util.Param1Actions<PointerEventData> onClick = new Util.Param1Actions<PointerEventData>();
    public Util.Param1Actions<PointerEventData> onPress = new Util.Param1Actions<PointerEventData>();
    public Util.Param1Actions<PointerEventData> onRelease = new Util.Param1Actions<PointerEventData>();
    public Util.Param1Actions<DragGridCell> onSwap = new Util.Param1Actions<DragGridCell>();
    public Util.Param1Actions<DragGridCell> onBeSwap = new Util.Param1Actions<DragGridCell>();
    public Util.Param1Actions<PointerEventData> onDrop = new Util.Param1Actions<PointerEventData>();
    public Util.Param1Actions<PointerEventData> onDragging = new Util.Param1Actions<PointerEventData>();

    RectTransform m_cachedRc;
    Transform m_parent;
    Vector3 m_localPosition;
    bool m_dragging;
    public bool clickable;
    public bool draggle;

    void Awake() { m_cachedRc = transform as RectTransform; }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickable && !eventData.dragging) { onClick.Fire(eventData); }
    }
    public void OnPointerDown(PointerEventData eventData) { if (clickable || draggle) { onPress.Fire(eventData); } }
    public void OnPointerUp(PointerEventData eventData) { if (clickable || draggle) { onRelease.Fire(eventData); } }
    
    public void OnBeginDrag(PointerEventData sender)
    {
        if (!draggle) { return; }
        if (m_dragging)
        {
            ReleaseDrag();
        }
        m_dragging = true;
        m_parent = transform.parent;
        m_localPosition = transform.localPosition;        
    }
    public void OnDrag(PointerEventData ev)
    {
        if (!draggle || !m_dragging) { return; }

        var loc = Vector2.zero;
        var uiParent = transform.parent as RectTransform;
        if (uiParent)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, ev.position, ev.pressEventCamera, out loc);
            transform.localPosition = loc;
        }
        else
        {
            var world = ev.pressEventCamera.ScreenToWorldPoint(ev.position);
            transform.localPosition = transform.parent.worldToLocalMatrix.MultiplyPoint3x4(world);
        }        
        onDragging.Fire(ev);
    }
    public void OnEndDrag(PointerEventData ev)
    {
        //if (!draggle) { return; }
        if (!m_dragging) { return; }

        List<RaycastResult> list = new List<RaycastResult>();
        GraphicRaycaster raycaster = transform.GetComponentInParent<GraphicRaycaster>();
        raycaster.Raycast(ev, list);
        DragGridCell script = null;
        for (int index = 0; index < list.Count; ++index)
        {
            var go = list[index].gameObject;
            if (go)
            {
                script = go.GetComponentInParent<DragGridCell>();
                if (script != this) { break; }
            }
            else
            {
                continue;
            }
        }
        if (script)
        {
            onSwap.Fire(script);
            script.onBeSwap.Fire(this);
        }
        else
        {
            onDrop.Fire(ev);
        }
        ReleaseDrag();
    }
    public void ReleaseDrag()
    {
        if (m_dragging)
        {
            m_dragging = false;
            transform.SetParent(m_parent);
            transform.localPosition = m_localPosition;
        }
    }
    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            ReleaseDrag();
        }
    }
}