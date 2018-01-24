// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|alpha-5449-OUT,clip-6812-A,refract-8872-OUT;n:type:ShaderForge.SFN_Fresnel,id:9011,x:31703,y:33136,varname:node_9011,prsc:2|EXP-6417-OUT;n:type:ShaderForge.SFN_Vector1,id:6417,x:31570,y:33288,varname:node_6417,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:1584,x:32164,y:33088,varname:node_1584,prsc:2|A-3924-OUT,B-5259-OUT;n:type:ShaderForge.SFN_Append,id:8872,x:32382,y:33045,varname:node_8872,prsc:2|A-6039-OUT,B-1584-OUT;n:type:ShaderForge.SFN_Tex2d,id:6812,x:31917,y:32792,ptovrint:False,ptlb:Shape,ptin:_Shape,varname:node_6812,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0000000000000000f000000000000000,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:6039,x:32174,y:32955,varname:node_6039,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:3924,x:31637,y:32852,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_3924,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector1,id:5449,x:32455,y:32853,varname:node_5449,prsc:2,v1:0;n:type:ShaderForge.SFN_Lerp,id:5259,x:32027,y:33208,varname:node_5259,prsc:2|A-3019-OUT,B-9011-OUT,T-3754-OUT;n:type:ShaderForge.SFN_Vector1,id:3019,x:31827,y:33262,varname:node_3019,prsc:2,v1:0;n:type:ShaderForge.SFN_VertexColor,id:2390,x:31715,y:32978,varname:node_2390,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3754,x:31875,y:33016,varname:node_3754,prsc:2|A-6812-A,B-2390-A;proporder:6812-3924;pass:END;sub:END;*/

Shader "Shader Forge/AirCompression" {
    Properties {
        _Shape ("Shape", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Shape; uniform float4 _Shape_ST;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_9011 = pow(1.0-max(0,dot(normalDirection, viewDirection)),0.3);
                float4 _Shape_var = tex2D(_Shape,TRANSFORM_TEX(i.uv0, _Shape));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + float2(0.0,(_Intensity*lerp(0.0,node_9011,(_Shape_var.a*i.vertexColor.a))));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                clip(_Shape_var.a - 0.5);
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
