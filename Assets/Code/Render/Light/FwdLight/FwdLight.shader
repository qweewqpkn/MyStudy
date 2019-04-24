// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/FwdLight"
{
	Properties
	{
		
	}

	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct a2v
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldLightDir : TEXCOORD1;
			};

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.worldNormal = mul(i.normal, (float3x3)unity_WorldToObject);
				o.worldLightDir = WorldSpaceLightDir(i.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color;
				fixed3 diffuse = _LightColor0 * saturate(dot(normalize(i.worldNormal), normalize(i.worldLightDir)));
				color.rgb = diffuse;
				color.a = 1.0f;
				return color;
			}

			ENDCG
		}

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardAdd"
			}

			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct a2v
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldLightDir : TEXCOORD1;
			};

			float pointRange;

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.worldNormal = mul(i.normal, (float3x3)unity_WorldToObject);
				o.worldLightDir = WorldSpaceLightDir(i.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float attenuation = saturate(1 - length(i.worldLightDir) / pointRange);
				fixed4 color;
				fixed3 diffuse = attenuation * _LightColor0 * saturate(dot(normalize(i.worldNormal), normalize(i.worldLightDir)));
				color.rgb = diffuse;
				color.a = 1.0f;
				return color;
			}

			ENDCG
		}
	}
}


