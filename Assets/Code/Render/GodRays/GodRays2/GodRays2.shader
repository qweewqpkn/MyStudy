Shader "LH/GodRays2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LuminanceThreshold("_LuminanceThreshold", Float) = 0.5
		_LightScreenPos("_LightScreenPos", Vector) = (0.1, 0.1, 0.0, 0.0)
		_BlurOffset("_BlurOffset", Vector) = (0, 0, 0, 0)
		_LuminanceStrength("_LuminanceStrength", Float) = 1.0
	}
	SubShader
	{
		ZTest Off
		Cull Off

		CGINCLUDE
		#include "UnityCG.cginc"
		struct a2v
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _LuminanceThreshold;
		float2 _LightScreenPos;
		float4 _BlurOffset;
		sampler2D _LuminanceTex;
		float _LuminanceStrength;

		v2f vert_get(a2v i)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(i.vertex);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			return o;
		}
		
		fixed4 frag_get(v2f i) : SV_Target
		{
			fixed4 texColor = tex2D(_MainTex, i.uv);
			float luminance = texColor.r * 0.7f + texColor.g * 0.2f + texColor.b * 0.1f;
			float color = saturate(luminance - _LuminanceThreshold);
			return float4(color, color, color, 1.0f);
		}

		v2f vert_blur(a2v i)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(i.vertex);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			return o;
		}

		fixed4 frag_blur(v2f i) : SV_Target
		{
			float4 color = float4(0.0f, 0.0f, 0.0f, 1.0f);
			float2 blurDir = -(i.uv - _LightScreenPos.xy) * _BlurOffset;
			float2 uv = float2(0.0f, 0.0f);
			for(int index = 0; index < 4; index++)
			{
				uv = i.uv + index * blurDir;
				color += tex2D(_MainTex, uv);
			}

			return color / 3;
		}

		v2f vert_merge(a2v i)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(i.vertex);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			return o;
		}

		fixed4 frag_merge(v2f i) : SV_Target
		{
			return tex2D(_MainTex, i.uv) + tex2D(_LuminanceTex, i.uv) * _LuminanceStrength;
		}

		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_get
			#pragma fragment frag_get		
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_blur
			#pragma fragment frag_blur	
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_merge
			#pragma fragment frag_merge	
			ENDCG
		}

	}
}
