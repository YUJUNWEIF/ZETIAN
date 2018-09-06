using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class FunctionInterceptClick : UIBehaviour, IPointerClickHandler
{
    public bool interactable;
    public Util.Param1Actions<int> onClick = new Util.Param1Actions<int>();
    public int function { get; set; }
    private void Press()
    {
        if (!IsActive() || !interactable)
            return;

        onClick.Fire(function);
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Press();
    }
}