// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/RadialBlur"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		samplerDis("samplerDis", Range(0.01, 0.1)) = 0.1
		disFactor("disFactor", float) = 0.1
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

	float2 centerPos;
	float samplerDis;
	float disFactor;
	sampler2D _MainTex;

	static const float samples[8] =   
    {   
    	1,
    	2,
    	3,
    	4,
    	5,
    	6,
    	7,
    	8
     }; 

	v2f vert(a2v i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = i.uv;

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float2 dir = float2(0.5f, 0.5f) - i.uv;
		float dis = length(dir);
		dir /= dis;
		dir *= samplerDis;

		half4 color = half4(0.0f, 0.0f, 0.0f, 0.0f);
		for(int index = 0; index < 8; index++)
		{
			color += tex2D(_MainTex, i.uv + dir * samples[index]);
		}

		color /= 8;

		fixed4 finalColor = lerp(tex2D(_MainTex, i.uv), color, saturate(dis * disFactor));
		return finalColor;
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