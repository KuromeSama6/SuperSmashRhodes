Shader "Torappu/Particles-L2D/Particles-Poker-Reflect" {
	Properties {
		_PokerTex ("Poker Texture", 2D) = "white" {}
		_PokerColor ("Poker Color", Vector) = (0.5,0.5,0.5,1)
		_NumberTex ("Number Texture", 2D) = "white" {}
		_NumberMask ("Number Mask", 2D) = "white" {}
		_CubeMap ("Cube Map", Cube) = "_Skybox" {}
		_DiffuseIntensity ("Diffuse Intensity", Float) = 0
		_LightDir ("Light Direction", Vector) = (0,0,0,0)
		_NormalMap ("Normal Map", 2D) = "bump" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}