using UnityEngine;

public class TweenScale : UITweener
{
    [SerializeField]
    public Vector3 from = Vector3.one;
    [SerializeField]
    public Vector3 to = Vector3.one;
    public Vector3 scale { get { return transform.localScale; } set { transform.localScale = value; } }
    static public TweenScale Begin(GameObject go, float duration, Vector3 scale)
    {
        TweenScale comp = go.GetComponent<TweenScale>();
        if (comp == null) { comp = go.AddComponent<TweenScale>(); }
        comp.from = comp.scale;
        comp.to = scale;
        comp.Play();
		return comp;
    }
    protected override void OnStep(float factor)
    {
        transform.localScale = from * (1f - factor) + to * factor;
    }
}
