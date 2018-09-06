// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Guide" 
{
	Properties
	{
		_MainTex("Texture (RGB)", 2D) = "black" {}
		_Color("Color", Color) = (0, 0, 0, 1)
		_ShapeType("ShapeType", int) = 1
		_ShapePos("ShapePos", Vector) = (1, 1, 1, 1)
		_ShapeSize("ShapeSize", Vector) = (1, 1, 1, 1)
	}


	SubShader
	{			
			Tags {
// 			"RenderType"="Transparent"
// 			"Queue"="Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}
		
		Pass
		{
			//Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }
			ColorMask RGB			
			Cull Off Lighting Off ZWrite Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float4 _Color;
				uniform int _ShapeType;
				uniform float4 _ShapePos;
				uniform float4 _ShapeSize;

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 texcoord : TEXCOORD2;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				float4 frag(v2f i) : COLOR
				{
					float4 color = tex2D(_MainTex, i.texcoord)*_Color;
					if (_ShapeType == 1)//this is rect
					{
						float2 offset = i.texcoord - _ShapePos.xy;						
						if( offset.x > -_ShapeSize.x && offset.x < _ShapeSize.x && 
							offset.y > -_ShapeSize.y && offset.y < _ShapeSize.y)
						{
							color.a = 0;
						}
						offset = i.texcoord - _ShapePos.zw;
						if( offset.x > -_ShapeSize.z && offset.x < _ShapeSize.z && 
							offset.y > -_ShapeSize.w && offset.y < _ShapeSize.w)
						{
							color.a = 0;
						}
					}
					else //this is circle
					{
						float2 offset = i.texcoord - _ShapePos.xy;						
						if( offset.x * offset.x + offset.y * offset.y < _ShapeSize.x * _ShapeSize.x)
						{
							color.a = 0;
						}
						offset = i.texcoord - _ShapePos.zw;
						if( offset.x * offset.x + offset.y * offset.y < _ShapeSize.z * _ShapeSize.z)
						{
							color.a = 0;
						}
					} 
					return color;
				}
			ENDCG
		}		
	}
	FallBack "Diffuse"
}
