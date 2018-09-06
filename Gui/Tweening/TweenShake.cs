using UnityEngine;

[ExecuteInEditMode]
public class TweenShake : UITweener
{
    public float distance;
    public float intensity;

    Ray m_ray;
    Vector3 m_orign;
    
    static public TweenShake Begin(GameObject go, float duration, float distance, float intensity)
    {
        TweenShake comp = go.GetComponent<TweenShake>();
        if (comp == null) { comp = go.AddComponent<TweenShake>(); }
        comp.distance = distance;
        comp.intensity = intensity;
        comp.Play();
        return comp;
    }
    protected override void OnBegin()
    {
        m_ray = new Ray(transform.position, transform.forward);
        m_orign = m_ray.GetPoint(distance);
    }
    protected override void OnStep(float factor)
    {
        var lookAt = m_orign + new Vector3(
             Random.Range(-intensity, intensity),
             Random.Range(-intensity, intensity),
             Random.Range(-intensity, intensity));
        transform.LookAt(lookAt);
    }
    protected override void OnStop()
    {
        transform.LookAt(m_orign);
    }
}
