// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33659,y:32839,varname:node_1873,prsc:2|emission-1749-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32008,y:32636,ptovrint:False,ptlb:Texture_NoAnimal,ptin:_Texture_NoAnimal,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1086,x:33120,y:32780,cmnt:RGB,varname:node_1086,prsc:2|A-1814-OUT,B-3160-OUT,C-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32008,y:33098,ptovrint:False,ptlb:Color_NoAnimal,ptin:_Color_NoAnimal,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32980,y:33395,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:33341,y:32780,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-603-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:33250,y:33226,cmnt:A,varname:node_603,prsc:2|A-9119-OUT,B-2243-OUT,C-5376-A;n:type:ShaderForge.SFN_Tex2d,id:2016,x:32008,y:32825,ptovrint:False,ptlb:Texture_SomeAnimals,ptin:_Texture_SomeAnimals,varname:node_2016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Color,id:5465,x:32018,y:33265,ptovrint:False,ptlb:Color_SomeAnimals,ptin:_Color_SomeAnimals,varname:node_5465,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7144,x:32528,y:32789,ptovrint:False,ptlb:Texture_AllAnimals,ptin:_Texture_AllAnimals,varname:node_7144,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Color,id:2694,x:32504,y:33175,ptovrint:False,ptlb:Color_AllAnimals,ptin:_Color_AllAnimals,varname:node_2694,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:1799,x:31940,y:33008,ptovrint:False,ptlb:SomeAnimalsAmount,ptin:_SomeAnimalsAmount,varname:node_1799,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7058,x:32263,y:33015,ptovrint:False,ptlb:AllAnimalsAmount,ptin:_AllAnimalsAmount,varname:node_7058,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:5715,x:32361,y:32709,varname:node_5715,prsc:2|A-4805-RGB,B-2016-RGB,T-1799-OUT;n:type:ShaderForge.SFN_Lerp,id:5094,x:32342,y:33117,varname:node_5094,prsc:2|A-5983-RGB,B-5465-RGB,T-1799-OUT;n:type:ShaderForge.SFN_Lerp,id:9713,x:32361,y:32852,varname:node_9713,prsc:2|A-4805-A,B-2016-A,T-1799-OUT;n:type:ShaderForge.SFN_Lerp,id:1145,x:32342,y:33274,varname:node_1145,prsc:2|A-5983-A,B-5465-A,T-1799-OUT;n:type:ShaderForge.SFN_Lerp,id:1814,x:32721,y:32718,varname:node_1814,prsc:2|A-5715-OUT,B-7144-RGB,T-7058-OUT;n:type:ShaderForge.SFN_Lerp,id:9119,x:32721,y:32882,varname:node_9119,prsc:2|A-9713-OUT,B-7144-A,T-7058-OUT;n:type:ShaderForge.SFN_Lerp,id:3160,x:32728,y:33145,varname:node_3160,prsc:2|A-5094-OUT,B-2694-RGB,T-7058-OUT;n:type:ShaderForge.SFN_Lerp,id:2243,x:32728,y:33316,varname:node_2243,prsc:2|A-1145-OUT,B-2694-A,T-7058-OUT;proporder:4805-5983-2016-5465-7144-2694-1799-7058;pass:END;sub:END;*/

Shader "Shader Forge/SpriteTransfer" {
    Properties {
        _Texture_NoAnimal ("Texture_NoAnimal", 2D) = "white" {}
        _Color_NoAnimal ("Color_NoAnimal", Color) = (1,1,1,1)
        _Texture_SomeAnimals ("Texture_SomeAnimals", 2D) = "gray" {}
        _Color_SomeAnimals ("Color_SomeAnimals", Color) = (0.5,0.5,0.5,1)
        _Texture_AllAnimals ("Texture_AllAnimals", 2D) = "black" {}
        _Color_AllAnimals ("Color_AllAnimals", Color) = (0,0,0,1)
        _SomeAnimalsAmount ("SomeAnimalsAmount", Range(0, 1)) = 0
        _AllAnimalsAmount ("AllAnimalsAmount", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
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
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture_NoAnimal; uniform float4 _Texture_NoAnimal_ST;
            uniform float4 _Color_NoAnimal;
            uniform sampler2D _Texture_SomeAnimals; uniform float4 _Texture_SomeAnimals_ST;
            uniform float4 _Color_SomeAnimals;
            uniform sampler2D _Texture_AllAnimals; uniform float4 _Texture_AllAnimals_ST;
            uniform float4 _Color_AllAnimals;
            uniform float _SomeAnimalsAmount;
            uniform float _AllAnimalsAmount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _Texture_NoAnimal_var = tex2D(_Texture_NoAnimal,TRANSFORM_TEX(i.uv0, _Texture_NoAnimal));
                float4 _Texture_SomeAnimals_var = tex2D(_Texture_SomeAnimals,TRANSFORM_TEX(i.uv0, _Texture_SomeAnimals));
                float4 _Texture_AllAnimals_var = tex2D(_Texture_AllAnimals,TRANSFORM_TEX(i.uv0, _Texture_AllAnimals));
                float node_603 = (lerp(lerp(_Texture_NoAnimal_var.a,_Texture_SomeAnimals_var.a,_SomeAnimalsAmount),_Texture_AllAnimals_var.a,_AllAnimalsAmount)*lerp(lerp(_Color_NoAnimal.a,_Color_SomeAnimals.a,_SomeAnimalsAmount),_Color_AllAnimals.a,_AllAnimalsAmount)*i.vertexColor.a); // A
                float3 emissive = ((lerp(lerp(_Texture_NoAnimal_var.rgb,_Texture_SomeAnimals_var.rgb,_SomeAnimalsAmount),_Texture_AllAnimals_var.rgb,_AllAnimalsAmount)*lerp(lerp(_Color_NoAnimal.rgb,_Color_SomeAnimals.rgb,_SomeAnimalsAmount),_Color_AllAnimals.rgb,_AllAnimalsAmount)*i.vertexColor.rgb)*node_603);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_603);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
