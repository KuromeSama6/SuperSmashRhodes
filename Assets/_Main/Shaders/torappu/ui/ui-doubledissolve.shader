Shader "Torappu/UI/DoubleDissolve" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_MainTexConfig ("MainTex Tile:XY", Vector) = (1,1,0,0)
		_AmountA ("Amount A", Range(0, 1)) = 0.5
		_BorderWidthA ("BorderWidth A", Range(0.0001, 1)) = 1
		_AmountB ("Amount B", Range(0, 1)) = 0.5
		_BorderWidthB ("BorderWidth B", Range(0.0001, 1)) = 0.8043448
		_DissolveTillingSpeed ("DissolveTillingSpeed A:xy,B:zw", Vector) = (0,0,0,0)
		_DissolveMapA ("DissolveMapA", 2D) = "white" {}
		_DissolveMapB ("DissolveMapB", 2D) = "white" {}
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