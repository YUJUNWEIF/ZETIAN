// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GodRayOptimize"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}

	ScreenLightPos("ScreenLightPos", Vector) = (0,0,0,0)
		Density("Density", Float) = 0.01
		Decay("Decay", Float) = 0.5
		Exposure("Exposure", Float) = 0.5
	}

		CGINCLUDE

#include "UnityCG.cginc"

		struct v2in
	{
		float4 vertex : POSITION;
		float2 texcoord  : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float2 uv0  : TEXCOORD0;
		float2 uv1  : TEXCOORD1;
		float2 uv2  : TEXCOORD2;
		float2 uv3  : TEXCOORD3;
		float2 uv4  : TEXCOORD4;
		float2 uv5  : TEXCOORD5;
		float2 uv6  : TEXCOORD6;
		float2 uv7  : TEXCOORD7;
	};

	sampler2D _MainTex;

	uniform float4 ScreenLightPos;
	uniform float Density;
	uniform float Decay;
	uniform float Exposure;

	v2f vert(v2in v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		half2 texCoord = v.texcoord;
		half2 deltaTexCoord = texCoord - ScreenLightPos.xy;
		deltaTexCoord *= 1.0f / 8 * Density;

		o.uv0 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv1 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv2 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv3 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv4 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv5 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv6 = texCoord;
		texCoord -= deltaTexCoord;
		o.uv7 = texCoord;
		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half illuminationDecay = 1.0f;
	half4 color = tex2D(_MainTex, i.uv0)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv1)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv2)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv3)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv4)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv5)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv6)*illuminationDecay;
	illuminationDecay *= Decay;
	color += tex2D(_MainTex, i.uv7)*illuminationDecay;

	color /= 8;

	return half4(color.xyz * Exposure, 1);
	}

		ENDCG

		Subshader
	{
		Tags{ "Queue" = "Transparent" }

			Pass
		{
			ZWrite Off

			BindChannels
		{
			Bind "Vertex", vertex
			Bind "texcoord", texcoord0
			Bind "texcoord1", texcoord1
		}

			Fog{ Mode off }

			CGPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest 
#pragma vertex vert
#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}