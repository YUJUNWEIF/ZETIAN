Shader "Custom/GrayFunc" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader{ 
		Tags{ "RenderType" = "Opaque" }
		LOD 150

		Pass{
		Cull Back
		Lighting Off
		//ZWrite Off
		ZWrite On
		Blend Off
		Fog{ Mode Off }
		ColorMask RGB
		//Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		fixed4 frag(v2f_img i) : COLOR
		{
			fixed4 output = tex2D(_MainTex, i.uv);
			output.rgb = dot(output.rgb, fixed3(.222, .707, .071));
			return output;
		}
		ENDCG
		}
	}
	Fallback "Mobile/VertexLit"
}