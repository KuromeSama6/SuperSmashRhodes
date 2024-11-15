Shader "Torappu/Spine/Skeleton-AlphaSplit (Z-Distorted)" {
	Properties {
		_Color ("Tint Color", Vector) = (1,1,1,1)
		_Cutoff ("Shadow alpha cutoff", Range(0, 1)) = 0.1
		_BaselineZ ("Baseline of Z axis", Float) = 0
		[NoScaleOffset] _MainTex ("Texture to blend", 2D) = "black" {}
		[ToggleOff] _UseAlphaTex ("Use Alpha Texture", Float) = 1
		[NoScaleOffset] _AlphaTex ("Alpha Texture", 2D) = "white" {}
		[Toggle] _UseBleed ("Use Bleed", Float) = 0
		[Toggle] _UseFixedFormula ("Use Fixed Formula", Float) = 0
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