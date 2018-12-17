Shader "LH/Fog/Fog_Internel"
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
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

//Unity内置fog实现
//#define UNITY_FOG_COORDS_PACKED(idx, vectype) vectype fogCoord : TEXCOORD##idx;//

//#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
//	#define UNITY_FOG_COORDS(idx) UNITY_FOG_COORDS_PACKED(idx, float1)//

//	#if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
//		// mobile or SM2.0: calculate fog factor per-vertex
//		#define UNITY_TRANSFER_FOG(o,outpos) UNITY_CALC_FOG_FACTOR((outpos).z); o.fogCoord.x = unityFogFactor
//	#else
//		// SM3.0 and PC/console: calculate fog distance per-vertex, and fog factor per-pixel
//		#define UNITY_TRANSFER_FOG(o,outpos) o.fogCoord.x = (outpos).z
//	#endif
//#else
//	#define UNITY_FOG_COORDS(idx)
//	#define UNITY_TRANSFER_FOG(o,outpos)
//#endif//

//#define UNITY_FOG_LERP_COLOR(col,fogCol,fogFac) col.rgb = lerp((fogCol).rgb, (col).rgb, saturate(fogFac))//
//

//#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
//	#if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
//		// mobile or SM2.0: fog factor was already calculated per-vertex, so just lerp the color
//		#define UNITY_APPLY_FOG_COLOR(coord,col,fogCol) UNITY_FOG_LERP_COLOR(col,fogCol,coord.x)
//	#else
//		// SM3.0 and PC/console: calculate fog factor and lerp fog color
//		#define UNITY_APPLY_FOG_COLOR(coord,col,fogCol) UNITY_CALC_FOG_FACTOR(coord.x); UNITY_FOG_LERP_COLOR(col,fogCol,unityFogFactor)
//	#endif
//#else
//	#define UNITY_APPLY_FOG_COLOR(coord,col,fogCol)
//#endif