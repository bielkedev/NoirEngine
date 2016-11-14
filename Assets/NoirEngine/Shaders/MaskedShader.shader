Shader "Hidden/MaskedShader"
{
	Properties
	{
		_MainTex ("Sprite", 2D) = "white"
		_MaskTex ("Mask", 2D) = "white"
		_Alpha ("Alpha", Float) = 1.0
	}
	SubShader
	{
		Cull Off
		Lighting Off
		ZTest Off
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _Alpha;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a *= tex2D(_MaskTex, i.uv).a * _Alpha;
				return col;
			}
			ENDCG
		}
	}
}