// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Texture"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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
			float4 _MainTex_TexelSize;

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
					{
    					o.uv.y = 1 - o.uv.y;
					}
				#endif

				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}

			ENDCG
		}
	}
}