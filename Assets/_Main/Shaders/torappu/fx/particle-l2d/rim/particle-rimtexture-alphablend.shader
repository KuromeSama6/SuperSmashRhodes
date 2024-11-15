Shader "Torappu/Particles-L2D/Rim/Rim Texture (AlphaBlend)" {
	Properties {
		_TintColor ("Tint Color", Vector) = (1,0,0,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_RimPower ("Rim Power", Range(0.5, 10)) = 3
		[Enum(Off, 0, On, 4)] _ZTest ("ZTest", Float) = 4
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