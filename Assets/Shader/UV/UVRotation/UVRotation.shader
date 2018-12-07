Shader "LH/UVRotation"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white"{}
		_RotFactor("_RotFactor", Range(0,10)) = 0.1
		_DistortCenter("_dstFactor", Vector) = (0.5, 0.5, 0.0, 0.0)
	}

	SubShader
	{
		Tags
		{

		}

		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

     		sampler2D _MainTex;  
     		sampler2D _NoiseTex;  
     		float _RotFactor;   //扭曲强度  
     		float4 _DistortCenter;  //扭曲中心点xy值（0-1）屏幕空间  

			v2f vert(a2v i)
			{	
				v2f o = (v2f)0;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;
				return o; 
			}

			fixed4 frag(v2f i) : SV_Target
			{
        		//平移坐标点到中心点,同时也是当前像素点到中心的方向  
        		fixed2 dir = i.uv - _DistortCenter.xy;  
        		//计算旋转的角度：对于像素点来说，距离中心越远，旋转越少，所以除以距离。相当于用DistortFactor作为旋转的角度值Distort/180 * π，π/180 = 0.1745  
        		float rot = _RotFactor * 0.1745 / (length(dir) + 0.001);//+0.001防止除零  
        		//计算sin值与cos值，构建旋转矩阵  
        		fixed sinval, cosval;  
        		sincos(rot, sinval, cosval);  
        		//uv坐标是顺时针旋转，但是图像是反时针旋转的，因为原uv旋转到新的uv，所以采样后，会将新uv的点放到当前点
        		float2x2  rotmatrix = float2x2(cosval, sinval, -sinval, cosval);  
        		//旋转  
        		dir = mul(rotmatrix, dir);  
        		//再平移回原位置  
        		dir += _DistortCenter.xy;   
        		//用偏移过的uv+扰动采样MainTex  
        		return tex2D(_MainTex, dir); 
			}

			ENDCG
		}
	}
}