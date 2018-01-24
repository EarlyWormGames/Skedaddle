// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EW/Greyscale"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Amount ("Amount", Float) = 0
		_Mult ("Multiplier", Float) = 1
		_Invert("Invert", Int) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 c = tex2D(_MainTex, i.uv);

				float lum = c.r*.3 + c.g*.59 + c.b*.11;
				float3 bw = float3(lum, lum, lum);

				float4 result = c;
				float amount = _Amount;
				if (_Invert == 1)
				{
					amount = 1 - amount;
				}
				result.rgb = lerp(c.rgb, bw, clamp(amount * _Mult, 0, 1));
				return result;
			}
			ENDCG
		}
	}
}
