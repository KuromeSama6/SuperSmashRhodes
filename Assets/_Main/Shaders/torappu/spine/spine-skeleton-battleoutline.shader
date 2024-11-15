Shader "Torappu/Spine/Skeleton (Outline-Battle)" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0, 1)) = 0.1
		_Color ("Outline Color", Vector) = (1,0,0,1)
		[ToggleOff] _UseAlphaTex ("Use Alpha Texture", Float) = 1
		[NoScaleOffset] _MainTex ("Texture to blend", 2D) = "black" {}
		[NoScaleOffset] _AlphaTex ("Alpha Texture", 2D) = "black" {}
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