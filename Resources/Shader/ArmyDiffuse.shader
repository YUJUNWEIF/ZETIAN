// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Custom/ArmyDiffuse" {
	Properties{
		//材料颜色默认为黑色，可在inspector中调节
		_Color("Material Color", Color) = (1,1,1,1)
		_SpecColor("Spec Color", Color) = (1,1,1,1)
		_EmitColor("Emit Color", Color) = (0.2,0.2,0.2,0.2)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Gloss("Gloss", float) = 0.5
		_Ns("Ns", float) = 0.5
	}

	SubShader{
			Tags{ "Queue" = "Geometry" }
		Pass{
		//第一个像素光所在的反射通道标记为ForwardBase
			Tags{ "LightMode" = "ForwardBase" }
		LOD 100
		Lighting Off Cull Back
		AlphaTest Off
			ZWrite On

		CGPROGRAM
		//定义顶点着色器与片段着色器入口
		#pragma vertex vertForwardBase 
		#pragma fragment fragForwardBase

		#include "UnityCG.cginc"
		//获取property中定义的材料颜色
		sampler2D _MainTex;
		float4 _MainTex_ST;
		uniform float4 _Color;
		uniform float4 _EmitColor;
		uniform float4 _SpecColor;
		uniform float _Gloss;
		uniform float _Ns;
		uniform float4 _LightColor0;
		
		struct VertexInput
		{
			float4 vertex	: POSITION;
			half3 normal	: NORMAL;
			float2 uv0		: TEXCOORD0;
		};
		struct VertexOutputForwardBase
		{
			float4 pos							: SV_POSITION;
			float2 tex							: TEXCOORD0;
			half3 eyeVec 						: TEXCOORD1;
			half3 normal 						: TEXCOORD2;
			UNITY_FOG_COORDS(7)
		};

		VertexOutputForwardBase vertForwardBase(VertexInput v)
		{
			VertexOutputForwardBase o;

			float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.tex = TRANSFORM_TEX(v.uv0, _MainTex);
			o.eyeVec = normalize(-posWorld.xyz + _WorldSpaceCameraPos);
			float3 normalWorld = UnityObjectToWorldNormal(v.normal);
			o.normal = normalWorld;

			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		half4 fragForwardBase(VertexOutputForwardBase i) : SV_Target
		{
			half atten = 1;


			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
			float3 viewDir = i.eyeVec;

			half3 h = normalize(lightDir + viewDir);
			fixed diff = max(0, dot(i.normal, lightDir));
			float nh = max(0, dot(i.normal, h));
			float spec = pow(nh, _Ns*128.0) * _Gloss;

			fixed4 c;
			c.a = _Color.a;
			c.rgb = _Color.rgb * ( (_LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec ) * atten + _EmitColor.rgb);

			return c* tex2D(_MainTex, i.tex);
		}

		ENDCG
		}
	}
}

