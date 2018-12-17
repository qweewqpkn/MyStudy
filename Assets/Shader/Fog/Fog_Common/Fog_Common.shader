Shader "LH/Fog/Fog_Common"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FogColor("_FogColor", Color) = (1,1,1,1)
		_FogStart("_FogStart", Float) = 1
		_FogEnd("_FogEnd", Float) = 10
		_FogIntensity("_FogIntensity", Range(0, 1)) = 1
		[KeywordEnum(LinearFog, Exp, Exp2)] _FogMode("_FogMode", Float) = 0
		[KeywordEnum(Z, EulerDist, WorldHeight)] _DistMode("_DistMode", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry"} 
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag	
			#include "UnityCG.cginc"
			#pragma multi_compile _FOGMODE_LINEARFOG _FOGMODE_EXP _FOGMODE_EXP2
			#pragma multi_compile _DISTMODE_Z _DISTMODE_EULERDIST _DISTMODE_WORLDHEIGHT

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 viewPos : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _FogColor;
			float _FogStart;
			float _FogEnd;
			float _FogIntensity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewPos = mul(UNITY_MATRIX_MV, v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float fogWeight = 0;
				float dist = 0;

				#ifdef _DISTMODE_Z
					dist = i.viewPos.z;
				#elif _DISTMODE_EULERDIST 
					dist = length(i.viewPos);
				#elif _DISTMODE_WORLDHEIGHT
					dist = i.worldPos.y;
				#endif

				#ifdef _FOGMODE_LINEARFOG
					#ifdef _DISTMODE_WORLDHEIGHT
						fogWeight = clamp( (abs(dist) - _FogStart) / (_FogEnd - _FogStart), 0.0, 1.0);
					#else
						fogWeight = clamp((_FogEnd - abs(dist)) / (_FogEnd - _FogStart), 0.0, 1.0);
					#endif
				#elif _FOGMODE_EXP
					fogWeight = clamp(exp(-abs(_FogIntensity * dist)), 0.0, 1.0);
				#elif _FOGMODE_EXP2
					fogWeight = clamp(exp(-abs(_FogIntensity * dist) * (_FogIntensity * dist)), 0.0, 1.0);
				#endif

				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed4 finalColor = lerp(_FogColor, texColor, fogWeight);
				return finalColor;
			}
			ENDCG
		}
	}
}
