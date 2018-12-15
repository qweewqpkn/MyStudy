Shader "LH/CubeMapReflection"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_ReflectTex("_ReflectTex", Cube) = "_Skybox"{}
		_MaskTex("_MaskTex", 2D) = "white"{}
		_Gloss("_Gloss", Float) = 8
		_LMStrength("_LMStrength", Float) = 1
		_ReflectStrength("_ReflectStrength", Float) = 1
		_Color("_Color", Color) = (1,1,1,1)
		_LerpRange("_LerpRange", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ LIGHTMAP_ON
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

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
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float3 normal : TEXCOORD2;
				#ifdef LIGHTMAP_ON
				float2 uvLM : TEXCOORD3;
				#endif
			};

			sampler2D _MainTex;
			samplerCUBE _ReflectTex;
			sampler2D _MaskTex;
			float _Gloss;
			float _SpecularStrength;
			float _LMStrength;
			float _ReflectStrength;
			float4 _Color;
			float _LerpRange;
			float4 _ForwardDir;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				#ifdef LIGHTMAP_ON
				o.uvLM = v.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.normal);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 H = normalize(viewDir + lightDir);
				fixed3 specular = _LightColor0 * pow(saturate(dot(H, normal)), _Gloss);

				fixed3 texColor = tex2D(_MainTex, i.uv);
				fixed3 maskColor = tex2D(_MaskTex, i.uv);
				fixed3 reflectDir = reflect(-viewDir, normal);
				fixed3 reflectColor = (specular + texCUBE(_ReflectTex, reflectDir).rgb) * _ReflectStrength;

				fixed3 lmColor = 0;
				#ifdef LIGHTMAP_ON
				lmColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM));
				#endif
				lmColor = lmColor * texColor * _LMStrength;
				//下面的插值是为了让反射区域受到插值的影响(maskColor来控制反射区域)
				fixed3 finalColor = lerp(lmColor, reflectColor, maskColor.r);
				finalColor = lerp(lmColor, finalColor, _LerpRange) * _Color.rgb;

				return fixed4(finalColor, _Color.a);
			}
			ENDCG
		}
	}
}
