using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

public partial class SplineEditor : InstantInspector
{
	private SerializedProperty updateModeProp;
	private SerializedProperty interpolationModeProp;
	private SerializedProperty rotationModeProp;
	private SerializedProperty tangentModeProp;
	private SerializedProperty normalModeProp;
	private SerializedProperty accuracyProp;
	private SerializedProperty upVectorProp;
	private SerializedProperty autoCloseProp;
	
	
	private CustomArrayDrawer<SplineNode> customArrayDrawer;
	
	private static readonly string performanceInfo = 
		"Performance Hint: Accuracy values above 15 are only reasonable if the segment length betweeen two spline nodes exceeds 10^4 game units, " +
		"or if you need high accuracy in a small scale of less than 10^(-4) game units.";
	
	private static readonly string editingInfo = 
		"In order to insert spline nodes at particular positions on the curve, simply right-click " +
		"somewhere near the spline while pressing the " + (Application.platform == RuntimePlatform.OSXEditor ? "Command" : "Control") + " key.";
	
	private static readonly string multiEditingWarning = 
		"Multi-object editing is not supported for the node array. \nPlease select only one spline!";
	
	private static readonly string bezierWarning = 
		"Bezier Splines must contain a multiple of three plus one node! Only the first {0} nodes will be used as control nodes!";
	
	public void OnEnable( )
	{
        interpolationModeProp = serializedObject.FindProperty("m_interpolationMode");
        rotationModeProp = serializedObject.FindProperty("m_rotationMode");
        tangentModeProp = serializedObject.FindProperty("m_tangentMode");
        accuracyProp = serializedObject.FindProperty("m_interpolationAccuracy");
        upVectorProp = serializedObject.FindProperty("m_normal");
        autoCloseProp = serializedObject.FindProperty("m_autoClose");
        normalModeProp = serializedObject.FindProperty("m_normalMode");

        customArrayDrawer = new CustomArrayDrawer<SplineNode>( this, OnInspectorChanged, target as Spline, (target as Spline).splineNodesArray, "Spline Nodes" ); 
	}
	
	public override void OnInspectorGUIInner( )
    {
        var targetSpline = target as Spline;
        DrawSplineSettings( );
		DrawSplineNodeArray(targetSpline);
	}
	
	private void DrawSplineSettings( )
	{
		EditorGUILayout.PrefixLabel( "Spline Settings", EditorStyles.label, EditorStyles.boldLabel );
		
		++EditorGUI.indentLevel;
		
		EditorGUILayout.PropertyField( interpolationModeProp, new GUIContent( "Spline Type" ) );
		EditorGUILayout.PropertyField( rotationModeProp, new GUIContent( "Rotation Mode" ) );
		
		if( (InterpolationMode) interpolationModeProp.enumValueIndex == InterpolationMode.Hermite )
		{
			EditorGUILayout.PrefixLabel( new GUIContent( "Hermite Settings" ), EditorStyles.label, EditorStyles.boldLabel );
			
			++EditorGUI.indentLevel;
			
			EditorGUILayout.PropertyField( tangentModeProp, new GUIContent( "Tangent Mode" ) );
						
			--EditorGUI.indentLevel;
			
			SmallSpace( );
		}
		
		if( (RotationMode) rotationModeProp.enumValueIndex == RotationMode.Tangent ) 
		{
			EditorGUILayout.PrefixLabel( new GUIContent( "Rotation Options" ), EditorStyles.label, EditorStyles.boldLabel );
			GUILayout.Space(-5);
			EditorGUILayout.PrefixLabel( new GUIContent( "(Tangent-Rotation Mode)" ), EditorStyles.miniLabel, EditorStyles.miniLabel );
			
			++EditorGUI.indentLevel;
			
			EditorGUILayout.PropertyField( normalModeProp, new GUIContent( "Normal Mode" ) );
			EditorGUILayout.PropertyField( upVectorProp, new GUIContent( "Up-Vector (Normal)" ), true );
			
			--EditorGUI.indentLevel;
			
			SmallSpace( );
		}
		
		EditorGUILayout.IntSlider( accuracyProp, 1, 30, new GUIContent( "Calc. Accuracy" ) );
		
		if( accuracyProp.intValue > 15 )
			EditorGUILayout.HelpBox( performanceInfo, MessageType.Info );
		
		if( (InterpolationMode) interpolationModeProp.enumValueIndex != InterpolationMode.Bezier )
			EditorGUILayout.PropertyField( autoCloseProp, new GUIContent( "Auto Close" ), true );
		
		--EditorGUI.indentLevel;

		SmallSpace();
	}
	
	private void DrawSplineNodeArray( Spline currentSpline )
	{
		if( targets.Length > 1 )
		{
			EditorGUILayout.Space( );
			EditorGUILayout.HelpBox( multiEditingWarning, MessageType.Warning );
			EditorGUILayout.Space( );
			
			return;
		}
		
		customArrayDrawer.DrawArray( );
		
		if( currentSpline.interpolationMode == InterpolationMode.Bezier )
		{
			int nodeCount = currentSpline.splineNodesArray.Count;
			int unUsedNodes = (nodeCount - 1) % 3;
			
			if( currentSpline.splineNodesArray.Count > 3 )
				if( unUsedNodes != 0 )
					EditorGUILayout.HelpBox( bezierWarning.Replace( "{0}", (nodeCount-unUsedNodes).ToString( ) ), MessageType.Warning );
		}
		
		EditorGUILayout.HelpBox( editingInfo, MessageType.Info );
	}
	
	public override void OnInspectorChanged( )
	{
		foreach( Object targetObject in serializedObject.targetObjects )
			ApplyChangesToTarget( targetObject );
		
		SceneView.RepaintAll( );
	}
	
	public void ApplyChangesToTarget( Object targetObject )
	{
		Spline spline = targetObject as Spline;

        //spline.Start();
        spline.UpdateSpline( );
		
		SplineMesh splineMesh = spline.GetComponent<SplineMesh>( );
		
		if( splineMesh != null )
			splineMesh.UpdateMesh( );
	}
}
