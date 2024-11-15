Shader "Torappu/Unlit/Water" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,1)
		_WaterTex ("Water Tex", 2D) = "white" {}
		_WaterColor ("Water Color", Vector) = (0.5,0.5,0.5,1)
		_WaterIntensity ("Water Intensity", Range(0, 10)) = 0
		_MaskTex ("Mask Tex", 2D) = "white" {}
		_WaveColor ("Wave Color", Vector) = (0.5,0.5,0.5,1)
		_WaveIntensity ("Wave Intensity", Range(0, 10)) = 1.452997
		_WaveScale ("Wave Scale", Float) = 10
		_WaveSpeed ("Wave Speed", Float) = 0.2
		_WaveDistortionScale ("Wave Distortion Scale", Float) = 1
		_WaveDistortionLerp ("Wave Distortion Lerp", Range(0, 1)) = 0.009916387
		_WaveDistortionSpeed ("Wave Distortion Speed", Float) = 1
		_FoamColor ("Foam Color", Vector) = (0.854902,0.9921569,1,1)
		_FoamIntensity ("Foam Intensity", Range(0, 2)) = 0.9059834
		_FoamScale ("Foam Scale", Float) = 10
		_FoamWidth ("Foam Width", Range(0, 10)) = 1.452997
		_FoamSpeed ("Foam Speed", Float) = 1
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