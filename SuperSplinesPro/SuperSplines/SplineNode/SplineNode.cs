using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class represents a control node of the Spline.
/// </summary>
/// <remarks>
/// This class stores data about the position and orientation of the control node as well as information about 
/// the spline parameter that is associated to the control node and the normalized distance to the next adjacent control node.
/// For advanced use there is also a custom value that will be interpolated according to the interpolation mode that is used to calculate the spline.
/// </remarks>
[AddComponentMenu("SuperSplines/Spline Node")]
[ExecuteInEditMode]
public class SplineNode : MonoBehaviour
{
    FESplineNode m_impl = new FESplineNode();
    public FESplineNode impl
    {
        get
        {
            m_impl.position = transform.position.V3();
            m_impl.rotation = transform.rotation.QF();
            m_impl.CustomValue = customValue;
            m_impl.normal = normal.V3();
            return m_impl;
        }
    }
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; m_impl.position = value.V3(); }
    }
	public Quaternion Rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; m_impl.rotation = value.QF(); }
    }
	public float CustomValue
    {
        get { return customValue; }
        set { customValue = value; m_impl.CustomValue = value; }
    }
    public Vector3 CustomNormal
    {
        get { return normal; }
        set { normal = value; m_impl.normal = value.V3(); }
    }
    [SerializeField]
    Vector3 normal = Vector3.up;
    [SerializeField]
	float customValue = 0f;
    void Awake()
    {
        Refresh();
    }

    public void Refresh()
    {
        impl.unityUse = this;
        m_impl.position = transform.position.V3();
        m_impl.rotation = transform.rotation.QF();
        m_impl.CustomValue = customValue;
        m_impl.normal = normal.V3();
    }
    //void LateUpdate()
    //{
    //    if (transform.hasChanged)
    //    {

    //    }
    //}
    //void OnEnable()
    //{
    //    impl.unityUse = this;
    //    impl.position = transform.position.V3();
    //    impl.rotation = transform.rotation.QF();
    //    impl.CustomValue = customValue;
    //    impl.normal = normal.V3();
    //}
}