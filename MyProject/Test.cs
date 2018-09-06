using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerClickHandler
{
    public Ripple ripple;
    void OnEnable()
    {
        if (!ripple) { ripple = GetComponent<Ripple>(); }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ripple)
        {
            Vector2 loc;
            if (geniusbaby.Framework.ScreenToRectangleLocal(eventData.position, out loc))
            {
                ripple.AddRipple(loc, RIPPLE_TYPE.RIPPLE_TYPE_RUBBER, Random.Range(1f, 3.0f));
            }
        }
    }
}