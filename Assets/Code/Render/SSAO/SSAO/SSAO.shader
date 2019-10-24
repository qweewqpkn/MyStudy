Shader "LH/SSAO"
{
	Properties
	{
		_NoiseTex("_NoiseTex", 2D) = "white"{}
		_Radius("_Radius", Float) = 1.0
		_MainTex("_MainTex", 2D) = "white"{}
	}

	SubShader
	{
		ZWrite Off
		ZTest Always
		Cull Off

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

		sampler2D _SSAOTex;
		sampler2D _MainTex;
		sampler2D _NoiseTex;
		sampler2D _CameraDepthNormalsTexture;
		sampler2D _CameraDepthTexture;
		float4x4 _ProjectionInverseMatrix;
		float4x4 _ProjectionMatrix;
		float4 _Samples[64];
		float _Radius;

		v2f vert_ssao(a2v i)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(i.pos);
			o.uv = i.uv;
			return o;
		}

		fixed4 frag_ssao(v2f i) : SV_Target
		{
			//求出摄像机空间坐标
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
			#if defined(UNITY_REVERSED_Z)
			depth = 1.0 - depth;
			#endif
			float4 ndcPos = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, depth * 2 - 1, 1);
			float4 viewPos = mul(_ProjectionInverseMatrix, ndcPos);
			viewPos = viewPos / viewPos.w;

			//构建TBN(由于我们随机点生成是在切线空间，所以需要有TBN矩阵以便转换到view space)
			float3 normal;
			float depth1;
			DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth1, normal);
			float3 randomVec = tex2D(_NoiseTex, i.uv).xyz;
			normal = normalize(normal);
			float3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
			float3 bitangent = cross(tangent, normal);
			float3x3 TBN = transpose(float3x3(tangent, bitangent, normal));

			float occlusion = 0.0f;
			for(int j = 0; j < 64; j++)
			{
				//将随机采样点从切线空间转换到摄像机空间
				float3 sample = mul(TBN, _Samples[j].xyz);
				//将当前像素在摄像机空间的位置进行一定偏移		
				sample = viewPos.xyz + sample * _Radius;
				//求取偏移后对应的屏幕空间坐标，用于采样对应点的深度值
				float4 offset = float4(sample, 1.0f);
				offset = mul(_ProjectionMatrix, offset);
				offset.xyz /= offset.w;
				offset.xyz = offset.xyz * 0.5f + 0.5f;
 
				float sampleDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset.xy);
				//如果偏移点的深度大于对应像素的深度值，那么就对我们原始像素有遮蔽的作用
				occlusion += abs(sample.z) >= LinearEyeDepth(sampleDepth) ? 1.0f : 0.0f;
			}

			occlusion = 1 - (occlusion / 64);
			return fixed4(occlusion, occlusion, occlusion, 1.0f);
		}

		v2f vert_add(a2v i)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(i.pos);
			o.uv = i.uv;
			return o;
		}

		fixed4 frag_add(v2f i) : SV_Target
		{
			float aoFactor = tex2D(_SSAOTex, i.uv).r;
			fixed4 color = tex2D(_MainTex, i.uv) * aoFactor;
			return color;

		}

		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_ssao
			#pragma fragment frag_ssao
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_add
			#pragma fragment frag_add
			ENDCG
		}
	}
}