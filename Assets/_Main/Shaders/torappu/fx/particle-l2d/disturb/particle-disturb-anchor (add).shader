Shader "Torappu/Particles-L2D/Disturb/Disturb Anchor (Add)" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		[Header(UVTween)] _UVTween ("UV Tween Main(xy), DisturTex(zw)", Vector) = (0,0,0,0)
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainColorACtrl ("Main A atten Ctrl", Range(0, 1)) = 1
		[Toggle] _Weight ("Use Weight Tex", Float) = 0
		_WeightTex ("Weight Texture", 2D) = "white" {}
		[Header(Disturb1)] _DisturTex ("Disturb Texture", 2D) = "black" {}
		_IntensityU ("Disturb Intensity U", Range(0, 2)) = 0
		_AnchorU ("Disturb U center", Range(0, 1)) = 0.5
		_IntensityV ("Disturb intensity V", Range(0, 2)) = 0
		_AnchorV ("Disturb V center", Range(0, 1)) = 0.5
		[Header(Disturb2)] [Toggle] _ToggleUseDisturb2 ("Use disturb 2", Float) = 0
		_DisturTex_02 ("Disturb Texture_02", 2D) = "black" {}
		_IntensityU_02 ("Disturb Intensity U", Range(0, 2)) = 0
		_AnchorU_02 ("Disturb U center", Range(0, 1)) = 0.5
		_IntensityV_02 ("Disturb intensity V", Range(0, 2)) = 0
		_AnchorV_02 ("Disturb V center", Range(0, 1)) = 0.5
		[Header(Dissolve)] [Toggle] _ToggleUseDissolve ("Use Dissolve", Float) = 0
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_Amount ("Amount", Range(0, 1)) = 0.5
		_BorderWidth ("Border Width", Range(0.0001, 1)) = 0.1
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