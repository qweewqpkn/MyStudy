Shader"LH/ShowRT2UI"
{
	Properties
	{	
		_MainTex("_MainTex", 2D) = "white"{}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct a2v
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v i) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.pos);
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