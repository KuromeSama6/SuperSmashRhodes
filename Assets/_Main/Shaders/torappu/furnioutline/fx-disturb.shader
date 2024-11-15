Shader "Torappu/Furni/Fx Disturb" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainColorACtrl ("Main A atten Ctrl", Range(0, 1)) = 0
		[Header(UVTween)] _UVTween ("UV Tween Main(xy), DisturTex(zw)", Vector) = (0,0,0,0)
		[Toggle] _ToggleUseWeight ("Use Weight Tex", Float) = 0
		_WeightTex ("Weight Texture", 2D) = "white" {}
		[Header(Disturb1)] [Toggle] _ToggleUseDisturb ("Use Disturb", Float) = 0
		_DisturTex ("Disturb Texture", 2D) = "black" {}
		_DisturbParam ("Disturb Param(xy[0,2]:intensity, zw[0,1] :anchor)", Vector) = (0,0,0.5,0.5)
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