Shader "LH/BillBoard2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _ViewUpDir;
			float4 _ViewRightDir;
			
			v2f vert (appdata v)
			{
				v2f o;
				float3 rightDir = normalize(UnityWorldToObjectDir(_ViewRightDir.xyz));
				float3 upDir = float3(0.0, 1.0, 0.0);
				float3 viewDir = normalize(cross(upDir, rightDir));
				upDir = normalize(cross(rightDir, viewDir));
				//float3 upDir = normalize(UnityWorldToObjectDir(_ViewUpDir.xyz));
				//float3 viewDir = normalize(cross(upDir, rightDir));			
				v.vertex.xyz = v.vertex.x * rightDir + v.vertex.y * upDir + v.vertex.z * viewDir;
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
	}
}
