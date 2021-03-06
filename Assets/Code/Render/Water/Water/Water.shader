﻿Shader "LH/Water"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_MaskTex("_MaskTex(r:specular mask)", 2D) = "white"{}
		_NoiseTex("_NoiseTex", 2D) = "white"{}
		_BumpTex("_BumpTex", 2D) = "white"{}
		_Color("_Color", Color) = (1,1,1,1)
		_DepthColor1("_DepthColor1", Color) = (1, 1, 1, 1)
		_DepthColor2("_DepthColor2", Color) = (1, 1, 1, 1)
		_Gloss("_Gloss", Float) = 8
		_SpecularStrength("_SpecularStrength", Float) = 1
		_FresnelStrength("_FresnelStrength", Range(0, 1)) = 1
		_NormalSpeed("_NormalSpeed", Float) = 1
		_NormalOffset("_NormalOffset", Range(0, 1)) = 0.5
		_Range("_Range(r:深度范围, g:海岸范围)", Float) = 8

		[Header(distortion)]
		_DistTime("_DistTime", Range(-0.1, 0.1)) = 0.1
		_OffsetScaleX("_OffsetScaleX", Range(0, 1)) = 0.1
		_OffsetScaleY("_OffsetScaleY", Range(0, 1)) = 0.1

		[Header(Sin Wave)]
		_WaveAmplitude("_WaveAmplitude(x:波1振幅,y:波2振幅,z:波3振幅,w:波4振幅)", Vector) = (0, 0, 0, 0)
		_WaveSpeed("_WaveSpeed(x:波1速度,y:波2速度,z:波3速度,w:波4速度)", Vector) = (1, 1, 1, 1)
		_WaveLength("_WaveLength(x:波1长度,y:波2长度,z:波3长度,w:波4长度)", Float) = (2, 2, 2, 2)
		_WaveDirection1("_WaveDirection1", Vector) = (1, 0, 0, 0)
		_WaveDirection2("_WaveDirection2", Vector) = (1, 0, 0, 0)
		_WaveDirection3("_WaveDirection3", Vector) = (1, 0, 0, 0)
		_WaveDirection4("_WaveDirection4", Vector) = (1, 0, 0, 0)
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
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float2 uvMask : TEXCOORD3;
				float4 Tangent2World1 : TEXCOORD4;
				float4 Tangent2World2 : TEXCOORD5;
				float4 Tangent2World3 : TEXCOORD6;
				float3 normal : TEXCOORD7;
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _NoiseTex;
			sampler2D _ReflectTex;
			sampler2D _RefractTex;
			sampler2D _BumpTex;
			sampler2D _CameraDepthTexture;
			float4 _MaskTex_ST;
			float4 _MainTex_ST;
			float4 _BumpTex_ST;
			float4x4 _InverViewProjMatrix;

			float _Gloss;
			float _SpecularStrength;
			float _FresnelStrength;
			float4 _Color;
			float4 _DepthColor1;
			float4 _DepthColor2;
			float _OffsetScaleX;
			float _OffsetScaleY;
			float _DistTime;
			float _NormalOffset;
			float _Range;
			float _NormalSpeed;

			float4 _WaveAmplitude;
			float4 _WaveSpeed;
			float4 _WaveLength;
			float4 _WaveDirection1;
			float4 _WaveDirection2;
			float4 _WaveDirection3;
			float4 _WaveDirection4;

			float SinWaveHeightOne(float4 vertex, float waveLength, float waveSpeed, float waveAmplitude, float4 waveDirection)
			{
				//公式：A*sin(dot(D, (x,z))*w+t*b)
				float w = 2 * 3.14 / waveLength;
				float phase = waveSpeed * w;
				float height = waveAmplitude * sin(dot(waveDirection.xz, vertex.xz) * w + _Time.x * phase);
				return height;
			}

			float SinWaveHeight(float4 vertex)
			{
				float height = 0;
				height += SinWaveHeightOne(vertex, _WaveLength.x, _WaveSpeed.x, _WaveAmplitude.x, _WaveDirection1);
				height += SinWaveHeightOne(vertex, _WaveLength.y, _WaveSpeed.y, _WaveAmplitude.y, _WaveDirection2);
				height += SinWaveHeightOne(vertex, _WaveLength.z, _WaveSpeed.z, _WaveAmplitude.z, _WaveDirection3);
				height += SinWaveHeightOne(vertex, _WaveLength.w, _WaveSpeed.w, _WaveAmplitude.w, _WaveDirection4);
				return height;
			}

			//求单个波法线
			float3 SinWaveNormalOne(float4 vertex, float waveLength, float waveSpeed, float waveAmplitude, float4 waveDirection)
			{
				float w = 2 * 3.14 / waveLength;
				float phase = waveSpeed * w;
				float partialX = waveAmplitude * waveDirection.x * w * cos(dot(waveDirection.xz, vertex.xz) * w + _Time.x * phase);
				float partialZ = waveAmplitude * waveDirection.z * w * cos(dot(waveDirection.xz, vertex.xz) * w + _Time.x * phase);
				return float3(-partialX, -partialZ, 0);
			}

			//求所有波形叠加后的法线
			float3 SinWaveNormal(float4 vertex)
			{
				float3 normal = float3(0, 0, 1);
				normal += SinWaveNormalOne(vertex, _WaveLength.x, _WaveSpeed.x, _WaveAmplitude.x, _WaveDirection1);
				normal += SinWaveNormalOne(vertex, _WaveLength.y, _WaveSpeed.y, _WaveAmplitude.y, _WaveDirection2);
				normal += SinWaveNormalOne(vertex, _WaveLength.z, _WaveSpeed.z, _WaveAmplitude.z, _WaveDirection3);
				normal += SinWaveNormalOne(vertex, _WaveLength.w, _WaveSpeed.w, _WaveAmplitude.w, _WaveDirection4);
				return normal;
			}

			//求单个波切线
			float3 SinWaveTangentOne(float4 vertex, float waveLength, float waveSpeed, float waveAmplitude, float4 waveDirection)
			{
				float w = 2 * 3.14 / waveLength;
				float phase = waveSpeed * w;
				float partialZ = waveAmplitude * waveDirection.z * w * cos(dot(waveDirection.xz, vertex.xz) * w + _Time.x * phase);
				return float3(0, 0, partialZ);
			}

			//求所有波形叠加后的切线
			float3 SinWaveTangent(float4 vertex)
			{
				float3 tangent = float3(0, 1, 0);
				tangent += SinWaveTangentOne(vertex, _WaveLength.x, _WaveSpeed.x, _WaveAmplitude.x, _WaveDirection1);
				tangent += SinWaveTangentOne(vertex, _WaveLength.y, _WaveSpeed.y, _WaveAmplitude.y, _WaveDirection2);
				tangent += SinWaveTangentOne(vertex, _WaveLength.z, _WaveSpeed.z, _WaveAmplitude.z, _WaveDirection3);
				tangent += SinWaveTangentOne(vertex, _WaveLength.w, _WaveSpeed.w, _WaveAmplitude.w, _WaveDirection4);
				return tangent;
			}

			float4 GetWorldPosFromDepth(float4x4 InverVP, float depth, float2 screenPos)
			{
				float4 ndcPos = float4(screenPos.x * 2 - 1, screenPos.y * 2 - 1, 2 *(1 - depth) - 1, 1);
				//#if defined(UNITY_REVERSED_Z)
				//	//D3d with reversed Z
				//	ndcPos.z = 1.0f - ndcPos.z;
				//#elif UNITY_UV_STARTS_AT_TOP
				//	//D3d without reversed z
				//#else
				//	//opengl, map to -1,1
				//	ndcPos.z = ndcPos.z * 2.0f - 1.0f;
				//#endif

				float4 worldPos = mul(InverVP, ndcPos);
				worldPos = worldPos / worldPos.w;
				return worldPos;
			}

			v2f vert (appdata v)
			{
				v2f o;
				//最多叠加4重sin波
				v.vertex.y += SinWaveHeight(v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv0.zw = TRANSFORM_TEX(v.uv, _MaskTex);
				o.uv1.xy = TRANSFORM_TEX(v.uv, _BumpTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 normal = UnityObjectToWorldNormal(SinWaveNormal(v.vertex));
				float3 tangent = UnityObjectToWorldDir(SinWaveTangent(v.vertex));
				float3 binormal = cross(normal, tangent);

				o.Tangent2World1 = float4(binormal.x, tangent.x, normal.x, worldPos.x);
				o.Tangent2World2 = float4(binormal.y, tangent.y, normal.y, worldPos.y);
				o.Tangent2World3 = float4(binormal.z, tangent.z, normal.z, worldPos.z);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 screenPos =  i.screenPos.xy / i.screenPos.w;
				float depth =  SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenPos);
				//根据深度获取世界坐标
				float4 worldPosFromDepth = GetWorldPosFromDepth(_InverViewProjMatrix, depth, screenPos);

				//世界坐标
				float4 worldPos = float4(i.Tangent2World1.w, i.Tangent2World2.w, i.Tangent2World3.w, 1.0);

				//扰动
				fixed3 noiseColor1 = tex2D(_NoiseTex, i.uv0.xy + frac(_Time.xy * _DistTime));
				fixed3 noiseColor2 = tex2D(_NoiseTex, i.uv0.xy + frac(_Time.yx * _DistTime));
				float offsetX = (noiseColor1.r + noiseColor2.r - 1) * _OffsetScaleX;
				float offsetY = (noiseColor1.r + noiseColor2.r - 1) * _OffsetScaleY;
				float2 offset = float2(offsetX, offsetY);

				//获取法线
				float2 normalOffset = _WaveDirection1.xz * _NormalSpeed * _Time.x;
				fixed3 normal = UnpackNormal((tex2D(_BumpTex, i.uv1.xy + normalOffset)));
				normal = normalize(float3(dot(i.Tangent2World1, normal), dot(i.Tangent2World2, normal), dot(i.Tangent2World3, normal)));
				normal = lerp(i.normal, normal, _NormalOffset);
				float3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));

				//计算高光
				fixed3 maskColor = tex2D(_MaskTex, i.uv0.zw);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 H = normalize(lightDir + viewDir);
				fixed3 specular = _SpecularStrength * _LightColor0.rgb * pow(saturate(dot(H, normal)), _Gloss) * maskColor.r;

				//纹理
				fixed3 texColor = tex2D(_MainTex, i.uv0.xy).rgb;
				//渐变
				fixed3 gradientColor = lerp(_DepthColor1, _DepthColor2, (worldPos.y - worldPosFromDepth.y) / _Range.r);
				//反射
				fixed3 reflectColor = tex2D(_ReflectTex, screenPos + offset).rgb;
				//折射
				fixed3 refractColor = tex2D(_RefractTex, screenPos + offset).rgb;
				//fresnel
				fixed fresnel = _FresnelStrength + (1 -_FresnelStrength) * pow(1 - saturate(dot(viewDir, normal)), 5);
				//插值
				fixed3 finalColor = lerp(refractColor, reflectColor, fresnel);
				finalColor = finalColor * gradientColor + specular;
				return fixed4(specular,_Color.a);
			}
			ENDCG
		}
	}
}
