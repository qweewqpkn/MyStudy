Shader "LH/AnisotropicSpecular"
{
	Properties
	{
		_Gloss("_Gloss", Float) = 1
		_Offset("_Offset", Float) = 0.5
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
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			struct a2v
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float3 tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 lightDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 tangent : TEXCOORD3;
			};

			float _Gloss;
			float _Offset;

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.normal = UnityObjectToWorldNormal(i.normal);
				o.lightDir = WorldSpaceLightDir(i.pos);
				o.viewDir = WorldSpaceViewDir(i.pos);
				o.tangent = UnityObjectToWorldDir(i.tangent);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 normal = normalize(i.normal);
				float3 lightDir = normalize(i.lightDir);
				float3 viewDir = normalize(i.viewDir);
				float3 tangent = normalize(i.tangent);
				float3 binormal = normalize(cross(normal, tangent));
				
				float NdotL = saturate(dot(lightDir, normal));
				fixed3 diffuse = _LightColor0 * NdotL;

				float3 H = normalize(lightDir + viewDir);
				float HdotT = dot(H, normalize(tangent + normal * _Offset));
				float sinHT = sqrt(1 - HdotT * HdotT);
				fixed dirAtten = smoothstep(-1, 0, HdotT);
				fixed3 specular = _LightColor0 * dirAtten * pow(sinHT, _Gloss);

				fixed4 finalColor = 0;
				finalColor.rgb = diffuse + UNITY_LIGHTMODEL_AMBIENT + specular;
				finalColor.a = 1.0f;
				return finalColor;
			}
			
			ENDCG
		}
	}
}
