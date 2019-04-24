// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/AverageBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _BlurRadius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 =  v.uv;
				o.uv1 = v.uv + _MainTex_TexelSize * _BlurRadius * float2(1 ,1);
				o.uv2 = v.uv + _MainTex_TexelSize * _BlurRadius * float2(-1, 1);
				o.uv3 = v.uv + _MainTex_TexelSize * _BlurRadius * float2(1, -1);
				o.uv4 = v.uv + _MainTex_TexelSize * _BlurRadius * float2(-1, -1);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.0 , 0.0 ,0.0 ,0.0);
				// sample the texture
				col += tex2D(_MainTex, i.uv0);
				col += tex2D(_MainTex, i.uv1);
				col += tex2D(_MainTex, i.uv2);
				col += tex2D(_MainTex, i.uv3);
				col += tex2D(_MainTex, i.uv4);
				return col * 0.2;
			}
			ENDCG
		}
	}
}
