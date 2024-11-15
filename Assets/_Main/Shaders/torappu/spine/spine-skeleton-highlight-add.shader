Shader "Torappu/Spine/L2D/Skeleton-Highlight-Add" {
	Properties {
		_Color ("Tint Color", Vector) = (1,1,1,1)
		[NoScaleOffset] _MainTex ("Texture to blend", 2D) = "black" {}
		[Toggle] _UseBleed ("Use Bleed", Float) = 0
		[Header(Highlight)] _HighlightColor ("Hightlight Color", Vector) = (1,1,1,1)
		_HighlightTintColor ("Highlight Tint Color", Vector) = (1,1,1,1)
		_HighlightTex ("Highlight Tex", 2D) = "black" {}
		_HighlightAmount ("Highlight Amount", Range(0, 5)) = 0
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