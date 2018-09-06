Shader "Custom/FlashShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
		_Angle("Angle", Float) = 75//光带角度
		_XLength("XLength", Float) = 0.25//光带的x长度
		_Interval("Interval", Float) = 2  //光带播放间隔
		_BeginTime("BeginTime", Float) = 1//光带开始时间
		_LoopTime("LoopTime", Float) = 0.5//光带单次循环时间
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
			#pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
       
            sampler2D _MainTex;
			uniform float _Angle;
			uniform float _XLength;
			uniform float _Interval;
			uniform float _BeginTime;
			uniform float _LoopTime;
            float4 _MainTex_ST;
			                    
            //必须放在使用其的 frag函数之前，否则无法识别。
            float inFlash(float2 uv)
            {
                //亮度值
                float brightness =0;               
               
                //获取本次光照的起始时间
                int round = _Time.y/_Interval;
               
                //获取本次光照的流逝时间 = 当前时间 - 起始时间
                float currentTimePassed = _Time.y - round * _Interval;
                if(currentTimePassed >_BeginTime)
                {
                    float x0 = (currentTimePassed-_BeginTime)/_LoopTime;

					//设置底部边界
					float2 xBottomBounds = float2(x0 - _XLength * 0.5f, x0 + _XLength * 0.5f);
                   
                    //投影至x的长度 = y/ tan(_Angle)
                    float xProjL = (uv.y)/tan(0.0174444 * _Angle);
					xBottomBounds += float2(xProjL, xProjL);
                   
                    //如果该点在区域内
                    if(uv.x > xBottomBounds.x && uv.x < xBottomBounds.y)
                    {
                        //得到发光区域的中心点
                        float midness = (xBottomBounds.x + xBottomBounds.y) * 0.5f;
                       
                        //趋近中心点的程度，0表示位于边缘，1表示位于中心点
                        float rate= (_XLength -2*abs(uv.x - midness))/ (_XLength);
                        brightness = rate;
                    }
                }
                return max(brightness,0);
            }
           
            float4 frag (v2f_img i) : COLOR
            {
                float4 outp;
                
                //根据uv取得纹理颜色，和常规一样
                float4 texCol = tex2D(_MainTex,i.uv);
       
                //传进i.uv等参数，得到亮度值
                float tmpBrightness = inFlash(i.uv);
           
                //图像区域，判定设置为 颜色的A > 0.5,输出为材质颜色+光亮值
                if(texCol.w >0.5)
                        outp = texCol+float4(1,1,1,1)*tmpBrightness;
                //空白区域，判定设置为 颜色的A <=0.5,输出空白
                else
                    outp =float4(0,0,0,0);

                return outp;
            }
            ENDCG
        }
    }
}