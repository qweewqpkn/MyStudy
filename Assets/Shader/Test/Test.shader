Shader "LH/Test"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LerpY("_LerpY", Float) = 0
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
			float _LerpY;
			
			v2f vert (appdata v)
			{
				v2f o;
				float3 cameraPos = mul(unity_WorldToObject, _WorldSpaceCameraPos); 
				float3 forward = float3(0.0f, 0.0f, 0.0f) -  cameraPos;
				forward.y = lerp(0.0f, forward.y, _LerpY);
				forward = normalize(forward);
				float3 up = abs(forward.y) > 0.99 ? float3(0.0f, 0.0f, 1.0f) : float3(0.0f, 1.0f, 0.0f);
				float3 right = normalize(cross(up, forward));
				up = normalize(cross(forward, right));
				v.vertex.xyz = v.vertex.x * right + v.vertex.y * up + v.vertex.z * forward;
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
