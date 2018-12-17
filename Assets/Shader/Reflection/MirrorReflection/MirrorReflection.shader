Shader "LH/MirrorReflection"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_Color("_Color", Color) = (1,1,1,1)
		_Gloss("_Gloss", Float) = 8
		_SpecularStrength("_SpecularStrength", Float) = 1
		_LerpReflect("_LerpReflect", Range(0, 1)) = 1
		_MaskTex("_MaskTex(r:specular mask)", 2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ LIGHTMAP_ON

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uvLM : TEXCOORD1;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uvMain : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
				float3 normal : TEXCOORD3;
				#ifdef LIGHTMAP_ON
				float2 uvLM : TEXCOORD4;
				#endif
				float2 uvMask : TEXCOORD5;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _ReflectTex;
			float _Gloss;
			float _SpecularStrength;
			float _LerpReflect;
			float4 _LightPos;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvMask = TRANSFORM_TEX(v.uv, _MaskTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				#ifdef LIGHTMAP_ON
				o.uvLM = v.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 maskColor = tex2D(_MaskTex, i.uvMask);
				fixed3 normal = normalize(i.normal);
				float3 lightDir = normalize(-_LightPos.xyz);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 H = normalize(lightDir + viewDir);
				fixed3 specular = _SpecularStrength * _LightColor0.rgb * pow(saturate(dot(H, normal)), _Gloss) * maskColor.r;

				fixed3 lmColor = 0;
				#ifdef LIGHTMAP_ON
				lmColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM));
				#endif

				fixed3 texColor = tex2D(_MainTex, i.uvMain).rgb;
				fixed3 reflectColor = tex2D(_ReflectTex, i.screenPos.xy / i.screenPos.w).rgb * maskColor.r;

				fixed3 finalColor = lerp(texColor, reflectColor, _LerpReflect) * lmColor * _Color + specular;
				return fixed4(finalColor, 1.0);
			}
			ENDCG
		}
	}
}
