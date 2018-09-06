using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Spline : MonoBehaviour
{
    private int ControlNodeCount { get { return AutoClose ? SplineNodes.Count + 1 : SplineNodes.Count; } }
    private double InvertedAccuracy { get { return 1.0 / interpolationAccuracy; } }
    private bool IsBezier { get { return interpolationMode == InterpolationMode.Bezier; } }
    private bool HasNodes { get { return SplineNodes.Count > 0; } }
    void OnDrawGizmos( )
	{
        UpdateSpline( );
		
		if( !HasNodes )
			return;
		
		DrawSplineGizmo( new Color( 0.5f, 0.5f, 0.5f, 0.5f ) );
		
		Plane screen = new Plane( );
		Gizmos.color = new Color( 1f, 1f, 1f, 0.5f );
		
		
		screen.SetNormalAndPosition( Camera.current.transform.forward, Camera.current.transform.position );
		
		foreach( var node in SplineNodes)
			Gizmos.DrawSphere( node.position.V3(), GetSizeMultiplier( node ) * 2 );
	}
	
	void OnDrawGizmosSelected( )
	{
		UpdateSpline( );
		
		if( !HasNodes )
			return;
		
		DrawSplineGizmo( new Color( 1f, 0.5f, 0f, 1f ) );
		
		Gizmos.color = new Color( 1f, 0.5f, 0f, 0.75f );
		
		int nodeIndex = -1;
		
		foreach( var node in SplineNodes)
		{
			++nodeIndex;
			
			if( IsBezier && (nodeIndex % 3) != 0 )
				Gizmos.color = new Color( .8f, 1f, .1f, 0.70f );
			else
				Gizmos.color = new Color( 1f, 0.5f, 0f, 0.75f );
			
			Gizmos.DrawSphere( node.position.V3(), GetSizeMultiplier( node ) * 1.5f );
		}
	}
	
	void DrawSplineGizmo( Color curveColor )
    {
        Gizmos.color = curveColor;
        for (int index = 0; index < cacheImpl.Count - 1; ++index)
        {
            Gizmos.DrawLine(
            cacheImpl.GetPositionOnSpline(index * 1.0f / cacheImpl.Count).V3(),
            cacheImpl.GetPositionOnSpline((index + 1) * 1.0f / cacheImpl.Count).V3());
        }
        return;

		switch( interpolationMode )
		{
		case InterpolationMode.BSpline:
		case InterpolationMode.Bezier:
                //Gizmos.color = new Color( curveColor.r, curveColor.g, curveColor.b, curveColor.a * 0.25f );
                //	Gizmos.color = new Color( .8f, 1f, .1f, curveColor.a * 0.25f );
                Gizmos.color = curveColor;

            for ( int i = 0; i < ControlNodeCount-1; i++ )
			{
				Gizmos.DrawLine( impl.GetNode( i, 0 ).position.V3(), impl.GetNode( i, 1 ).position.V3() );
			
				if( ( i % 3 == 0) && IsBezier )
					++i;
			}
			
			goto default;
			
		case InterpolationMode.Hermite:
		default:
			Gizmos.color = curveColor;
			
			for( int i = 0; i < ControlNodeCount-1; i += NodesPerSegment )
			{
				Vector3 lastPos = impl.GetPositionOnSpline( new FESpline. SegmentParameter( i, 0 ) ).V3();
				
				for( float f = (IsBezier ? 0.025f : 0.1f); f < 1.0005f; f += (IsBezier ? 0.025f : 0.1f) )
				{
					Vector3 curPos = impl.GetPositionOnSpline( new FESpline.SegmentParameter( i, f ) ).V3();
					
					Gizmos.DrawLine( lastPos, curPos );
					
					lastPos = curPos;
				}
			}
			
			break;
		}
	}
	
	float GetSizeMultiplier( FESplineNode node )
	{
		if( !Camera.current.orthographic )
		{
			Plane screen = new Plane( );
			
			float sizeMultiplier = 0f;
			
			screen.SetNormalAndPosition( Camera.current.transform.forward, Camera.current.transform.position );
			screen.Raycast( new Ray( node.position.V3(), Camera.current.transform.forward ), out sizeMultiplier );
			
			return sizeMultiplier * .0075f;
		}
	
		return Camera.current.orthographicSize * 0.01875f;
	}
}
