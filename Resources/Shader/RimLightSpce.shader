Shader "Custom/RimLightSpce" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		// comment out due to no lighting calculation
		//_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		//_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 2.0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_UseRim ("Use Rim", Range(0,1)) = 0.0
	}


	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
	
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;
		fixed4 _Color;

		float4 _RimColor;
		float _RimPower;
		float _UseRim;

		struct Input 
		{
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal; 
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;


			if (_UseRim > 0.0)
			{
				// comment out due to no lighting calculation
				//o.Gloss = tex.a;
				half rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
				o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			}
		}
		ENDCG
	}

	Fallback "Transparent/Cutout/VertexLit"
}
