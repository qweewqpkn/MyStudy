Shader "LH/Test"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MoveDir("_MoveDir", vector) = (0, 0, 0, 0)
		_MoveSpeed("_MoveSpeed", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			cull off

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
			float4 _MoveDir;
			float _MoveSpeed;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				if(v.uv.y > 0.9)
				{	
					worldPos = worldPos + abs(sin(_Time.x * _MoveSpeed)) * _MoveDir;
				}

				o.vertex = mul(UNITY_MATRIX_VP, worldPos);
				o.uv = v.uv;
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
