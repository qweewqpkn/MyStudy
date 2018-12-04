Shader "LH/RM_Sphere"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
	}

	SubShader
	{
		Pass
		{
			ZTest Always
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
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
				float3 ray : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
			};

			static int MARCHING_STEPS = 16;
			static float MAX_DIST = 10;
			sampler2D _MainTex;
			float4x4 _FrustumCornerRay;

			float Sphere(float3 p)
			{
				float R = 0.5;
				return length(p) - R;
			}

			float sdTorus(float3 p, float2 t)
			{
  				float2 q = float2(length(p.xz)-t.x,p.y);
  				return length(q)-t.y;
			}

            float sdCube( float3 p, float3 b, float r )
            {
              return length(max(abs(p)-b,0.0))-r;
            }

			//返回点p到目标物体的距离
			float Map(float3 p)
			{
				return sdCube(p, float3(0.0, 2.0, 0.0), 1);
				//return Sphere(p);
				//return sdTorus(p, float2(0.5, 1));
			}

			float3 calcNorm(float3 p)
            {
                float eps = 0.001;

                float3 norm = float3(
                    Map(p + float3(eps, 0, 0)) - Map(p - float3(eps, 0, 0)),
                    Map(p + float3(0, eps, 0)) - Map(p - float3(0, eps, 0)),
                    Map(p + float3(0, 0, eps)) - Map(p - float3(0, 0, eps))
                );

                return normalize(norm);
            }

			float4 RayMarch(float3 ro, float3 rd, float3 lightDir,float maxD)
			{
				float t = 0.0f;
				float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);
				for(int i = 0; i < MARCHING_STEPS; i++)
				{
					//射线前进，到达下个步进点
					float3 p = ro + rd * t;
					float d = Map(p);
					if(d < 0.01f)
					{
						//如果距离小于很小的值,那么认为触碰到了目标的表面
						float3 normal = calcNorm(p);
						color.rgb = _LightColor0.rgb * saturate(dot(normal, lightDir));
						color.a = 1.0f;
						break;
					}

					//如果射线没有到达目标表面，那么继续前进
					t += d;
					if(t >= maxD)
					{
						//如果大于了最大距离,都还没达到目标的表面，那么就返回了
						break;
					}
				}

				return color;
			}

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				int index = 0;
				if(i.uv.x < 0.5f && i.uv.y < 0.5f)
				{
					index = 0;
				}
				else if(i.uv.x > 0.5f && i.uv.y < 0.5f)
				{
					index = 1;
				}
				else if(i.uv.x < 0.5f && i.uv.y > 0.5f)
				{
					index = 2;
				}
				else if(i.uv.x > 0.5f && i.uv.y > 0.5f)
				{
					index = 3;
				}
				o.ray = _FrustumCornerRay[index];
				o.worldPos = mul(unity_ObjectToWorld, i.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float4 bgColor = tex2D(_MainTex, i.uv);
				float3 rd = normalize(i.ray);
				float3 ro = _WorldSpaceCameraPos;
				float4 color = RayMarch(ro, rd, lightDir,MAX_DIST);

				float4 finalColor = float4(lerp(bgColor.rgb, color.rgb, color.a), 1.0f);
				return finalColor;
			}

			ENDCG
		}
	}
}