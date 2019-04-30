Shader "Custom/Fur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NormalScale1("_NormalScale1", Float) = 1
		_NoiseTex1("_NoiseTex1", 2D) = "white"{}
		_NormalScale2("_NormalScale2", Float) = 1
		_NoiseTex2("_NoiseTex2", 2D) = "white"{}
		_AlphaTH2("_AlphaTH", Float) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _NormalScale1;
			sampler2D _NoiseTex1;
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz = v.vertex.xyz + v.normal * _NormalScale1;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a = tex2D(_NoiseTex1, i.uv).r;
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
			float _NormalScale2;
			float _AlphaTH2;
			sampler2D _NoiseTex2;
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xyz = v.vertex.xyz + v.normal * _NormalScale2;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed alpha = tex2D(_NoiseTex2, i.uv).r;
				col.a = step(_AlphaTH2, alpha);
				return col;
			}
			ENDCG
		}
	}
}
