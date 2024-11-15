Shader "Torappu/Particles-L2D/3D/MatCapTransparent" {
	Properties {
		_Color ("Main Color", Vector) = (0.5,0.5,0.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		[Header(Dissolve)] _DissolveTex ("DissolveTex", 2D) = "white" {}
		_Amount ("Amount", Range(0, 1)) = 0.5
		_BorderWidth ("Border Width", Range(0.0001, 1)) = 0.1
		[Header(MapCap)] [NoScaleOffset] _MatCap ("MatCap (RGB)", 2D) = "white" {}
		[Header(HighLight)] [NoScaleOffset] _MatCapHighLight ("MatCap (r)", 2D) = "black" {}
		_HighLightStrength ("Highlight Strength", Range(0, 1)) = 1
		_FresnelPow ("Fresnel Pow", Range(1, 5)) = 3
		_CtrlParams ("Ctrl (Smoothstep : x<y[fresnel], z<w[hL])", Vector) = (0,1,0,1)
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