﻿Shader "LH/PlanarShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PlanePoint("_PlanePoint", Vector) = (0, 0, 0, 0)
		_PlaneNormal("_PlaneNormal", Vector) = (0, 0, 0, 0)
		[KeywordEnum(DirectionLight, PointLight)]_ShadowMode("_ShadowMode", Float) = 0
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
			//避免阴影超过了显示阴影的平面
			Stencil
			{
				Ref 1
				Comp Equal 
				Pass Keep
			}
			Tags
			{
				"LightMode"="ForwardBase"
			}

			//为了让阴影显示在平面之上 避免z-fighting
			//https://docs.unity3d.com/Manual/SL-CullAndDepth.html
			offset -1,-1
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _SHADOWMODE_DIRECTIONLIGHT _SHADOWMODE_POINTLIGHT

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
			float4 _LightPos;
			
			v2f vert (appdata v)
			{
				v2f o;

				#ifdef _SHADOWMODE_DIRECTIONLIGHT
					//方法1
					//https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
					//平面与直线的交点方程
					//float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
					//float3 lightDir = UnityWorldSpaceLightDir(worldPos);
					//float dist = dot(_PlanePoint.xyz - worldPos, _PlaneNormal) / dot(lightDir , _PlaneNormal);
					//float4 projectPos = float4(worldPos + lightDir * dist, 1.0f);
					//float4 objPos = mul(unity_WorldToObject, projectPos);
					//o.vertex = UnityObjectToClipPos(objPos);
					
					//方法2(根据角度sin cos)
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
					float3 lightDir = UnityWorldSpaceLightDir(worldPos);
					float4 projectPos = float4(0.0, 0.0, 0.0, 1.0);
					projectPos.x = worldPos.x - (worldPos.y / lightDir.y) * lightDir.x;
					projectPos.z = worldPos.z - (worldPos.y / lightDir.y) * lightDir.z;
					float4 objPos = mul(unity_WorldToObject, projectPos);
					o.vertex = UnityObjectToClipPos(objPos);

				#elif _SHADOWMODE_POINTLIGHT
					float3 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
					float3 lightWorldPos = _LightPos;
					float4 projectPos = float4(0.0, 0.0, 0.0, 1.0);
					projectPos.x = (vertexWorldPos.y * lightWorldPos.x - lightWorldPos.y * vertexWorldPos.x) / (vertexWorldPos.y - lightWorldPos.y);
					projectPos.z = (vertexWorldPos.y * lightWorldPos.z - lightWorldPos.y * vertexWorldPos.z) / (vertexWorldPos.y - lightWorldPos.y);
					o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, projectPos));
				#endif
			
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0.3f, 0.3f, 0.3f, 1.0f);
			}
			ENDCG
		}
	}
}
