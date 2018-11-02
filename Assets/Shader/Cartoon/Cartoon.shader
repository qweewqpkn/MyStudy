// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Cartoon"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_EdgeColor("_EdgeColor", Color) = (0.0, 0.0, 0.0, 1.0)
		_EdgeWidth("_EdgeWidth", Range(0, 0.1)) = 0
		_BackColor("_BackColor", Color) = (0.0, 0.0, 0.0, 1.0)
		_RampTex("_RampTex", 2D) = "white"{}
		_Gloss("_Gloss", Range(1, 128)) = 1
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Lighting.cginc"

	struct a2v
	{
		float4 pos : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : TEXCOORD1;
		float3 worldPos : TEXCOORD2;
	};

	float _EdgeWidth;
	float4 _EdgeColor;
	float4 _BackColor;
	sampler2D _MainTex;
	sampler2D _RampTex;
	float _Gloss;

	v2f vertEdge(a2v i)
	{
		v2f o;
		i.pos.xyz = i.pos.xyz + _EdgeWidth * normalize(i.normal);
		o.pos = UnityObjectToClipPos(i.pos);
		return o;
	}

	fixed4 fragEdge(v2f i) : SV_Target
	{
		return _EdgeColor;
	}

	v2f vert(a2v i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = i.uv;
		o.normal = mul(i.normal, (float3x3)unity_WorldToObject);
		o.worldPos = mul(unity_ObjectToWorld, i.pos);
		return o;
	}

	fixed4 frag(v2f i ): SV_Target
	{
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
		fixed3 normal = normalize(i.normal);

		fixed3 texColor = tex2D(_MainTex, i.uv).rgb;
		fixed  nDotl = dot(normal, lightDir);
		fixed halfLambert = nDotl * 0.5 + 0.5;
		//fixed3 rampColor = halfLambert > 0.5 ? _LightColor0.rgb : _BackColor.rgb;
		fixed3 rampColor = tex2D(_RampTex, float2(halfLambert, halfLambert)).rgb;
		fixed3 diffuse = _LightColor0 * rampColor;

		fixed3 halfDir = normalize(lightDir + viewDir);
		fixed spec = saturate(dot(halfDir, normal));
		fixed3 specular = fixed3(1.0f, 1.0f, 1.0f) * pow(spec, _Gloss) * saturate(nDotl);

		fixed4 finalColor;
		finalColor.rgb = texColor * (diffuse + specular);
		finalColor.a = 1.0f;
		return finalColor;

	}

	ENDCG

	SubShader
	{
		Pass
		{
			Cull Front
			ZWrite On

			CGPROGRAM

			#pragma vertex vertEdge
			#pragma fragment fragEdge
			ENDCG
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
			ENDCG
		}
	}
}