// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Outline(3)"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_outlineFactor1("_outlineFactor1", Range(0, 2)) = 0
		_outlineFactor2("_outlineFactor2", Range(0, 2)) = 0
	}
	SubShader
	{
		Pass
		{
			ZWrite Off
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _outlineFactor1;
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz = v.vertex.xyz + normalize(v.normal) * _outlineFactor1;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = 0;
				return col;
			}
			ENDCG
		}

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _outlineFactor2;
			
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz = v.vertex.xyz + normalize(v.normal) * _outlineFactor2;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
