Shader "Torappu/Particles-L2D/Disturb/Disturb2 (Add)" {
	Properties {
		[KeywordEnum(Default, Glow )] _DisturbMode ("Disturb Mode", Float) = 0
		_MainTex ("Main Texture", 2D) = "white" {}
		_DisturTex ("Disturb Texture R:noise1  G:noise2  B:opacity", 2D) = "black" {}
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,0.5)
		_GlowColor ("Glow Color(Glow Mode only)", Vector) = (0.5,0.5,0.5,0.5)
		_Noise1Param ("noise1 X:scale  Y:speed  Z:IntX  W:IntY", Vector) = (1,1,1,1)
		_Noise2Param ("noise2 X:scale  Y:speed  Z:IntX  W:IntY", Vector) = (1,1,1,1)
		[Enum(Off, 0, LEqual, 4)] _ZTest ("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode ("CullMode", Float) = 2
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