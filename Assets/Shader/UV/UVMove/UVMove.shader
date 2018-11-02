Shader "LH/UVMove"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_FlashTex("_FlashTex", 2D) = "white"{}
		_SpeedX("_SpeedX", float) = 1.0
		_SpeedY("_SpeedY", float) = 1.0
		_ScalePos("_ScalePos", float) = 0.1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _FlashTex;
			float _SpeedX;
			float _SpeedY;
			float _ScalePos;

			v2f vert(a2v i)
			{
				v2f o = (v2f)0;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				o.worldPos = mul(unity_ObjectToWorld, i.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 mainTexColor = tex2D(_MainTex, i.uv);
				fixed4 flashTexColor = tex2D(_FlashTex, i.worldPos.xy * _ScalePos + float2(_SpeedX, _SpeedY) * _Time.y);

				return flashTexColor;
			}

			ENDCG
		}
	}
}