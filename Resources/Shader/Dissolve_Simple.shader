// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Dissolve/Simplest" {
Properties {
    _TintColor ("Tint Color[_TintColor]", Color) = (1,1,1,1)
    _MainTex ("Main Texture[_MainTex]", 2D) = "white" {}
    _DissolveMap ("Dissolve Map (R)[_DissolveMap]", 2D) = "white" {}
    _DissolvePower("DissolvePower[_DissolvePower]",Range(0,0.99)) = 0
    _ColorStrength("ColorStrength[_ColorStrength]",Float) = 1
    _DissolveColor("DissolveColor[_DissolveColor]",Color) = (1,1,1,1)
    _DissolveColorStrength("DissolveColorStrength[_DissolveColorStrength]",Float) = 1
    }


 SubShader {
    Tags {"Queue"="Transparent"  "IgnoreProjector"="True" "RenderType"="Transparent"}
    Lighting Off 
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha 
     Pass {
         CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
         
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            

            uniform fixed4 _TintColor;
            uniform half _ColorStrength;

            uniform sampler2D _DissolveMap;
            uniform fixed4 _DissolveColor;
            uniform half _DissolveColorStrength; 
            uniform half _DissolvePower;
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
			    float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            }; 
         
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
			     fixed4 mainColor = tex2D(_MainTex, i.texcoord);

                fixed4 dissolveColor = tex2D(_DissolveMap,  i.texcoord);
                fixed grayscale = dissolveColor.r ;

                grayscale = grayscale.r - _DissolvePower;
                fixed4 col = fixed4(0,0,0,0);
                 if(grayscale < 0 && grayscale > -0.02)
                 {
                    float s = grayscale / -0.02;
                    col = _DissolveColor * s + mainColor * (1 - s);
                    col =  col *  mainColor.a *  _DissolveColorStrength;
                    //col =  _DissolveColor * _DissolveColorStrength *  mainColor.a ;

                }
                else if(grayscale < -0.02)
                {
                    clip(-0.1);
                    col = float4(0,0,0,0);
                }
                else
                {
                    col = i.color * _TintColor * mainColor  * _ColorStrength;
                }
                return col;
            }
            ENDCG
        }
    }
    CustomEditor "DissolveMaterialEditor"   
}