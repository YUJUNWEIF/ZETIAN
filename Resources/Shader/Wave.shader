Shader "Custom/ClickStyle" {
        Properties {
            [PerRendererData]_MainTex("Sprite Texture",2D)="white"{}
            [HideInInspector]_StartTime("Start Time",Float)=0  //起始时间，这个在Ripple中设置就行
            _AnimationTime("AnimationTime",Range(0.1,10.0))=1.5 //动画时间
            _Width("Width",Range(0.1,3.0))=0.3   //内圆和外圆形成环的宽度
            _StartWidth("Start Width",Range(0,1.0))=0.3  //内圆的默认直径，注意这里是按照uv尺寸走的，最大也就是1
            [Toggle]_isAlpha("isAlpha",Float)=1
            [Toggle]_isColorShift("isColorShift",Float)=1
            [MaterialToggle]PixelSnap("Pixe Snap",Float)=1
        }
        SubShader {

            //启用透明混合，要不然没有透明效果哦
            Tags{"Queue"="Transparent" "RenderType"="Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert 
                #pragma fragment frag 
                #include "UnityCG.cginc"

                struct v2f{
                     float4 vertex:SV_POSITION;
                     float2 texcoord:TEXCOORD0;
                };
                sampler2D _MainTex;
                float _StartTime;
                float _AnimationTime;
                float _StartWidth;
                float _Width;
                float _isAlpha;
                float _isColorShift;

                v2f vert(appdata_base IN)
                {
                    v2f OUT;
                    OUT.vertex=UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord=IN.texcoord;
                    return OUT;
                }

                //这个函数是用来变换颜色的，当然你可以想怎么干就怎么干，随你洛。
                //注意这个函数写在frag函数前面，要不然不识别哦
                fixed3 shift_col(fixed3 RGB,half3 shift)
                {
                    fixed3 RESULT = fixed3(RGB);
                    float VSU = shift.z*shift.y*cos(shift.x*3.14159265 / 180);
                    float VSW = shift.z*shift.y*sin(shift.x*3.14159265 / 180);

                    RESULT.x = (.299*shift.z + .701*VSU + .168*VSW)*RGB.x
                    + (.587*shift.z - .587*VSU + .330*VSW)*RGB.y
                    + (.114*shift.z - .114*VSU - .497*VSW)*RGB.z;

                    RESULT.y = (.299*shift.z - .299*VSU - .328*VSW)*RGB.x
                    + (.587*shift.z + .413*VSU + .035*VSW)*RGB.y
                    + (.114*shift.z - .114*VSU + .292*VSW)*RGB.z;

                    RESULT.z = (.299*shift.z - .3*VSU + 1.25*VSW)*RGB.x
                    + (.587*shift.z - .588*VSU - 1.05*VSW)*RGB.y
                    + (.114*shift.z + .886*VSU - .203*VSW)*RGB.z;

                    return RESULT;
                }


                fixed4 frag(v2f IN):SV_Target{
                    fixed4 color=tex2D(_MainTex,IN.texcoord);
                    float2 pos=(IN.texcoord-float2(0.5,0.5))*2; //计算像素点到中心的距离，乘以2就当做圆的直径吧。


                 //如果从中心开始向外扩散
                //  float dis=(_Time.y-_StartTime)/_AnimationTime-length(pos);

                //如果指定内径开始向外扩散 ,在Ripple类中 已经减去了开始长度的时间。所以这里就不用再减了
                float dis=(_Time.y-_StartTime)/_AnimationTime+_StartWidth-length(pos);
                    //大于最大宽度以及小于0都去掉这部分像素

                    if(dis<0||dis>_Width)
                    {
                      return fixed4(0,0,0,0);
                    }

                    //测试用
                    //if(dis < 0 )
                    //{
                    //  return fixed4(1,0,0,1);
                    //}
                    //if(dis>_Width)
                    //{
                    //  return fixed4(0,1,0,1);
                    //}

                    //如果开启了透明度渐变就让透明度进行插值
                    float alpha=1;
                    if(_isAlpha==1)
                    {
                        alpha = clamp((_Width-dis)*3,0.1,1.5);
                    }

                    fixed3 shiftColor=color;
                    //if(_isColorShift==1)
                    //{
                    //    half3 shift=half3(_Time.w*10,1,1);
                    //    shiftColor=shift_col(color,shift);
                    //}

                    return fixed4(shiftColor,color.a*alpha);
                }


                ENDCG
            }
        }
    }