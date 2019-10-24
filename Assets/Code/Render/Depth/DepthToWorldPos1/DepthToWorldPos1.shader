Shader "LH/DepthToWorldPos1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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

	sampler2D _CameraDepthTexture;
	float4x4 _ProjMatInv;

	v2f vert(a2v i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv = i.uv;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float eyeDepth = LinearEyeDepth(depth);
		float4 ndcPos = float4(i.uv.x * 2 - 1, i.uv.y * 2 -1, 1, 1.0);
		float4 ray = mul(_ProjMatInv, ndcPos);
		ray = ray / ray.w;
		ray = ray / ray.z * -1;
		ray = ray * eyeDepth;
		//float3 worldPos = _WorldSpaceCameraPos + ray.xyz;
		return float4(ray.z * -1 / 10, 0, 0, 1);
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
