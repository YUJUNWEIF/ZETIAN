using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The BranchingSpline class groups multiple three-dimensional curves together, which enables junctions, and branched paths.
/// </summary>
/// <remarks>
/// It provides functions for calculate positions, rotations, etc. on and handling an arbitrarily branching path. The path is defined by individual splines.
/// These splines can be linked together by making them share their SplineNodes with each other. Each SplineNode that is used by two or more splines registred in the array,
/// will be treated as a junction. 
/// </remarks>
[AddComponentMenu("SuperSplines/Other/Branching Spline")]
public class BranchingSpline : MonoBehaviour 
{
	public List<Spline> splines = new List<Spline>( );									///< An array of Splines that will be used as possible paths.
	private int recoursionCounter = 0;
    public FEBranchingSpline impl = new FEBranchingSpline();
	/// <summary>
	/// This function adds an offset to a BranchingSplineParameter while automatically switching splines when a juction is passed.
	/// </summary>
	/// <param name='bParam'>
	/// A BranchingSplineParameter.
	/// </param>
	/// <param name='distanceOffset'>
	/// An offset that shall be added to the BranchingSplineParameter (in game units).
	/// </param>
	/// <param name='bController'>
	/// A BranchingController-delegate that decides which path to follow if a junction is passed.
	/// </param>
	/// <returns>
	/// True if the spline used as reference path has been changed; False if not.
	/// </returns>
	public bool Advance( FEBranchingSplineParameter bParam, float distanceOffset, FEBranchingSpline.BranchingController bController )
	{
        return impl.Advance(bParam, distanceOffset, bController);
	}
	
	/// <summary>
	/// This function returns a point on the branched path for a BranchingSplineParameter.
	/// </summary>
	/// <returns>
	/// A point on the spline.
	/// </returns>
	/// <param name='bParam'>
	/// A BranchingSplineParameter.
	/// </param>
	public Vector3 GetPosition(FEBranchingSplineParameter bParam )
	{
        return impl.GetPosition(bParam).V3();
	}
	
	/// <summary>
	/// This function returns a rotation on the branched path for a BranchingSplineParameter.
	/// </summary>
	/// <returns>
	/// A rotation on the spline.
	/// </returns>
	/// <param name='bParam'>
	/// A BranchingSplineParameter.
	/// </param>
	public Quaternion GetOrientation(FEBranchingSplineParameter bParam )
    {
        return impl.GetOrientation(bParam).QU();
    }
	
	/// <summary>
	/// This function returns a tangent to the branched path for a BranchingSplineParameter.
	/// </summary>
	/// <returns>
	/// A tangent to the spline.
	/// </returns>
	/// <param name='bParam'>
	/// A BranchingSplineParameter.
	/// </param>
	public Vector3 GetTangent(FEBranchingSplineParameter bParam )
    {
        return impl.GetTangent(bParam).V3();
	}
	
	/// <summary>
	/// This function returns a custom value on the branched path for a BranchingSplineParameter.
	/// </summary>
	/// <returns>
	/// A custom value on the spline.
	/// </returns>
	/// <param name='bParam'>
	/// A BranchingSplineParameter.
	/// </param>
	public float GetCustomValue(FEBranchingSplineParameter bParam )
    {
        return impl.GetCustomValue(bParam);
    }
	
	
	/// <summary>
	/// This function returns a normal to the branched path for a BranchingSplineParameter.
	/// </summary>
	/// <returns>
	/// A normal to the spline.
	/// </returns>
	/// <param name='bParam'>
	/// A BranchingSplineParameter.
	/// </param>
	public Vector3 GetNormal(FEBranchingSplineParameter bParam )
    {
        return impl.GetNormal(bParam).V3();
	}
}