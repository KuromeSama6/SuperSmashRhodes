Shader "Torappu/UI/Disturb/DisturbDissolve-CustomDirection" {
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
		[Enum(Alpha,10,Additive,1)] _Dst ("AdditiveMode", Float) = 10
		[Enum(On,0,Off,1)] _RgbCtrl ("RgbCtrl", Float) = 1
		_MainIntensity ("MainIntensity", Float) = 1
		_MainColor ("MainColor", Vector) = (1,1,1,1)
		_ParticleTex ("ParticleTex", 2D) = "white" {}
		_DisturbTex ("DisturbTex", 2D) = "black" {}
		_DisturbMask ("DisturbMask", 2D) = "white" {}
		_DisturbIntensity ("DisturbIntensity(R)", Float) = 0
		[Enum(UV,0,UorV,1)] _DisturbModeCtrl ("DisturbMode", Float) = 0
		[Enum(U,0,V,1)] _DirectionCtrl ("DisturbDirection", Float) = 0
		_DisturbUSpeed ("DisturbSpeed_U", Float) = 0
		_DisturbVSpeed ("DisturbSpeed_V", Float) = 0
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_DissolveMask ("DissolveMask", 2D) = "white" {}
		_DissolveIntensity ("DissolveIntensity (G)", Range(0, 1)) = 0
		_DissolveMaskRotate ("DissolveMaskRotate", Range(0, 360)) = 0
		_DissolveSoftness ("DissolveSoftness", Range(0, 0.5)) = 0
		_DissolveSpeedU ("DissolveSpeed_U", Float) = 0
		_DissolveSpeedV ("DissolveSpeed_V", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
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