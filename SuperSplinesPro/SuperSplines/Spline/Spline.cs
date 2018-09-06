using UnityEngine;

using System;
using System.Collections.Generic;

/// <summary>
/// The Spline class represents three-dimensional curves.
/// </summary>
/// <remarks>
/// It provides the most important functions that are necessary to create, calculate and render Splines.
/// The class derives from MonoBehaviour so it can be attached to gameObjects and used like any other self-written script.
/// </remarks>
[AddComponentMenu("SuperSplines/Spline")]
[ExecuteInEditMode]
public partial class Spline : MonoBehaviour
{
    SplineInterpolator splineInterpolator = new HermiteInterpolator();      ///< The SplineInterpolator that will be used for spline interpolation. 
    [SerializeField]
    InterpolationMode m_interpolationMode = InterpolationMode.Hermite;                              ///< Specifies what kind of curve interpolation will be used.
    [SerializeField]
    RotationMode m_rotationMode = RotationMode.Tangent;                                                 ///< Specifies how to calculate rotations on the spline.
    [SerializeField]
    TangentMode m_tangentMode = TangentMode.UseTangents;                                                ///< Specifies how tangents are calculated in hermite mode.
    [SerializeField]
    NormalMode m_normalMode = NormalMode.UseGlobalSplineNormal;                                        ///< Specifies how the spline's normal is defined. (mostly needed for RotationMode.Tangent)
    [SerializeField]
    Vector3 m_normal = Vector3.up;                                                                     ///< Spline's Normal / Up-Vector used to calculate rotations (only needed for RotationMode.Tangent)
    [SerializeField]
    bool m_autoClose = false;                                                                           ///< If set to true the spline start and end points of the spline will be connected. (Note that Bézier-Curves can't be auto-closed!)
    [SerializeField]
    int m_interpolationAccuracy = 3;                                                                   ///< Defines how accurately numeric calculations will be done.
    public List<SplineNode> splineNodesArray = new List<SplineNode>(); 									///< A collection of SplineNodes that are used as control nodes.

    public InterpolationMode interpolationMode
    {
        get { return m_interpolationMode; }
        set { m_interpolationMode = value; impl.interpolationMode = value; }
    }
	public RotationMode rotationMode
    {
        get { return m_rotationMode; }
        set { m_rotationMode = value; impl.rotationMode = value; }
    }
    public TangentMode tangentMode
    {
        get { return m_tangentMode; }
        set { m_tangentMode = value; impl.tangentMode = value; }
    }
    public NormalMode normalMode
    {
        get { return m_normalMode; }
        set { m_normalMode = value; impl.normalMode = value; }
    }
    public Vector3 normal
    {
        get { return m_normal; }
        set { m_normal = value; impl.normal = value.V3(); }
    }
    public bool autoClose
    {
        get { return m_autoClose; }
        set { m_autoClose = value; impl.autoClose = value; }
    }
    public int interpolationAccuracy
    {
        get { return m_interpolationAccuracy; }
        set { m_interpolationAccuracy = value; impl.interpolationAccuracy = value; }
    }

    public float Length { get { return impl.Length; } } 								///< Returns the length of the spline in game units.
	public bool AutoClose { get { return autoClose && interpolationMode != InterpolationMode.Bezier; } } ///< Returns true if spline is auto-closed. If the spline is a Bézier-Curve, false will always be returned.
	public int NodesPerSegment { get { return impl.NodesPerSegment; } }											///< Returns the number of spline nodes that are needed to describe a spline segment.
	public int SegmentCount { get { return impl.SegmentCount; } }      ///< Returns the number of spline segments. (Note that a spline segment of a Bézier-Curve is defined by 4 control nodes!)

    public FESpline impl = new FESpline();
    public FECacheSpline cacheImpl = new FECacheSpline();
    //void Awake()
    //{
    //    OnEnable();
    //}
    void OnEnable()
    {
        impl.unityUse = this;

        int id;
        int.TryParse(name, out id);
        impl.Id = id;
        impl.interpolationMode = interpolationMode;
        impl.rotationMode = rotationMode;
        impl.tangentMode = tangentMode;
        impl.normalMode = normalMode;
        impl.normal = normal.V3();
        impl.autoClose = autoClose;
        impl.interpolationAccuracy = interpolationAccuracy;
    }
    void Start()
    {
        UpdateSpline();
    }
    public List<FESplineNode> SplineNodes { get { return impl.SplineNodes; } }
    public FESplineSegment[] SplineSegments { get { return impl.SplineSegments; } }
    public Vector3 GetPositionOnSpline(float param) { return impl.GetPositionOnSpline(param).V3(); }
    public Vector3 GetTangentToSpline(float param) { return impl.GetTangentToSpline(param).V3(); }
    public Vector3 GetNormalToSpline(float param) { return impl.GetNormalToSpline(param).V3(); }
    public Vector3 GetCurvatureOfSpline(float param) { return impl.GetCurvatureOfSpline(param).V3(); }
    public Quaternion GetOrientationOnSpline(float param) { return impl.GetOrientationOnSpline(param).QU(); }
    public float GetCustomValueOnSpline(float param) { return impl.GetCustomValueOnSpline(param); }
    public FESplineSegment GetSplineSegment(float param) { return impl.GetSplineSegment(param); }
    public float ConvertNormalizedParameterToDistance(float param) { return impl.ConvertNormalizedParameterToDistance(param); }
    public float ConvertDistanceToNormalizedParameter(float param) { return impl.ConvertDistanceToNormalizedParameter(param); }
    public GameObject AddSplineNode()
    {
        if (splineNodesArray.Count > 0)
            return AddSplineNode(splineNodesArray[splineNodesArray.Count - 1]);
        else
            return AddSplineNode(null);
    }
    public void UpdateSpline()
    {
        int id;
        int.TryParse(name, out id);

        impl.Id = id;
        impl.interpolationMode = interpolationMode;
        impl.rotationMode = rotationMode;
        impl.tangentMode = tangentMode;
        impl.normalMode = normalMode;
        impl.normal = normal.V3();
        impl.autoClose = autoClose;
        impl.interpolationAccuracy = interpolationAccuracy;
        impl.splineNodesArray.Clear();
        splineNodesArray.ForEach(it => { if (it) { impl.splineNodesArray.Add(it.impl); } });
        impl.UpdateSpline();
        cacheImpl.FromSpline(impl, 2);
    }
    //public GameObject AddSplineNode( float normalizedParam )
    //{	
    //	if( SplineNodes.Length == 0 )
    //		return AddSplineNode( );

    //	SplineNode previousNode = null;

    //	foreach( var sNode in SplineNodes )
    //	{
    //		if( sNode.Parameters[this].position >= normalizedParam )
    //			return AddSplineNode( previousNode ); 

    //		previousNode = sNode;
    //	}

    //	return AddSplineNode( splineNodesArray[splineNodesArray.Count - 1] );
    //}
    public GameObject AddSplineNode(SplineNode precedingNode)
    {
        GameObject gObject = new GameObject();
        SplineNode splineNode = gObject.AddComponent<SplineNode>();

        int insertIndex;
        if (precedingNode == null)
            insertIndex = 0;
        else
            insertIndex = splineNodesArray.IndexOf(precedingNode) + 1;

        if (insertIndex == -1)
            throw (new ArgumentException("The SplineNode referenced by \"percedingNode\" is not part of the spline " + gameObject.name));

        splineNodesArray.Insert(insertIndex, splineNode);
        UpdateSpline();
        return gObject;
    }
    public void RemoveSplineNode(GameObject gObject)
    {
        SplineNode splineNode = gObject.GetComponent<SplineNode>();
        if (splineNode != null) RemoveSplineNode(splineNode);
    }
    public void RemoveSplineNode(SplineNode splineNode)
    {
        splineNodesArray.Remove(splineNode);
        UpdateSpline();
    }
}