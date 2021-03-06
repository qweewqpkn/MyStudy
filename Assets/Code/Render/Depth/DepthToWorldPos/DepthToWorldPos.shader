﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/DepthToWorldPos"
{
	Properties
	{

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
		float2 uv : TEXCOORD0;
		float4 pos1 : TEXCOORD1;
	};

	sampler2D _CameraDepthTexture;
	float4x4 ProjectInverseMatrix;
	float4x4 WroldToViewInverseMatrix;

	v2f vert(a2v i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = i.uv;
		o.uv.y = 1 - i.uv.y;
		o.pos1 = UnityObjectToClipPos(i.pos);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float linearDepth = Linear01Depth(depth);
		float4 clipPos = float4(i.uv.x*2 -1, i.uv.y*2 -1, depth, 1.0f);
		float4 viewPos = mul(ProjectInverseMatrix, clipPos);
		viewPos = viewPos / viewPos.w;
		float4 worldPos = mul(WroldToViewInverseMatrix, viewPos);
		return float4(viewPos.x, 0.0, 0.0, 1.0f);
	}

	ENDCG

	SubShader
	{
		Tags
		{

		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
