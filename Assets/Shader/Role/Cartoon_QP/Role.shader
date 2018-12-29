Shader "Custom/Role"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_MaskTex("_MaskTex(r:specular mask)", 2D) = "white"{}
		_DiffuseStrength("_DiffuseStrength", Float) = 1.0
		_SpecularStrength("_SpecularStrength", Float) = 1.0
		_SpecularGloss("_SpecularGloss", Float) = 1.0
		_FresnelColor("_FresnelColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_FresnelGloss("_FresnelGloss", Float) = 1.0
		_FresnelStrength("_FresnelStrength", Float) = 1.0
		_FresnelDir("_FresnelDir", Range(-1.0, 1.0)) = 1.0 
	}

	SubShader
	{
		Tags
		{

		}

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
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
				SHADOW_COORDS(3)
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _DiffuseStrength;
			float _SpecularStrength;
			float _SpecularGloss;
			float4 _FresnelColor;
			float _FresnelGloss;
			float _FresnelStrength;
			float _FresnelDir;

			v2f vert(a2v v)
			{
				v2f o = (v2f)0;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_SHADOW(o) 
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 texColor = tex2D(_MainTex, i.uv).rgb;
				fixed4 maskColor = tex2D(_MaskTex, i.uv);

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 N = normalize(i.normal);
				fixed3 diffuse = _DiffuseStrength * _LightColor0.rgb * saturate(dot(i.normal, lightDir));
				fixed3 H = normalize(lightDir + viewDir);
				fixed3 specular = maskColor.r * _LightColor0.rgb *_SpecularStrength * pow(saturate(dot(H, N)), _SpecularGloss);

				fixed3 fresnelDir = cross(viewDir, fixed3(0.0, 1.0, 0.0)) * _FresnelDir;
				fixed3 fresnelColor = smoothstep(0.65, 1, dot(fresnelDir, N)) * _FresnelStrength * _FresnelColor.rgb ;//* pow(1 - saturate(dot(viewDir, N)), _FresnelGloss);				
				fixed3 finalColor = texColor * ((diffuse + specular) * SHADOW_ATTENUATION(i)  + UNITY_LIGHTMODEL_AMBIENT) + fresnelColor;
				return fixed4(finalColor, 1.0f);
			}

			ENDCG
		}
	}

	Fallback "Diffuse"
}