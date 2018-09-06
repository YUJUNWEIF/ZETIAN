// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.05 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.05;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:False,dith:0,ufog:True,aust:False,igpj:True,qofs:0,qpre:4,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9063,x:33807,y:32831,varname:node_9063,prsc:2|normal-7204-OUT,emission-7110-OUT,alpha-2816-OUT,refract-3947-OUT;n:type:ShaderForge.SFN_Tex2d,id:850,x:31896,y:32714,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_850,prsc:2,tex:2b200e7fec2ce8c40abd7bbc9560e4b0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_If,id:459,x:31948,y:33010,varname:node_459,prsc:2|A-1342-OUT,B-5856-R,GT-1623-OUT,EQ-1623-OUT,LT-7299-OUT;n:type:ShaderForge.SFN_Tex2d,id:5856,x:31365,y:33072,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_5856,prsc:2,tex:44860c2146229d548ae688b9f9cbb1ae,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:1623,x:31365,y:33240,varname:node_1623,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7299,x:31365,y:33302,varname:node_7299,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:8707,x:32842,y:32939,varname:node_8707,prsc:2|A-850-A,B-4959-OUT;n:type:ShaderForge.SFN_VertexColor,id:8858,x:31192,y:32775,varname:node_8858,prsc:2;n:type:ShaderForge.SFN_If,id:2124,x:31948,y:33226,varname:node_2124,prsc:2|A-8858-R,B-5856-R,GT-1623-OUT,EQ-1623-OUT,LT-7299-OUT;n:type:ShaderForge.SFN_Add,id:1342,x:31681,y:32714,varname:node_1342,prsc:2|A-2095-OUT,B-8858-R;n:type:ShaderForge.SFN_ValueProperty,id:2095,x:31508,y:32677,ptovrint:False,ptlb:0_勾边大小,ptin:_0_,varname:node_2095,prsc:2,glob:False,v1:0.1;n:type:ShaderForge.SFN_Subtract,id:3805,x:32223,y:33170,varname:node_3805,prsc:2|A-459-OUT,B-2124-OUT;n:type:ShaderForge.SFN_Multiply,id:3683,x:32450,y:33197,varname:node_3683,prsc:2|A-3805-OUT,B-7276-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7276,x:32196,y:33419,ptovrint:False,ptlb:1_勾边亮度,ptin:_1_,varname:node_7276,prsc:2,glob:False,v1:100;n:type:ShaderForge.SFN_Add,id:4959,x:32641,y:33076,varname:node_4959,prsc:2|A-459-OUT,B-3683-OUT;n:type:ShaderForge.SFN_Multiply,id:378,x:32169,y:32597,varname:node_378,prsc:2|A-5389-RGB,B-850-RGB;n:type:ShaderForge.SFN_Multiply,id:779,x:32395,y:32536,varname:node_779,prsc:2|A-7493-OUT,B-378-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7493,x:32169,y:32478,ptovrint:False,ptlb:2_diffuse强度,ptin:_2_diffuse,varname:node_7493,prsc:2,glob:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:7110,x:32628,y:32617,varname:node_7110,prsc:2|A-779-OUT,B-850-R;n:type:ShaderForge.SFN_Multiply,id:2816,x:33140,y:33018,varname:node_2816,prsc:2|A-8858-A,B-8707-OUT;n:type:ShaderForge.SFN_Color,id:5389,x:31709,y:32324,ptovrint:False,ptlb:3_color,ptin:_3_color,varname:node_5389,prsc:2,glob:False,c1:0.8,c2:0.3,c3:0.1,c4:1;n:type:ShaderForge.SFN_Lerp,id:7204,x:33066,y:33362,varname:node_7204,prsc:2|A-3224-OUT,B-850-A,T-2822-OUT;n:type:ShaderForge.SFN_Vector3,id:3224,x:32812,y:33251,varname:node_3224,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:2822,x:32373,y:33697,ptovrint:False,ptlb:4_扭曲强度,ptin:_4_,varname:node_2822,prsc:2,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Multiply,id:1502,x:32979,y:33797,varname:node_1502,prsc:2|A-2822-OUT,B-7199-OUT;n:type:ShaderForge.SFN_Vector1,id:7199,x:32703,y:33887,varname:node_7199,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:3947,x:33201,y:33690,varname:node_3947,prsc:2|A-70-OUT,B-1502-OUT;n:type:ShaderForge.SFN_Multiply,id:70,x:32979,y:33587,varname:node_70,prsc:2|A-4724-OUT,B-8858-A;n:type:ShaderForge.SFN_ComponentMask,id:4724,x:32699,y:33422,varname:node_4724,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-850-RGB;proporder:850-5856-2095-7276-7493-5389-2822;pass:END;sub:END;*/

Shader "Custom/daoguang_rongjie" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _0_ ("0_勾边大小", Float ) = 0.1
        _1_ ("1_勾边亮度", Float ) = 100
        _2_diffuse ("2_diffuse强度", Float ) = 10
        _3_color ("3_color", Color) = (0.8,0.3,0.1,1)
        _4_ ("4_扭曲强度", Range(0, 1)) = 0.2
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _0_;
            uniform float _1_;
            uniform float _2_diffuse;
            uniform float4 _3_color;
            uniform float _4_;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.pos;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((_Diffuse_var.rgb.rg*i.vertexColor.a)*(_4_*0.1));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = lerp(float3(0,0,1),float3(_Diffuse_var.a,_Diffuse_var.a,_Diffuse_var.a),_4_);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
////// Lighting:
////// Emissive:
                float3 emissive = ((_2_diffuse*(_3_color.rgb*_Diffuse_var.rgb))*_Diffuse_var.r);
                float3 finalColor = emissive;
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float node_459_if_leA = step((_0_+i.vertexColor.r),_Noise_var.r);
                float node_459_if_leB = step(_Noise_var.r,(_0_+i.vertexColor.r));
                float node_7299 = 0.0;
                float node_1623 = 1.0;
                float node_459 = lerp((node_459_if_leA*node_7299)+(node_459_if_leB*node_1623),node_1623,node_459_if_leA*node_459_if_leB);
                float node_2124_if_leA = step(i.vertexColor.r,_Noise_var.r);
                float node_2124_if_leB = step(_Noise_var.r,i.vertexColor.r);
                return fixed4(lerp(sceneColor.rgb, finalColor,(i.vertexColor.a*(_Diffuse_var.a*(node_459+((node_459-lerp((node_2124_if_leA*node_7299)+(node_2124_if_leB*node_1623),node_1623,node_2124_if_leA*node_2124_if_leB))*_1_))))),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
