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
			};

			static int MARCHING_STEPS = 16;
			static float MAX_DIST = 10;

			float Map(float3 p)
			{
				float R = 1.0f;
				return length(p) - R;
			}

			float RayMarch(float3 ro, float3 rd, float maxD)
			{
				float t = 0.0f;
				for(int i = 0; i < MARCHING_STEPS; i++)
				{
					float3 p = ro + rd * t;
					float d = Map(p);
					if(d < 0.01f)
					{
						return t;
					}

					t += d;
					if(t >= maxD)
					{
						return maxD;
					}
				}

				return maxD;
			}

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed2 uv = i.uv;
				uv = uv * 2.0f - 1.0f;
				fixed3 rd = normalize(fixed3(uv, 1.0f));
				fixed3 ro = fixed3(0.0f, 0.0f, -2.0f);
				float dist = RayMarch(ro, rd, MAX_DIST);
				//return fixed4(dist, 0.0f, 0.0f, 1.0f);
				if(dist >= MAX_DIST - 0.01f)
				{
					return fixed4(0.0f, 0.0f, 0.3f, 1.0f);
				}

				return fixed4(1.0f, 0.0f, 0.0f, 1.0f);
			}

			ENDCG
		}
	}
}