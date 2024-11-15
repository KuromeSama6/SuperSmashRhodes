Shader "Torappu/PostEffect/Grayscale" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Params ("Luminance (RGB) Amount (A)", Vector) = (0.3,0.59,0.11,1)
		_Inverse ("ColorInverse (0 disabled)", Float) = 0
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