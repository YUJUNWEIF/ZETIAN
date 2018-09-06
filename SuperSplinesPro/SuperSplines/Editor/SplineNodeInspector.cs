using System.Collections;

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SplineNode))]
public class SplineNodeInspector : InstantInspector
{
	//private SerializedProperty customValueProp;
	//private SerializedProperty normalProp;
	
	private GUIStyle buttonGUIStyle;
	
	private SplineNode targetNode;
	
	private static FESpline selectedSpline = null;
	private static LengthMode lMode = LengthMode.GameUnits;
	
	private readonly string notUsedWarning = 
		"This SplineNode isn't used by any spline in the scene. Attach this node to a spline by dragging it onto a spline's Inspector window!";
	
	public void OnEnable( )
	{
		//customValueProp = serializedObject.FindProperty( "customValue" );
		//normalProp = serializedObject.FindProperty( "normal" );
	}
		
	public override void OnInspectorGUIInner( )
	{
		targetNode = target as SplineNode;
		
		List<FESpline> splinesToRemove = new List<FESpline>( );
			
		foreach( var key in targetNode.impl.Parameters.Keys )
		{
			if( key == null)
				splinesToRemove.Add( key );
			else if( !key.splineNodesArray.Contains( targetNode.impl ) )
				splinesToRemove.Add( key );
		}
		
		foreach( var key in splinesToRemove )
			targetNode.impl.Parameters.Remove( key );
		
		DrawInspectorOptions( );
		
		DrawSplineSettings( );
		
		DrawCustomSettings( );
		
		DrawButtons( );
	}
	
	private void DrawInspectorOptions( )
	{
		EditorGUILayout.PrefixLabel( "Inspector Options", EditorStyles.label, EditorStyles.boldLabel );
		
		++EditorGUI.indentLevel;
		
		lMode = (LengthMode) EditorGUILayout.EnumPopup( "Length Mode", lMode );
		SmallSpace( );
		
		--EditorGUI.indentLevel;
	}
	
	private void DrawSplineSettings( )
	{
		EditorGUILayout.PrefixLabel( "Spline Data", EditorStyles.label, EditorStyles.boldLabel );
		
		++EditorGUI.indentLevel;
		
		if( targetNode.impl.Parameters.Count <= 0 )
		{
			EditorGUILayout.HelpBox( notUsedWarning, MessageType.Info );
		}
		else
		{
			List<FESpline> splineKeys = new List<FESpline>( targetNode.impl.Parameters.Keys );
			List<string> splineNames = new List<string>( );
			
			foreach( var no in splineKeys )
			{
                var spline = no.unityUse as Spline;
                if ( !splineNames.Contains( spline.name ) )
					splineNames.Add( spline.name );
				else
				{
					string newName = spline.name;
					
					while( splineNames.Contains( newName ) )
						newName += "*";
					
					splineNames.Add( newName );
				}
			}
			
			int index = splineKeys.IndexOf( selectedSpline );
			
			if( index < 0 )
				index = 0;
			
			index = EditorGUILayout.Popup( "Spline", index, splineNames.ToArray( ) );
			
			selectedSpline = splineKeys[index];
			
			float lengthFactor = (lMode != LengthMode.GameUnits) ? 1 : selectedSpline.Length;
			
			float position = (float)targetNode.impl.Parameters[selectedSpline].position * lengthFactor;
			float length = (float)targetNode.impl.Parameters[selectedSpline].length * lengthFactor;
			SmallSpace( );
			
			int nodeIndex = selectedSpline.SplineNodes.IndexOf(targetNode.impl );
			
			EditorGUILayout.TextField( "Index in Spline", nodeIndex.ToString( ) );
			SmallSpace( );
			
			EditorGUILayout.TextField( "Spline Parameter", position.ToString( ) );
			GUILayout.Space(-5);
			EditorGUIUtility.LookLikeControls( 200 );
			EditorGUILayout.PrefixLabel( new GUIContent( "(Distance From Start Node)" ), EditorStyles.miniLabel, EditorStyles.miniLabel );
			EditorGUIUtility.LookLikeControls( );
			
			EditorGUILayout.TextField( "Length Parameter", length.ToString( ) );
			GUILayout.Space(-5);
			EditorGUIUtility.LookLikeControls( 200 );
			EditorGUILayout.PrefixLabel( new GUIContent( "(Distance To Next Node)" ), EditorStyles.miniLabel, EditorStyles.miniLabel );
			EditorGUIUtility.LookLikeControls( );
		}
		
		--EditorGUI.indentLevel;
	}
	
	private void DrawCustomSettings( )
	{
		EditorGUILayout.PrefixLabel( "Custom Settings", EditorStyles.label, EditorStyles.boldLabel );
		
		++EditorGUI.indentLevel;

        var customNormal = EditorGUILayout.Vector3Field("Curve Normal", targetNode.CustomNormal);
        if (customNormal != targetNode.CustomNormal) { targetNode.CustomNormal = customNormal; }
        EditorGUILayout.Space();

        var customValue = EditorGUILayout.FloatField("Curve Data", targetNode.CustomValue);
        if (Mathf.Approximately(customValue, targetNode.CustomValue)) { targetNode.CustomValue = customValue; }
        SmallSpace();

        //EditorGUILayout.PropertyField( normalProp, new GUIContent( "Curve Normal" ) );
        //EditorGUILayout.Space( );

        //EditorGUILayout.PropertyField( customValueProp, new GUIContent( "Custom Data" ) );
        //SmallSpace( );

        --EditorGUI.indentLevel;
	}
	
	private void DrawButtons( )
	{
		if( targetNode.impl.Parameters.Count <= 0 )
			return;
		
		var splineNodes = selectedSpline.SplineNodes;
		
		int nodeIndex = selectedSpline.SplineNodes.IndexOf(targetNode.impl);
		
		EditorGUI.BeginDisabledGroup( selectedSpline == null );
		
		EditorGUILayout.BeginHorizontal( );
		GUILayout.Space( 15 );
		
		if( GUILayout.Button( "Previous Node", GetButtonGUIStyleLeft( ), GUILayout.Height( 21f ) ) )
			Selection.activeGameObject = (splineNodes[ (nodeIndex!=0 ? nodeIndex : splineNodes.Count) - 1].unityUse as SplineNode).gameObject; 
		
		if( GUILayout.Button( "  Next Node	", GetButtonGUIStyleRight( ), GUILayout.Height( 21f ) ) )
			Selection.activeGameObject = (splineNodes[(nodeIndex+1)%splineNodes.Count].unityUse as SplineNode).gameObject; 
		
		EditorGUILayout.EndHorizontal( );
		
		EditorGUI.EndDisabledGroup( );
	}
	
	public void OnSceneGUI( )
	{
		if( targetNode == null )
			return;
		
		Handles.color = new Color( .3f, 1f, .20f, 1 );
        var up = targetNode.transform.TransformDirection(targetNode.CustomNormal);
        Handles.ArrowCap(0, targetNode.Position, Quaternion.LookRotation(up), HandleUtility.GetHandleSize(targetNode.Position) * 0.5f);
        //Handles.ArrowCap( 0, targetNode.Position, Quaternion.LookRotation( targetNode.TransformedNormal ), HandleUtility.GetHandleSize( targetNode.Position ) * 0.5f );

        Handles.color = new Color( .2f, 0.4f, 1f, 1 );
		Handles.ArrowCap( 0, targetNode.Position, Quaternion.LookRotation( targetNode.transform.forward ), HandleUtility.GetHandleSize( targetNode.Position ) * 0.5f );
		
		Handles.color = new Color( 1f, 0.5f, 0f, .75f );
		Handles.SphereCap( 0, targetNode.Position, targetNode.Rotation, HandleUtility.GetHandleSize( targetNode.Position ) * 0.175f );
	}
	
	private GUIStyle GetButtonGUIStyleLeft( )
	{
		GUIStyle buttonGUIStyle = new GUIStyle( EditorStyles.miniButtonLeft );
		
		buttonGUIStyle.alignment = TextAnchor.MiddleCenter;
		buttonGUIStyle.wordWrap = true;
		buttonGUIStyle.border = new RectOffset( 3, 3, 3, 3 );
		buttonGUIStyle.contentOffset = - Vector2.up * 2f;
		buttonGUIStyle.fontSize = 12;
		
		return buttonGUIStyle;
	}
	
	private GUIStyle GetButtonGUIStyleRight( )
	{
		GUIStyle buttonGUIStyle = new GUIStyle( EditorStyles.miniButtonRight );
		
		buttonGUIStyle.alignment = TextAnchor.MiddleCenter;
		buttonGUIStyle.wordWrap = true;
		buttonGUIStyle.border = new RectOffset( 3, 3, 3, 3 );
		buttonGUIStyle.contentOffset = - Vector2.up * 2f;
		buttonGUIStyle.fontSize = 12;
		
		return buttonGUIStyle;
	}
	
	private enum LengthMode
	{
		Normalized,
		GameUnits
	}
}
