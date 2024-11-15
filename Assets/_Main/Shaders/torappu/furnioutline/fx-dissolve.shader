Shader "Torappu/Furni/Fx Dissolve" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainColorACtrl ("Main A atten Ctrl", Range(0, 1)) = 0
		[Header(UVTween)] _UVTween ("UV Tween Main(xy), Dissolve(zw)", Vector) = (0,0,0,0)
		_DissolveTex_01 ("DissolveTex", 2D) = "white" {}
		_Amount_01 ("Amount_01", Range(0, 1)) = 0.5
		_BorderWidth_01 ("Border Width_01", Range(0.0001, 1)) = 0.1
		_EdgeColor ("Edge Color (a:edgeWidth)", Vector) = (0,0,0,0)
		_Pow ("pow", Range(0.1, 1)) = 1
		[Header(SecondDissovle)] [Toggle] _ToggleUseDissolve_02 ("Use Dissolve 02", Float) = 0
		_DissolveTex_02 ("DissolveTex", 2D) = "white" {}
		_Amount_02 ("Amount_02", Range(0, 1)) = 0.5
		_BorderWidth_02 ("Border Width_02", Range(0.0001, 1)) = 0.1
		_UVTween_02 ("UV Tween  Dissolve 02(xy)", Vector) = (0,0,0,0)
		[Enum(Off, 0, On, 4)] _ZTest ("ZTest", Float) = 4
		[Enum(One, 1, SrcAlpha, 5)] _SrcBlend ("SrcBlend", Float) = 1
		[Enum(One, 1, OneMinusSrcAlpha, 10)] _DstBlend ("DstBlend", Float) = 1
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