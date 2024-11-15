Shader "Torappu/UI/SandboxV2/SandboxV2Cloud" {
	Properties {
		[PerRendererData] _MainTex ("Mask Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("ColorMask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		[Space(20)] _CloudTex ("Cloud Texture", 2D) = "white" {}
		_CloudNoiseTex ("Cloud Noise Texture", 2D) = "white" {}
		_CloudSpeedX ("Cloud Speed X", Range(-0.5, 0.5)) = 0
		_CloudSpeedY ("Cloud Speed Y", Range(-0.5, 0.5)) = 0
		_MaskSize ("MaskSize", Float) = 5000
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}