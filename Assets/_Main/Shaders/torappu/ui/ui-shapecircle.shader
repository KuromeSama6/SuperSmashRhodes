Shader "Torappu/UI/Shape/ShapeCircle" {
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
		_OutStep ("Out Step", Range(0, 1)) = 0.3850705
		_InStep ("In Step", Range(0, 1)) = 0.2772508
		_OutSmooth ("Out Smooth", Range(0.005, 0.5)) = 0.01
		_InSmooth ("In Smooth", Range(0.005, 0.5)) = 0.01
		[KeywordEnum(Tile,Rotate)] _AdditionMapMoveType ("AdditionMapMoveType", Float) = 0
		_AdditionMap ("AdditionMap", 2D) = "white" {}
		_AdditionPannerSpeed ("AdditionPannerSpeed", Vector) = (0.1,0.1,0,0)
		_AdditionRotateAnchorTime ("AdditionRotateAnchorTime", Vector) = (0.5,0.5,0.5,0)
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