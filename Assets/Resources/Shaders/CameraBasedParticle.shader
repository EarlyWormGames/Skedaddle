// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32235,y:32601,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32495,y:32793,varname:node_2393,prsc:2|A-6074-RGB,B-4935-OUT,C-797-RGB,D-9248-OUT,E-9168-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31950,y:32835,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31950,y:32692,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32426,y:33320,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:6774,x:31662,y:33033,ptovrint:False,ptlb:DistanceThreshold,ptin:_DistanceThreshold,varname:node_6774,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;n:type:ShaderForge.SFN_Subtract,id:174,x:31881,y:33114,varname:node_174,prsc:2|A-6774-OUT,B-9065-OUT;n:type:ShaderForge.SFN_Clamp01,id:9168,x:32065,y:33114,varname:node_9168,prsc:2|IN-174-OUT;n:type:ShaderForge.SFN_Code,id:1300,x:30936,y:33342,varname:node_1300,prsc:2,code:cgBlAHQAdQByAG4AIABfAFcAbwByAGwAZABTAHAAYQBjAGUAQwBhAG0AZQByAGEAUABvAHMAOwAKAA==,output:2,fname:CameraPosition,width:249,height:132;n:type:ShaderForge.SFN_FragmentPosition,id:8109,x:31160,y:33122,varname:node_8109,prsc:2;n:type:ShaderForge.SFN_Distance,id:9065,x:31583,y:33213,varname:node_9065,prsc:2|A-8109-XYZ,B-1300-OUT;n:type:ShaderForge.SFN_Multiply,id:4935,x:32189,y:32864,varname:node_4935,prsc:2|A-2053-RGB,B-2053-A;proporder:6074-797-6774;pass:END;sub:END;*/

Shader "Shader Forge/CameraBasedParticle" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _DistanceThreshold ("DistanceThreshold", Float ) = 1.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _DistanceThreshold;
            float3 CameraPosition(){
            return _WorldSpaceCameraPos;
            
            }
            
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_MainTex_var.rgb*(i.vertexColor.rgb*i.vertexColor.a)*_TintColor.rgb*2.0*saturate((_DistanceThreshold-distance(i.posWorld.rgb,CameraPosition()))));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
