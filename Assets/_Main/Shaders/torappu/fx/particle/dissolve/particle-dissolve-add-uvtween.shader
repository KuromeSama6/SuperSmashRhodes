Shader "Torappu/Particles/Dissolve/Dissolve Add UVTween" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("MainTex", 2D) = "white" {}
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_Amount ("Amount", Range(0, 1)) = 0.5
		_BorderWidth ("Border Width", Range(0.0001, 1)) = 0.1
		[Header(UVTween)] _UVTween ("UV Tween Main(xy), Dissolve(zw)", Vector) = (0,0,0,0)
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