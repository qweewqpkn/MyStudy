// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/uv_start_at_top"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"
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

	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;

	v2f vert(a2v i) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = i.uv;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 texColor1 = tex2D(_MainTex, i.uv);
		fixed4 texColor2 = tex2D(_CameraDepthTexture, i.uv);
		return texColor1 + texColor2 ; 
	}

	ENDCG

	SubShader
	{
		Pass
		{
			ZWrite Off
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}