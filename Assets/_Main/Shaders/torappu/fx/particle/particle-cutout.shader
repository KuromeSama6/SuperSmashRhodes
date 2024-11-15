Shader "Torappu/Particles/Cutout" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Cutout ("Alpha CutOut", Range(0, 1)) = 0.5
		[Enum(One, 1, SrcAlpha, 5)] _SrcBlend ("SrcBlend", Float) = 5
		[Enum(One, 1, OneMinusSrcAlpha, 10)] _DstBlend ("DstBlend", Float) = 10
		[Enum(Off, 0, On, 4)] _ZTest ("ZTest", Float) = 4
		[Enum(Off, 0, On, 4)] _ZWrite ("ZWrite", Float) = 0
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