// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Custom/UV" {
	Properties{
		//������ɫĬ��Ϊ��ɫ������inspector�е���
		_Color("Material Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ColorUV("UV Color", Color) = (1,1,1,1)
		_UVTex("UV (RGB)", 2D) = "white" {}

	}

		SubShader{
		Tags{ "Queue" = "Geometry" }
		Pass{
		//��һ�����ع����ڵķ���ͨ�����ΪForwardBase
		Tags{ "LightMode" = "ForwardBase" }
		LOD 100
		Lighting Off Cull Back
		AlphaTest Off
		ZWrite On

		CGPROGRAM
		//���嶥����ɫ����Ƭ����ɫ�����
#pragma vertex vertForwardBase 
#pragma fragment fragForwardBase

#include "UnityCG.cginc"
		//��ȡproperty�ж���Ĳ�����ɫ
		sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _UVTex;
	float4 _UVTex_ST;
	uniform float4 _Color;
	uniform float4 _ColorUV;


	struct VertexInput
	{
		float4 vertex	: POSITION;
		half3 normal	: NORMAL;
		float2 uv0		: TEXCOORD0;
		float2 uv1		: TEXCOORD1;
	};
	struct VertexOutputForwardBase
	{
		float4 pos							: SV_POSITION;
		float2 tex							: TEXCOORD0;
		half3 uv1							: TEXCOORD1;
		//half3 normal 						: TEXCOORD3;
		UNITY_FOG_COORDS(7)
	};

	VertexOutputForwardBase vertForwardBase(VertexInput v)
	{
		VertexOutputForwardBase o;

		float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.pos = UnityObjectToClipPos(v.vertex);
		o.tex = TRANSFORM_TEX(v.uv0, _MainTex);
		o.uv1.xy = TRANSFORM_TEX(v.uv1 - 0.1 * _Time.xy, _UVTex);

		//o.eyeVec = normalize(-posWorld.xyz + _WorldSpaceCameraPos);
		float3 normalWorld = UnityObjectToWorldNormal(v.normal);
		o.uv1.z = normalWorld.y;
		//o.normal = normalWorld;
		//if (normalWorld.y > 0)
		//{
		//	o.uv1 = TRANSFORM_TEX(v.uv1 - 0.15 * _Time.xy , _UVTex);

		//}
		//else {
		//	o.uv1 = float2(0, 0);
		//}

		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}

	half4 fragForwardBase(VertexOutputForwardBase i) : SV_Target
	{
		fixed4 dd = _Color * tex2D(_MainTex, i.tex);
		if (i.uv1.z > 0) {
			fixed4 cc = tex2D(_UVTex, i.uv1)*_ColorUV;
			dd.rgb += cc.a * cc.rgb;
		}
	return  dd;
	}

		ENDCG
	}
	}
}

