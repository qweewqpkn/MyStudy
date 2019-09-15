Shader "Unlit/SSAO1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}


	CGINCLUDE
	struct a2v
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 viewRay : TEXCOORD1;
	};

	float4x4 _ProjMatrixInv;
	sampler2D _CameraDepthNormalsTexture ;


	v2f vertSSAO(a2v v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		float4 ndcPos = float4(v.uv.xy * 2 - 1, 1.0, 1.0);
		float4 viewRay = mul(_ProjMatrixInv, ndcPos);
		o.viewRay = viewRay.xyz / viewRay.w;
		return o;
	}
			
	fixed4 fragSSAO(v2f i) : SV_Target
	{
		float depth;
		float3 normal;
		float4 enc = tex2D(_CameraDepthNormalsTexture, i.uv);
		DecodeDepthNormal(enc, depth, normal)

		//还原坐标
		
		
		fixed4 col = tex2D(_MainTex, i.uv);
		return col;
	}
	ENDCG

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vertSSAO
			#pragma fragment fragSSAP
			ENDCG
		}
	}
}
