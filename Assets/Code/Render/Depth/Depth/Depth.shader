// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LH/Depth"
{
	Properties
	{

	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct a2v
	{
		float4 pos : POSITION;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float depth : TEXCOORD0;
	};

	v2f vert(a2v i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		//得到的时非线性深度，离摄像机近的位置精度高，越远精度越低，因为我们更关心更近的物体的远近，更远的物体我们看不清，所以精度不用太高
		o.depth = o.pos.z / o.pos.w;
		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		//Linear01Depth相当于把非线性的深度值，转换回到了摄像机空间下的Z值然后除以了farPlane的值.
		//试试：Linear01Depth(i.depth)
		//i.depth 摄像机的位置深度为1
		float4 color = float4(i.depth, 0.0f, 0.0f, 1.0f);
		return  color;
	}

	ENDCG

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}