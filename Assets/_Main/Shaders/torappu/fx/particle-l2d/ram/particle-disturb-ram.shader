Shader "Torappu/Particles-L2D/Ram/Disturb(CustomData)" {
	Properties {
		[Toggle] _HGCustomVertexStream ("Use Custom Data", Float) = 0
		_MainColor ("MainCol", Vector) = (0.5,0.5,0.5,0.5)
		_Opacity ("Opacity", Float) = 1
		_MainTex ("Maintex", 2D) = "white" {}
		_MainUSpeed ("Main U Speed", Float) = 0
		_MainVSpeed ("Main V Speed", Float) = 0
		[Enum(Off, 0, On, 4)] _ZTest ("ZTest", Float) = 4
		[Enum(Add, 1, AlphaBlend, 10)] _DstBlend ("Alpha Blend Mode", Float) = 1
		[Enum(Off, 0, Front, 1, Back, 2)] _CullMode ("Cull Mode", Float) = 0
		[Header(Dissolve)] _BorderWidth ("Border Width", Range(0.0001, 1)) = 0.1
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_Amount ("Amount", Range(0, 1)) = 0.5
		_DissolveUSpeed ("Dissolve U Speed", Float) = 0
		_DissolveVSpeed ("Dissolve V Speed", Float) = 0
		[Header(Ram)] _RamTex ("Ram Texture", 2D) = "white" {}
		[Header(Disturb)] [Toggle] _DisturbInfluenceDissolveUV ("Disturb influence Dissolve(UV)", Float) = 0
		_DisturbInfluenceMainUV ("Disturb influence Main(UV)", Range(0, 1)) = 1
		_DisturbTex ("Disturb Texture", 2D) = "black" {}
		_IntensityU ("Disturb Intensity U", Range(0, 2)) = 0
		_IntensityV ("Disturb intensity V", Range(0, 2)) = 0
		_DisturbUSpeed ("Disturb U Speed", Float) = 0
		_DisturbVSpeed ("Disturb V Speed", Float) = 0
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