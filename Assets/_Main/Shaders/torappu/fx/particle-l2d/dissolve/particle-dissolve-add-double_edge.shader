Shader "Torappu/Particles-L2D/Dissolve/Dissolve Add Double edge" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("MainTex", 2D) = "white" {}
		_DissolveTex_01 ("DissolveTex", 2D) = "white" {}
		_DissolveTex_02 ("DissolveTex", 2D) = "white" {}
		_Amount_01 ("Amount_01", Range(0, 1)) = 0.5
		_BorderWidth_01 ("Border Width_01", Range(0.0001, 1)) = 0.1
		_Amount_02 ("Amount_02", Range(0, 1)) = 0.5
		_BorderWidth_02 ("Border Width_02", Range(0.0001, 1)) = 0.1
		_Edgecolor ("Edge Color (a:edgeWidth)", Vector) = (0,0,0,0)
		_pow ("pow", Range(0.01, 1)) = 1
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