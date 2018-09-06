// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SwirlShader" {
    Properties {
        _MainTex ("贴图", 2D) = "white" {}
        _Cutout("透明剔除值",Float) = 0.5
        _SwirlArea("区域", Vector) = (100,100,0,0)
        _SwirlCenterXPercent("扭曲中心X偏移比例", Range(0,1)) = 0.5
        _SwirlCenterYPercent("扭曲中心Y偏移比例", Range(0,1)) = 0.5
        _Radius("扭曲半径", Float) = 5
        _SwirlValue("扭曲程度", Float) = 1
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MainTex;
            fixed4 _SwirlArea;
            fixed _SwirlCenterXPercent;
            fixed _SwirlCenterYPercent;
            fixed _Radius;
            fixed _SwirlValue;
            fixed _Cutout;
            struct vin_vct 
            {
                fixed4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };
            struct v2f_vct
            {
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };
            v2f_vct vert(vin_vct v)
            {
                v2f_vct o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;
                return o;
            }
            fixed4 frag(v2f_vct i) : SV_Target 
            {
                //得到想要扭曲的区域大小
                fixed2 swirlArea = fixed2(_SwirlArea.x, _SwirlArea.y);
                //计算扭曲区域的中心点位置
                fixed2 center = fixed2(_SwirlArea.x * _SwirlCenterXPercent, _SwirlArea.y * _SwirlCenterYPercent);
                //计算得到要扭曲的片段距离中心点的距离
                fixed2 targetPlace = i.texcoord * swirlArea;
                targetPlace -= center;
                fixed dis = length(targetPlace);
                //当距离比需要扭曲的半径小的时候，计算扭曲
                if(dis < _Radius)
                {
                    //计算扭曲的程度,慢慢向边缘减弱扭曲,当它恒为1的时候，是一个正圆
                    fixed percent = (_Radius - dis) / _Radius;
                    //根据扭曲程度计算因子，还可以改写为_Time的值让它一直转
                    fixed delta = percent * _SwirlValue;
                    //得到在轴上的值
                    fixed s = sin(delta);
                    fixed c = cos(delta);
                    targetPlace = fixed2(dot(targetPlace, fixed2(c, -s)), dot(targetPlace, fixed2(s, c)));
                }
                //把计算的值偏移回中心点
                targetPlace += center;
                fixed4 c = tex2D(_MainTex, targetPlace/swirlArea) * i.color;
                clip(c.a - _Cutout);
                return c;
            }
           ENDCG
        }
    }
    FallBack "Diffuse"
}