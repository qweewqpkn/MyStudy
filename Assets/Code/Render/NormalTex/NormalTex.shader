Shader "LH/NormalTex"
{
	Properties
	{
		_MainTex("_MainTetx", 2D) = "white"{}
		_NormalTex("_NormalTex", 2D) = "white"{}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		struct a2v
		{
			float4 pos : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 viewDir : TEXCOORD1;
			float3 lightDir : TEXCOORD2;
		};

		sampler2D _MainTex;
		sampler2D _NormalTex;

		v2f vert(a2v i)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(i.pos);
			
			float3 normal = normalize(i.normal);
			float4 tangent = normalize(i.tangent);
			float3 binormal = normalize(cross(normal, tangent.xyz) * tangent.w);
			//float3x3是按行构造的矩阵, 
			float3x3 objectToTangentMatrix = float3x3(tangent.xyz, binormal, normal);
			o.viewDir = mul(objectToTangentMatrix, ObjSpaceViewDir(i.pos));
			o.lightDir = mul(objectToTangentMatrix, ObjSpaceLightDir(i.pos));
			o.uv = i.uv;

			return o;
		}

		float4 frag(v2f i) : SV_Target
		{
			float3 viewDir = normalize(i.viewDir);
			float3 lightDir = normalize(i.lightDir);


			float3 normal = tex2D(_NormalTex, i.uv).xyz;
			normal.xyz = normal.xyz * 2 - 1.0f;
			//normal.z = 1 - saturate(dot(normal.xy, normal.xy)); 

			float4 texColor = tex2D(_MainTex, i.uv);
			float4 diffuse = _LightColor0 * texColor * (dot(normalize(normal), normalize(lightDir)) * 0.5f + 0.5f);
			float3 reflectDir = normalize(reflect(-lightDir, normal));
			float4 specular = _LightColor0 * texColor * pow(dot(viewDir, reflectDir), 8);

			float4 color = diffuse + specular;
			color.a = 1.0f;
			return color;
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