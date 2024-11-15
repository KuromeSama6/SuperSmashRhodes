Shader "Torappu/UI/Roguelike/InkBleed" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[Toggle] _HG_UI_Anm ("UI Animation", Float) = 0
		_MaskTex ("Mask", 2D) = "white" {}
		_Ctrl ("ctrl", Range(0, 2)) = 0
		_MarginColor ("MarginColor", Vector) = (0.1,0.1,0.1,1)
		_MarginCtrl ("MarginCtrl(x,y: range(x<y), z:strength([0:1]))", Vector) = (0.2,0.5,1,0)
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