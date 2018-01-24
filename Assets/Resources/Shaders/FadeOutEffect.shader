// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EW/FadeOutEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Amount ("Amount", Float) = 0
		_Mult("Multiplier", Float) = 1
		_Invert("Invert", Int) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Amount;
			float _Mult;
			int _Invert;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);

				float amount = _Amount * _Mult;

				//remove black areas at lower amounts
				if (all(col == float4(0, 0, 0, 1)))
				{
					col = float4(1, 1, 1, amount);
				}
				else
				{
					col = float4(1 - amount, 1 - amount, 1 - amount, amount);
				}
				return col;
			}
			ENDCG
		}
	}
}
