using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace geniusbaby.ui
{
    public class Tipable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        LSharpItemPanel script;
        void Awake()
        {
            script = GetComponent<LSharpItemPanel>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (script) script.InstanceInvoke("OnPointerEnter");
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (script) script.InstanceInvoke("OnPointerExit");
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (script) script.InstanceInvoke("OnPointerDown");
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (script) script.InstanceInvoke("OnPointerUp");
        }
    }
}