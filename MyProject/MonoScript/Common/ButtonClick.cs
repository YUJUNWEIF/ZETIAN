using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace geniusbaby.ui
{
    public class ButtonClick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public AudioClip clip;
        public UITweener tweener;
        void Awake()
        {
            tweener = GetComponent<UITweener>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (tweener) { tweener.Play(); }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (tweener) { tweener.SampleAt(0); }
        }
    }
}