Shader "LH/Water"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_MaskTex("_MaskTex(r:specular mask)", 2D) = "white"{}
		_NoiseTex("_NoiseTex", 2D) = "white"{}
		_Color("_Color", Color) = (1,1,1,1)
		_Gloss("_Gloss", Float) = 8
		_SpecularStrength("_SpecularStrength", Float) = 1
		_FresnelStrength("_FresnelStrength", Float) = 8
		_DistTime("_DistTime", Range(-0.1, 0.1)) = 0.1
		_OffsetScaleX("_OffsetScaleX", Range(0, 1)) = 0.1
		_OffsetScaleY("_OffsetScaleY", Range(0, 1)) = 0.1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ LIGHTMAP_ON

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uvLM : TEXCOORD1;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uvMain : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
				float3 normal : TEXCOORD3;
				#ifdef LIGHTMAP_ON
				float2 uvLM : TEXCOORD4;
				#endif
				float2 uvMask : TEXCOORD5;
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _NoiseTex;
			sampler2D _ReflectTex;
			sampler2D _RefractTex;
			float4 _MaskTex_ST;
			float4 _MainTex_ST;
			float _Gloss;
			float _SpecularStrength;
			float _FresnelStrength;
			//float4 _LightPos;
			float4 _Color;
			float _OffsetScaleX;
			float _OffsetScaleY;
			float _DistTime;

			v2f vert (appdata v)
			{
				v2f o;

				//FFT
				

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvMask = TRANSFORM_TEX(v.uv, _MaskTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				#ifdef LIGHTMAP_ON
				o.uvLM = v.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//计算高光
				fixed3 maskColor = tex2D(_MaskTex, i.uvMask);
				fixed3 normal = normalize(i.normal);
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 H = normalize(lightDir + viewDir);
				fixed3 specular = _SpecularStrength * _LightColor0.rgb * pow(saturate(dot(H, normal)), _Gloss) * maskColor.r;

				//计算lightmap颜色
				fixed3 lmColor = 1;
				#ifdef LIGHTMAP_ON
				lmColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM));
				#endif

				//扰动
				fixed3 noiseColor1 = tex2D(_NoiseTex, i.uvMain + frac(_Time.xy * _DistTime));
				fixed3 noiseColor2 = tex2D(_NoiseTex, i.uvMain + frac(_Time.yx * _DistTime));
				float offsetX = (noiseColor1.r + noiseColor2.r) * _OffsetScaleX;
				float offsetY = (noiseColor1.r + noiseColor2.r) * _OffsetScaleY;
				//纹理
				fixed3 texColor = tex2D(_MainTex, i.uvMain).rgb;
				//反射
				fixed3 reflectColor = tex2D(_ReflectTex, i.screenPos.xy / i.screenPos.w + float2(offsetX, offsetY)).rgb;
				//折射
				fixed3 refractColor = tex2D(_RefractTex, i.screenPos.xy / i.screenPos.w).rgb;
				//fresnel
				fixed fresnel = saturate(pow(1 - saturate(dot(viewDir, normal)), _FresnelStrength));

				fixed3 finalColor = lerp(refractColor, reflectColor, fresnel) * texColor * lmColor * _Color + specular;
				return fixed4(finalColor, _Color.a);
			}
			ENDCG
		}
	}
}
