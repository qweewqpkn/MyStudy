// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Water_Distortion"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_NoiseTex ("_NoiseTex", 2D) = "white" {}
		_MainTex ("_MainTex", 2D) = "white" {}
		_ReflectTex("_ReflectTex", 2D) = "white"{}
		_HeatTime  ("_HeatTime", range (-1,1)) = 0
		_ForceX  ("_ForceX", range (0,0.1)) = 0.1
		_ForceY  ("_ForceY", range (0,0.1)) = 0.1
		_ReflectRadio("_ReflectRadio", range(0,1)) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
	
		Lighting Off 
		ZWrite On 

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			#include "UnityCG.cginc"			

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};			

			struct v2f {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};			

			fixed4 _TintColor;
			fixed _ForceX;
			fixed _ForceY;
			fixed _HeatTime;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			sampler2D _ReflectTex;
			sampler2D _MainTex;		
			float _ReflectRadio;	

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}			
 
			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 offsetColor1 = tex2D(_NoiseTex, frac(i.uv + _Time.xy*_HeatTime));
			    fixed4 offsetColor2 = tex2D(_NoiseTex, frac(i.uv + _Time.yx*_HeatTime));
				fixed offsetX= (offsetColor1.r) * _ForceX;
				fixed offsetY= (offsetColor1.r + offsetColor2.r) * _ForceY;

				fixed4 reflectColor = tex2D(_ReflectTex, saturate(i.screenPos.xy / i.screenPos.w + fixed2(offsetX, offsetY))); 
				//reflectColor *= tex2D(_ReflectTex, saturate(i.screenPos.xy / i.screenPos.w + 2 * fixed2(offsetX, offsetY))); 
				fixed4 texColor = tex2D(_MainTex, i.uv + float2(offsetX, 0));
				fixed4 finalColor = lerp(texColor, reflectColor, _ReflectRadio);
				return  fixed4(_TintColor.rgb * finalColor.rgb, _TintColor.a);
			}
			ENDCG
		}
	}
}
