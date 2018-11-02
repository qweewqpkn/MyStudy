Shader "LH/LightMap"
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
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			
			#include "UnityCG.cginc" 

			struct appdata
			{    
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				#ifndef LIGHTMAP_OFF
				float2 uv1 : TEXCOORD1;
				#endif
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				#ifndef LIGHTMAP_OFF
				float2 uv1 : TEXCOORD1;
				#endif
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				#ifndef LIGHTMAP_OFF
				o.uv1 = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				#ifndef LIGHTMAP_OFF
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1));
				col.rgb *= lm;
				#endif

				return col;
			}
			ENDCG
		}
	}
}
