Shader "LH/ScreenPos"
{
	Properties
	{

	}

	SubShader
	{
		CGINCLUDE
		#include "UnityCG.cginc"
		struct a2v
		{
			float4 pos : POSITION;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 screenPos : TEXCOORD0;
			float4 screenPos1 : TEXCOORD1;
		};

		v2f vert(a2v i)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(i.pos);
			o.screenPos = ComputeScreenPos(o.pos);
			o.screenPos1 = o.pos / o.pos.w;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			//return float4(i.screenPos.x / i.screenPos.w, 0.0f, 0.0f, 1.0f);
			return float4(-i.screenPos1.y, 0.0f, 0.0f, 1.0f);
		}

		ENDCG


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}