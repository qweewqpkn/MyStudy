Shader "Custom/Hair"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_MaskTex("_MaskTex(r:specular mask, g:disturb mask)", 2D) = "white"{}
		_SpecularGloss1("_SpecularGloss1", Float) = 1.0
		_SpecularStrength1("_SpecularStrength1", Float) = 1.0
		_SpecularColor1("_SpecularColor1", Color) = (1.0, 1.0, 1.0, 0.0)
		_SpecularGloss2("_SpecularGloss2", Float) = 1.0
		_SpecularStrength2("_SpecularStrength2", Float) = 1.0
		_SpecularColor2("_SpecularColor2", Color) = (1.0, 1.0, 1.0, 0.0)
		_OffsetT1("_OffsetT1", Range(-1.0, 1.0)) =	1.0
		_OffsetT2("_OffsetT2", Range(-1.0, 1.0)) = 1.0
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
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 tangent : TEXCOORD2;
				float2 uv : TEXCOORD3;
				float2 uv1 : TEXCOORD4;
				SHADOW_COORDS(5)
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			float _SpecularGloss1;
			float _SpecularStrength1;
			float4 _SpecularColor1;
			float _SpecularGloss2;
			float _SpecularStrength2;
			float4 _SpecularColor2;
			float _OffsetT1;
			float _OffsetT2;

			fixed CalcSpecular(fixed3 lightDir, fixed3 viewDir, fixed3 tangent, float exponent)
			{
				fixed3 H = normalize(lightDir + viewDir);
				fixed dotTH = dot(H, tangent);
				fixed sinTH = sqrt(1 - dotTH * dotTH);
				fixed atten = smoothstep(-1, 0, dotTH);
				return atten * pow(sinTH, exponent);
			}

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
				o.uv1 = TRANSFORM_TEX(v.uv, _MaskTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_SHADOW(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 maskColor = tex2D(_MaskTex, i.uv1);
				fixed3 normal = normalize(i.normal);
				fixed3 tangent = normalize(i.tangent);
				fixed3 binormal = normalize(cross(i.normal, i.tangent));
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 diffuse = _LightColor0 * (dot(normal, lightDir) * 0.5 + 0.5);

				fixed3 T1 = normalize(binormal + normal * (maskColor.g + _OffsetT1));
				fixed3 T2 = normalize(binormal + normal * (maskColor.g + _OffsetT2));
				fixed3 specular = _SpecularColor1 * _SpecularStrength1 * CalcSpecular(lightDir, viewDir, T1, _SpecularGloss1);
				specular += _SpecularColor2 * _SpecularStrength2 * CalcSpecular(lightDir, viewDir, T2, _SpecularGloss2);
				specular *= maskColor.r * _LightColor0;
				fixed3 texColor = tex2D(_MainTex, i.uv);
				fixed3 finalColor =  texColor.rgb * ((diffuse + specular) * SHADOW_ATTENUATION(i) + UNITY_LIGHTMODEL_AMBIENT);
				return fixed4(finalColor, 1.0f);
			}


			ENDCG
		}
	}

	Fallback "Diffuse"
}