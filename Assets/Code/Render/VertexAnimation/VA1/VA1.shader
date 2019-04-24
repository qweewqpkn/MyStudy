Shader "LH/VA1"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_Speed("_Speed", float) = 1.0
		_Range("_Range", float) = 1.0
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
			ZWrite On
			ZTest On
			Blend Off
			Cull Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;		
				float2 uv : TEXCOORD0;	
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Speed;
			float _Range;

			v2f vert(a2v i)
			{
				v2f o;
				o.pos.xyz = i.pos.xyz + normalize(i.normal) * abs(sin(_Time.y * _Speed)) * _Range;
				o.pos = UnityObjectToClipPos(o.pos);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 texColor = tex2D(_MainTex, i.uv);
				return texColor;
			}

			ENDCG
		}
	}
}
