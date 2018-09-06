Shader "Custom/NightLight"
{
	Properties
	{
		_MainTex ("Texture (ARGB)", 2D) = "white" {}
		_NightTex("Texture (A)", 2D) = "white" {}
		_Tense("Light Tense", float) = 5
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert


		sampler2D _MainTex;
		sampler2D _NightTex;
		float _Tense;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			half4 alpha = tex2D(_NightTex, IN.uv_MainTex);
			o.Albedo = c.rgb +alpha.a * float3(1, 1, 1) * _Tense;
			o.Alpha = c.a;
		}
		ENDCG
		
	}
}
