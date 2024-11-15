Shader "Torappu/Spine/L2D/Skeleton-AlphaSplit (Tint Black)" {
	Properties {
		_Color ("Tint Color", Vector) = (1,1,1,1)
		_Black ("Dark Color", Vector) = (0,0,0,0)
		[NoScaleOffset] _MainTex ("Texture to blend", 2D) = "black" {}
		[ToggleOff] _UseAlphaTex ("Use Alpha Texture", Float) = 1
		[NoScaleOffset] _AlphaTex ("Alpha Texture", 2D) = "white" {}
		[Toggle] _UseBleed ("Use Bleed", Float) = 0
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