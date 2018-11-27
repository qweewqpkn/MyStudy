// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/GhostShadow"
{
	Properties
	{
		_color("_color", Color) = (0.0, 0.0, 0.0, 0.0)
		_freneselIntensity("freneselIntensity", float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IngoreProjector" = "True"
		}

		Pass
		{
			ZTest On
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct a2v
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
			};

			float4 _color;
			float _freneselIntensity;

			v2f vert(a2v i )
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.normal = mul(i.normal, (float3x3)unity_WorldToObject);
				o.viewDir = WorldSpaceViewDir(i.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float fresnel = 1 - dot(normalize(i.normal), normalize(i.viewDir));
				_color.a = _color.a * pow(fresnel, _freneselIntensity); 

				return _color;
			}

			ENDCG
		}
	}
}