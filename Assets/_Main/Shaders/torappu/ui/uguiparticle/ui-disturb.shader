Shader "Torappu/UI/Disturb(Origin)" {
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_DisturTex ("Disturb Texture", 2D) = "black" {}
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,1)
		_IntensityU ("Disturb Intensity U", Range(0, 2)) = 0
		_IntensityV ("Disturb intensity V", Range(0, 2)) = 0
		[Enum(One, 1, SrcAlpha, 5)] _SrcBlend ("SrcBlend", Float) = 1
		[Enum(One, 1, OneMinusSrcAlpha, 10)] _DstBlend ("DstBlend", Float) = 1
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}