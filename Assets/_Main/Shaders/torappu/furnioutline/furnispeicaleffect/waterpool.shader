Shader "Torappu/Furni/SpecialEffect/WaterPool" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Water Tex (r: flow,g:caustic, b: mask)", 2D) = "white" {}
		_DistortStrength ("Wave Strength", Range(0, 0.3)) = 0.1
		_DistortSpeedX ("Wave SpeedX", Range(-3, 3)) = 0.5
		_DistortSpeedY ("Wave SpeedY", Range(-3, 3)) = 0.5
		_RefractStrength ("Refract Strength", Range(0, 1)) = 0.03
		_ReflColor ("Reflect Color (a: Strength)", Vector) = (1,1,1,1)
		[NoScaleOffset] _ReflCube ("Env Cube", Cube) = "_Skybox" {}
		_ZWrite ("ZWrite", Float) = 1
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