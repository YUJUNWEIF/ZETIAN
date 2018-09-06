
Shader "Custom/UIShader" {
	Properties{
		//材料颜色默认为黑色，可在inspector中调节
		_Color("Material Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader{

		Pass{
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		//定义顶点着色器与片段着色器入口
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 

		#include "UnityCG.cginc"
		//获取property中定义的材料颜色
		sampler2D _MainTex;
		float4 _MainTex_ST;
		uniform float4 _Color;
		
		fixed4 frag(v2f_img i) : COLOR
		{
			fixed4 bg = tex2D(_MainTex, i.uv);
			//bg.rgb = bg.rgb * (1 - output.a) + output.rgb * output.a;
			bg *= _Color;
			return bg;
		}

		ENDCG
		}
	}
}

