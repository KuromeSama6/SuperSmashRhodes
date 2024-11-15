Shader "Torappu/UI/PatternDotWithBackground" {
	Properties {
		[KeywordEnum(Default, Outer)] _HGColorMode ("Color mode", Float) = 0
		_UVScale ("Scale", Range(0.01, 100)) = 10
		_DotCtrl ("Dot Ctrl", Range(0, 1)) = 0.8
		_DotAlpha ("Dot alpha", Range(0, 1)) = 0.5
		_BackgroundAlpha ("Background alpha", Range(0, 1)) = 0.05
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
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