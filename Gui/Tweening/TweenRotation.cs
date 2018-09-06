using UnityEngine;

public class TweenRotation : UITweener
{
	public Vector3 from;
	public Vector3 to;
	public Quaternion rotation { get { return transform.localRotation; } set { transform.localRotation = value; } }
    static public TweenRotation Begin(GameObject go, float duration, Vector3 eulerAngles)
	{
        TweenRotation comp = go.GetComponent<TweenRotation>();
        if (comp == null) { comp = go.AddComponent<TweenRotation>(); }
		comp.from = comp.rotation.eulerAngles;
		comp.to = eulerAngles;
        comp.Play();
		return comp;
	}
    protected override void OnStep(float factor)
    {
        transform.localRotation = Quaternion.Euler(from * (1f - factor) + to * factor);
    }
}
