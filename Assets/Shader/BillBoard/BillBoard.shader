Shader "LH/BillBoard"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 viewPos = mul(unity_WorldToObject, _WorldSpaceCameraPos);
				float3 viewDir = normalize(viewPos.xyz - float3(0.0f, 0.0f, 0.0f));
				float3 upDir = float3(0.0f, 1.0f, 0.0f);
				float3 rightDir = normalize(cross(viewDir, upDir));
				upDir = normalize(cross(rightDir, viewDir));
				float3 pos = rightDir * v.vertex.x + upDir * v.vertex.y + viewDir * v.vertex.z;
				o.vertex = UnityObjectToClipPos(float4(pos, v.vertex.w));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(1.0f, 0.0f, 0.0f, 1.0f);
			}
			ENDCG
		}
	}
}
