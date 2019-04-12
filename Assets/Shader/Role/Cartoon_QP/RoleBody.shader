Shader "Custom/RoleBody"
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
		_fOutlineWidth("_Outline Width", float) = 0.001
		_f4OutlineColor("_Outline Color", Color) = (0.0,0.0,0.0,0.0)
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

				//计算第一盏光照
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 N = normalize(i.normal);
				fixed3 diffuse = _DiffuseStrength * _LightColor0.rgb * saturate(dot(N, lightDir));
				fixed3 H = normalize(lightDir + viewDir);
				fixed3 specular = maskColor.r * _LightColor0.rgb *_SpecularStrength * pow(saturate(dot(H, N)), _SpecularGloss);

				//计算第二盏光照
				float3 lightDir1 = float4(unity_4LightPosX0.x, unity_4LightPosY0.x, unity_4LightPosZ0.x, 1) - i.worldPos;
				fixed3 lightDir1Normalize = normalize(lightDir);
				fixed3 viewDir1 = normalize(UnityWorldSpaceViewDir(i.worldPos));
				//float attenuation = 1.0 / (1.0 + unity_4LightAtten0.x * dot(lightDir1, lightDir1));
				diffuse +=  _DiffuseStrength * unity_LightColor[0].rgb * saturate(dot(N, lightDir1Normalize));
				fixed3 H1 = normalize(lightDir1Normalize + viewDir1);
				specular += maskColor.r * unity_LightColor[0].rgb *_SpecularStrength * pow(saturate(dot(H1, N)), _SpecularGloss);

				//计算阴影和衰减
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos.xyz);
				//fixed3 fresnelDir = cross(viewDir, fixed3(0.0, 1.0, 0.0)) * _FresnelDir;
				fixed3 fresnelColor = pow(1 - saturate(dot(viewDir, N)), _FresnelGloss) * (1 - abs(smoothstep(-1, 0, dot(N, lightDir)))) * _FresnelStrength * _FresnelColor.rgb ;			
				fixed3 finalColor = texColor * ((diffuse + specular) * atten  + UNITY_LIGHTMODEL_AMBIENT) + fresnelColor;
				return fixed4(finalColor, 1.0f);
			}
			

			ENDCG
		}

		Pass
		{
			Cull front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			float _fOutlineWidth;
			fixed4 _f4OutlineColor;

			float4 vert(appdata v) : SV_POSITION
			{
				return UnityObjectToClipPos(v.vertex + normalize(v.normal) * _fOutlineWidth);
			}

			fixed4 frag() : SV_Target
			{
				return _f4OutlineColor;
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}