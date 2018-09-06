using UnityEngine;

public class TweenPosition : UITweener
{
    [SerializeField]
    public Vector3 from;
    [SerializeField]
    public Vector3 to;    
    public Vector3 position 
    {
        get
        {
            var asUI = transform as RectTransform;
            if (asUI != null)
            {
                return asUI.anchoredPosition;
            }
            else
            {
                return transform.localPosition;
            }
        }
        set
        {
            var asUI = transform as RectTransform;
            if (asUI != null)
            {
                asUI.anchoredPosition = value;
            }
            else
            {
                transform.localPosition = value; 
            }
        }
    }
    protected override void OnStep(float factor)
    {
        position = from * (1f - factor) + to * factor;
    }
    static public TweenPosition Begin(GameObject go)
    {
        TweenPosition comp = go.GetComponent<TweenPosition>();
        if (comp == null) { comp = go.AddComponent<TweenPosition>(); }
        comp.from = comp.position;
        comp.Play();
        return comp;
    }
	static public TweenPosition Begin(GameObject go, float duration, Vector3 pos)
    {
        TweenPosition comp = Begin(go);
		comp.to = pos;
        comp.duration = duration;
		return comp;
	}
}
