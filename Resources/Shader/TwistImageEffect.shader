// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TwistImageEffect" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
		_Angle("Angle", Float) = 75//光带角度
		_XLength("XLength", Float) = 0.25//光带的x长度
		_Interval("Interval", Float) = 2  //光带循环时间
		_TorAngle("TorAngle", Float) = 75//光带角度
    }
    SubShader
    {
		Tags 
		{
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}
		Blend SrcAlpha OneMinusSrcAlpha 
        AlphaTest Greater 0.1

        pass
        {
            CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
         
			uniform sampler2D _MainTex;
			uniform float _Angle;
			uniform float _XLength;
			uniform float _Interval;
			uniform float _TorAngle;

			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};   

			v2f vert( appdata_img v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord - float2(0.5f, 0.5f);
				return o;
			}

            float4 frag (v2f_img i) : COLOR
            {
				int round = _Time.y/_Interval;               
                float x0 = (_Time.y - round * _Interval)/_Interval;
                float2 center = float2(x0 + (i.uv.y)/tan(0.0174444 * _Angle), i.uv.y);
				float rate= (_XLength -2*abs(i.uv.x - center.x))/ (_XLength);
				
				float2 offset = i.uv;
				if (rate > 0)
				{
					float2 tor;
					sincos(_TorAngle * 0.0174444, tor.y, tor.x);
					
					float2 distortedOffset = (offset.xy - center) * tor + center;
					//offset = lerp(distortedOffset, offset, 0);
					offset = lerp(offset, distortedOffset, rate);
					//offset = (offset.xy - center) * tor * rate + center;
				}
				offset += float2(0.5f, 0.5f);
				return tex2D(_MainTex, offset);
            }
            ENDCG
        }
    }
}