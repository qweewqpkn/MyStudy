Shader "LH/RM_Sphere"
{
	Properties
	{

	}

	SubShader
	{
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
				float2 screen_uv : TEXCOORD1;
			};

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				o.screen_uv = o.pos.xy / o.pos.w;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if(length(i.screen_uv) < 0.5)
				{
					return fixed4(1.0f, 0.0f, 0.0f, 1.0f);
				}

				return fixed4(0.0f, 1.0f, 0.0f, 1.0f);
			}

			ENDCG
		}
	}
}