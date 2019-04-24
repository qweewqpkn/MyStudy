Shader "LH/BillBoard"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 viewPos = mul(unity_WorldToObject, _WorldSpaceCameraPos);
				float3 viewDir = normalize(viewPos.xyz - float3(0.0f, 0.0f, 0.0f));
				float3 upDir = float3(0.0f, 1.0f, 0.0f);
				float3 rightDir = normalize(cross(upDir, viewDir));
				upDir = normalize(cross(viewDir, rightDir));
				float3 pos = rightDir * v.vertex.x + upDir * v.vertex.y + viewDir * v.vertex.z;
				o.vertex = UnityObjectToClipPos(float4(pos, v.vertex.w));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 texColor = tex2D(_MainTex, i.uv);
				return texColor;
			}
			ENDCG
		}
	}
}
