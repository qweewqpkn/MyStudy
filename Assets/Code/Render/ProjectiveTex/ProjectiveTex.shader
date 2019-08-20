Shader "LH/ProjectiveTex"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ProjTex ("_ProjTex", 2D) = "white" {}
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
				float4 projUV : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _ProjTex;
			float4 _MainTex_ST;
			float4x4 _ProjectMatrix;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.projUV = mul(_ProjectMatrix, mul(unity_ObjectToWorld, v.vertex));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				i.projUV = i.projUV / i.projUV.w;
				if(i.projUV.x < 0 || i.projUV.x > 1 || i.projUV.y < 0 || i.projUV.y > 1)
					return fixed4(1, 1, 1, 1);

				fixed4 projCol = tex2D(_ProjTex, i.projUV.xy);
				fixed4 col = tex2D(_ProjTex, i.uv);
				return projCol;
			}
			ENDCG
		}
	}
}
