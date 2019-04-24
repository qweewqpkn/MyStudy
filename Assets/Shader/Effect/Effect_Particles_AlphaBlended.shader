Shader "Effect/Particles_AlphaBlended" {
Properties {
_TintColor ("Tint Color", Color) = (1,1,1,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue" = "Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off Fog { Color (0,0,0,0) }
	ZWrite Off
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	
	SubShader {
		Pass {
		SetTexture [_MainTex] 
			{
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] 
			{
				combine texture * previous
			}
		}
	}
}
}
