Shader "Custom/HeatWave"
{
	Properties
	{
		_MainTex ("主贴图", 2D) = "white" {}
		_DisTex("扰动贴图", 2D) = "white" {}
		_HeatSpeed("扰动速度", Float) = 1.0
		_HeatScale("123", Float) = 1.0
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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
				float3 normal : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _DisTex;
			float4 _MainTex_ST;
			sampler2D _BackgroundTex;
			float _HeatSpeed;
			float _HeatScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 disCol = tex2D(_DisTex, i.uv + frac(_Time.yx * _HeatSpeed)) * _HeatScale;
				float3 normal = normalize(i.normal);
				fixed4 bgCol = tex2Dproj(_BackgroundTex, i.grabPos + float4(disCol.xy, 0.0f, 0.0f));
				return bgCol;
			}
			ENDCG
		}
	}
}
