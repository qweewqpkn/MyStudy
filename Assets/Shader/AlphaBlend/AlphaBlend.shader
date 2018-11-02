// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Blend"
{
	Properties
	{
		_mainTex("_mainTex", 2D) = "white"{}
		_alpha("_alpha", Range(0,1)) = 0.5
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _mainTex;
			float _alpha;

			struct a2v
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color;
				color.rgb = tex2D(_mainTex, i.uv);
				color.a = _alpha;
				return color;
			}

			ENDCG
		}
	}
}