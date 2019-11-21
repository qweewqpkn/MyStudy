Shader "LH/CBBlur"
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col1 = tex2D(_MainTex, i.uv + half2(1, 0) * _MainTex_TexelSize.xy);
				fixed4 col2 = tex2D(_MainTex, i.uv + half2(0, 1) * _MainTex_TexelSize.xy);
				fixed4 col3 = tex2D(_MainTex, i.uv + half2(0, 0) * _MainTex_TexelSize.xy);
				fixed4 col4 = tex2D(_MainTex, i.uv + half2(1, 1) * _MainTex_TexelSize.xy);
				return (col1 + col2 + col3 + col4) / 4;
			}
			ENDCG
		}
	}
}
