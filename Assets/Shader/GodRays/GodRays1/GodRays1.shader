// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/GodRays"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_BlendColor("_BlendColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_DstFactor("_DstFactor", Range(0, 0.01)) = 0.01
		_DecayFactor("_DecayFactor", Float) = 0.5
		_GodRayStrength("_GodRayStrength", Float) = 1.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct a2v
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _BlendTex;
	sampler2D _CameraDepthTexture;
	float4 lightScreenPos;
	float _DstFactor;
	float4 _BlendColor;
	float4  _MainTex_TexelSize;
	float _DecayFactor;
	float _GodRayStrength;

	v2f vert(a2v i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv.xy = i.uv;
		#if UNITY_UV_STARTS_AT_TOP
			o.uv.zw = i.uv.xy;
			if(_MainTex_TexelSize.y < 0)
			{
				o.uv.w = 1 - o.uv.w;
			}
		#endif
		return o;
	}

	fixed4 fragDepth(v2f i) : SV_Target
	{
		//对于有深度物体才保存它的颜色，没有深度的地方就是黑色
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.zw);
		float linearDepth = Linear01Depth(depth);

		half4 color = half4(1.0f, 1.0f, 1.0f, 1.0f);

		if(linearDepth > 0.99)
			return _BlendColor;

		return color; 
	}

	fixed4 fragRadial(v2f i) : SV_Target 
	{
		float2 dir = (lightScreenPos.xy - i.uv.xy ) * _DstFactor ;
		float3 color = float3(0.0f, 0.0f, 0.0f);
		float decayFactor = _DecayFactor;
		for(int j = 0; j < 8; j++)
		{
			i.uv.xy += dir * j;
			color += tex2D(_MainTex, i.uv.xy) * decayFactor;
			decayFactor *= _DecayFactor;
		}

		return float4(color, 1.0f);
	}

	fixed4 fragBlend(v2f i) : SV_Target
	{
		fixed4 texColor = tex2D(_MainTex, i.uv.xy);
		fixed4 blendColor = tex2D(_BlendTex, i.uv.xy);
		fixed4 finalColor = texColor + blendColor;

		return finalColor;
	}

	ENDCG

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragDepth
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragRadial
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragBlend

			ENDCG
		}
	}
}