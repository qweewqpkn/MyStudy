Shader "Custom/HeatWave"
{
	Properties
	{
		_DisTex("扰动贴图", 2D) = "white" {}
		_HeatSpeed("扰动速度", Float) = 1.0
		_HeatScale("扰动强度", Float) = 1.0
	}

	SubShader
	{
		Tags {"Queue" = "Transparent"}

		GrabPass
		{
			"_BackgroundTex"
		}

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
			};

			sampler2D _DisTex;
			sampler2D _BackgroundTex;
			float _HeatSpeed;
			float _HeatScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 disCol = tex2D(_DisTex, i.uv + frac(_Time.xy * _HeatSpeed)) * _HeatScale;
				fixed4 bgCol = tex2Dproj(_BackgroundTex, i.grabPos + fixed4(disCol.xy, 0.0f, 0.0f));
				return bgCol;
			}
			ENDCG
		}
	}
}
