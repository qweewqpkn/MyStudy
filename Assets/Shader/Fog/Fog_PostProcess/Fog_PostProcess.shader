Shader "LH/Fog/Fog_PostProcess"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZWrite Off
		ZTest Always
		Cull Off

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
			float _FogStart;
			float _FogEnd;
			float4 _FogColor;
			sampler2D _CameraDepthTexture;
			float4x4 _ProjectionInverseMatrix;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				//On DX11/12, PS4, XboxOne and Metal, the Z buffer range is 1–0 and UNITY_REVERSED_Z is defined. On other platforms, the range is 0–1.
				#if defined(UNITY_REVERSED_Z)
				depth = 1.0 - depth; 
				#endif
				float4 ndcPos = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, depth * 2 - 1, 1);
				float4 viewPos = mul(_ProjectionInverseMatrix, ndcPos);
				//mmp之前没加负号,一直在想为什么输出到屏幕这个值是黑色，结果camera空间z轴和世界空间下的z是反的
				//此时viewpos的范围是[near, far]
				viewPos = -viewPos / viewPos.w; 

				float fogWeight = (_FogEnd - viewPos.z) / (_FogEnd - _FogStart);
				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed4 finalColor = lerp(_FogColor, texColor, fogWeight);
				return finalColor;
			}
			ENDCG
		}
	}
}