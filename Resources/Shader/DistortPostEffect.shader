Shader "Custom/DistortPostEffect"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NoiseTex("Noise (RGB)", 2D) = "black" {}//Ĭ�ϸ���ɫ��Ҳ���ǲ���ƫ��  
		_TimeFactor("TimeFactor", float ) = 0.15
		_DistortStrength("DistortStrength", float) = 0.01
	}

		CGINCLUDE
#include "UnityCG.cginc"  
		uniform sampler2D _MainTex;
	uniform sampler2D _NoiseTex;
	uniform float _DistortTimeFactor;
	uniform float _DistortStrength;

	fixed4 frag(v2f_img i) : SV_Target
	{
		//����ʱ��ı��������ͼ�����������  
		float4 noise = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
		//����������*����ϵ���õ�ƫ��ֵ  
		float2 offset = noise.xy * _DistortStrength;
		//���ز���ʱƫ��offset  
		float2 uv = offset + i.uv;
		return tex2D(_MainTex, uv);
	}

		ENDCG

		SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Fog{ Mode off }

			CGPROGRAM
#pragma vertex vert_img  
#pragma fragment frag  
#pragma fragmentoption ARB_precision_hint_fastest   
			ENDCG
		}
	}
	Fallback off
}