Shader "LH/PlanarShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PlanePoint("_PlanePoint", Vector) = (0, 0, 0, 0)
		_PlaneNormal("_PlaneNormal", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType"="Opaque" 
			"Queue" = "Geometry+1"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Equal 
				Pass Keep
			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _PlanePoint;
			float3 _PlaneNormal;
			
			v2f vert (appdata v)
			{
				v2f o;
				//https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
				//平面与直线的交点方程
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 lightDir = UnityWorldSpaceLightDir(worldPos);
				float dist = dot(_PlanePoint.xyz - worldPos, _PlaneNormal) / dot(lightDir , _PlaneNormal);
				float4 projectPos = float4(worldPos + lightDir * dist, 1.0f);
				float4 objPos = mul(unity_WorldToObject, projectPos);
				o.vertex = UnityObjectToClipPos(objPos);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0.0f, 0.0f, 0.0f, 1.0f);
			}
			ENDCG
		}
	}
}
