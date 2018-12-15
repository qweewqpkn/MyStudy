// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Water_UV_Distortion"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_NoiseTex ("Distort Texture (RG)", 2D) = "white" {}
		_MainTex ("_MainTex", 2D) = "white" {}
		_ReflectTex("_ReflectTex", Cube) = "white"{}
		_HeatTime  ("Heat Time", range (-1,1)) = 0
		_ForceX  ("Strength X", range (0,1)) = 0.1
		_ForceY  ("Strength Y", range (0,1)) = 0.1
		_ReflectRadio("_ReflectRadio", range(0,1)) = 1
		_FinalStrength("_FinalStrength", Float) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent+400" "RenderType"="Transparent" }
		Blend SrcAlpha One
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
				float3 normal : NORMAL;
			};			

			struct v2f {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
				float3 normal : TEXCOORD3;
			};			

			fixed4 _TintColor;
			fixed _ForceX;
			fixed _ForceY;
			fixed _HeatTime;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			samplerCUBE _ReflectTex;
			sampler2D _MainTex;		
			float _ReflectRadio;	
			float _FinalStrength;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}			

			fixed4 frag( v2f i ) : COLOR
			{
				fixed3 normal = normalize(i.normal);
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 reflectDir = reflect(-viewDir, normal);
				fixed4 offsetColor1 = tex2D(_NoiseTex, frac(i.uv + _Time.xz*_HeatTime));
			    fixed4 offsetColor2 = tex2D(_NoiseTex, frac(i.uv + _Time.yx*_HeatTime));
				fixed offsetX= ((offsetColor1.r + offsetColor2.r) - 1) * _ForceX;
				fixed offsetY= ((offsetColor1.r + offsetColor2.r) - 1) * _ForceY;


				fixed4 reflectColor = texCUBE(_ReflectTex, reflectDir + fixed3(offsetX, offsetY, 0.0));
				fixed4 texColor = tex2D(_MainTex, i.uv + fixed2(offsetX, offsetY));
				return  _TintColor * texColor * _FinalStrength;
			}
			ENDCG
		}
	}
	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}
