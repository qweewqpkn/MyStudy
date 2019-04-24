// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Distortion"
{
	Properties
	{
		_NoiseTex("_NoiseTex", 2D) = "white"{}
		_MainTex("_MainTex", 2D) = "white"{}
		_TimeFactor("_TimeFactor", float) = 1.0
		_OffsetFactor("_OffsetFactor", float) = 0.1
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
	};

	sampler2D _NoiseTex;
	sampler2D _MainTex;
	float _TimeFactor;
	float _OffsetFactor;
 
	v2f vert(a2v i) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = i.uv;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed2 offset = tex2D(_NoiseTex, i.uv + _Time.y * _TimeFactor).rg;
		fixed3 color = tex2D(_MainTex, i.uv + offset * _OffsetFactor);
		return fixed4(color, 1.0f);
	}

	ENDCG

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}