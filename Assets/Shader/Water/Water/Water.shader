Shader "LH/Water"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_MaskTex("_MaskTex(r:specular mask)", 2D) = "white"{}
		_NoiseTex("_NoiseTex", 2D) = "white"{}
		_BumpTex("_BumpTex", 2D) = "white"{}
		_Color("_Color", Color) = (1,1,1,1)
		_Gloss("_Gloss", Float) = 8
		_SpecularStrength("_SpecularStrength", Float) = 1
		_FresnelStrength("_FresnelStrength", Float) = 8

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
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _NoiseTex;
			sampler2D _ReflectTex;
			sampler2D _RefractTex;
			sampler2D _BumpTex;
			float4 _MaskTex_ST;
			float4 _MainTex_ST;
			float4 _BumpTex_ST;

			float _Gloss;
			float _SpecularStrength;
			float _FresnelStrength;
			float4 _Color;
			float _OffsetScaleX;
			float _OffsetScaleY;
			float _DistTime;

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

			//求sin波的法线
			float3 SinWaveNormalOne(float4 vertex, float waveLength, float waveSpeed, float waveAmplitude, float4 waveDirection)
			{
				float w = 2 * 3.14 / waveLength;
				float phase = waveSpeed * w;
				float partialX = waveAmplitude * waveDirection.x * w * cos(dot(waveDirection.xz, vertex.xz) * w + _Time.x * phase);
				float partialZ = waveAmplitude * waveDirection.z * w * cos(dot(waveDirection.xz, vertex.xz) * w + _Time.x * phase);
				return float3(-partialX, 0, -partialZ);
			}

			float3 SinWaveNormal(float4 vertex)
			{
				float3 normal = float3(0, 1, 0);
				normal += SinWaveNormalOne(vertex, _WaveLength.x, _WaveSpeed.x, _WaveAmplitude.x, _WaveDirection1);
				normal += SinWaveNormalOne(vertex, _WaveLength.y, _WaveSpeed.y, _WaveAmplitude.y, _WaveDirection2);
				normal += SinWaveNormalOne(vertex, _WaveLength.z, _WaveSpeed.z, _WaveAmplitude.z, _WaveDirection3);
				normal += SinWaveNormalOne(vertex, _WaveLength.w, _WaveSpeed.w, _WaveAmplitude.w, _WaveDirection4);
				return normal;
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
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 normal = UnityObjectToWorldNormal(SinWaveNormal(v.vertex));
				float3 tangent = UnityObjectToWorldDir(v.tangent);
				float3 binormal = cross(normal, tangent) * v.tangent.w;

				o.Tangent2World1 = float4(binormal.x, tangent.x, normal.x, worldPos.x);
				o.Tangent2World2 = float4(binormal.y, tangent.y, normal.y, worldPos.y);
				o.Tangent2World3 = float4(binormal.z, tangent.z, normal.z, worldPos.z);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//世界坐标
				float4 worldPos = float4(i.Tangent2World1.w, i.Tangent2World2.w, i.Tangent2World3.z, 1.0);

				//获取法线
				fixed3 normal = UnpackNormal(tex2D(_BumpTex, i.uv1.xy));
				normal = normalize(float3(dot(i.Tangent2World1, normal), dot(i.Tangent2World2, normal), dot(i.Tangent2World3, normal)));
				float3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));

				//计算diffuse
				fixed3 diffuse = _LightColor0.rgb * (dot(normal, lightDir) * 0.5 + 0.5);

				//计算高光
				fixed3 maskColor = tex2D(_MaskTex, i.uv0.zw);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 H = normalize(lightDir + viewDir);
				fixed3 specular = _SpecularStrength * _LightColor0.rgb * pow(saturate(dot(H, normal)), _Gloss) * maskColor.r;

				//扰动
				fixed3 noiseColor1 = tex2D(_NoiseTex, i.uv0.xy + frac(_Time.xy * _DistTime));
				fixed3 noiseColor2 = tex2D(_NoiseTex, i.uv0.xy + frac(_Time.yx * _DistTime));
				float offsetX = (noiseColor1.r + noiseColor2.r) * _OffsetScaleX;
				float offsetY = (noiseColor1.r + noiseColor2.r) * _OffsetScaleY;
				//纹理
				fixed3 texColor = tex2D(_MainTex, i.uv0.xy).rgb;
				//反射
				fixed3 reflectColor = tex2D(_ReflectTex, i.screenPos.xy / i.screenPos.w + float2(offsetX, offsetY)).rgb;
				//折射
				fixed3 refractColor = tex2D(_RefractTex, i.screenPos.xy / i.screenPos.w).rgb;
				//fresnel
				fixed fresnel = saturate(pow(1 - saturate(dot(viewDir, normal)), _FresnelStrength));
				//插值
				fixed3 finalColor =lerp(refractColor, reflectColor, fresnel) * texColor * _Color;
				return fixed4(finalColor, _Color.a);
			}
			ENDCG
		}
	}
}
