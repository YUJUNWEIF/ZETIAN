using UnityEngine;
public class TweenTransform : UITweener
{
	public Transform from;
	public Transform to;
	Vector3 tmpPos;
	Quaternion tmpRot;
	Vector3 tmpScale;
	static public TweenTransform Begin (GameObject go, float duration, Transform to) { return Begin(go, duration, null, to); }
	static public TweenTransform Begin (GameObject go, float duration, Transform from, Transform to)
    {
        TweenTransform comp = go.GetComponent<TweenTransform>();
        if (comp == null) { comp = go.AddComponent<TweenTransform>(); }
		comp.from = from;
		comp.to = to;
        if (from == null)
        {
            comp.tmpPos = comp.transform.position;
            comp.tmpRot = comp.transform.rotation;
            comp.tmpScale = comp.transform.localScale;
        }
        comp.Play();
		return comp;
    }
    protected override void OnStep(float factor)
    {
        if (to == null) { return; }
        if (from != null)
        {
            transform.position = from.position * (1f - factor) + to.position * factor;
            transform.localScale = from.localScale * (1f - factor) + to.localScale * factor;
            transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, factor);
        }
        else
        {
            transform.position = tmpPos * (1f - factor) + to.position * factor;
            transform.localScale = tmpScale * (1f - factor) + to.localScale * factor;
            transform.rotation = Quaternion.Slerp(tmpRot, to.rotation, factor);
        }
    }
}
