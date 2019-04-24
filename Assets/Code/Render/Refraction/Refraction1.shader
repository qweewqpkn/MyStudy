Shader "LH/Refraction1"
{
	Properties
	{
		_NoiseTex("_NoiseTex", 2D) = "white"
		_NoiseScale("_NoiseScale", Float) = 1

	}
	SubShader
	{
		Tags 
		{ 
			"Queue" = "Transparent"
		}
		LOD 100

		GrabPass { "_GrabTex" }

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
				float4 screenPos : TEXCOORD1;
				float4 vertex : SV_POSITION; 
			};

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _GrabTex;
			float _NoiseScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.screenPos = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 noiseColor = tex2D(_NoiseTex, i.uv + _Time.y);
				float2 uv = i.screenPos.xy / i.screenPos.w;
				fixed4 grabColor = tex2D(_GrabTex, uv + noiseColor.r * 0.1);

				return grabColor;
			}
			ENDCG
		}
	}
}
