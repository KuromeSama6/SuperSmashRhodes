Shader "Torappu/Furni/SpecialEffect/Furniture_Water" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Water Tex (r: flow,g:caustic, b: mask)", 2D) = "white" {}
		_DistortStrength ("Wave Strength", Range(0, 0.3)) = 0.1
		_DistortSpeedX ("Wave SpeedX", Range(-3, 3)) = 0.5
		_DistortSpeedY ("Wave SpeedY", Range(-3, 3)) = 0.5
		_ReflColor ("Reflect Color (a: Strength)", Vector) = (1,1,1,1)
		_RefIntenisty ("水波表面扰动强度", Range(0, 1)) = 0.005
		_RefFrensnel ("水面反射角度", Range(1, 8)) = 2
		[NoScaleOffset] _ReflCube ("Env Cube", Cube) = "_Skybox" {}
		_ZWrite ("ZWrite", Float) = 1
		_Caustics ("焦散[Feature]", Float) = 1
		_Caustics_Tex ("焦散贴图[PartOf(_Caustics)]", 2D) = "black" {}
		_Caustics_Brightness ("焦散亮度[PartOf(_Caustics)]", Range(0, 4)) = 1
		_Caustics_Speed ("焦散速度[PartOf(_Caustics)]", Range(0, 1)) = 1
		_Caustics_FlowSpeed ("焦散流光速度(负数表示反方向)[PartOf(_Caustics)]", Range(-4, 4)) = 2
		[MyToggle] _Caustics_EnableDispersion ("开启色散[PartOf(_Caustics)]", Range(0, 1)) = 0
		_Caustics_Dispersion ("色散强度[PartOf(_Caustics_EnableDispersion)]", Range(0, 0.01)) = 0.01
		_Caustics_Parallax ("视差映射强度(模型需要导出切线)[PartOf(_Caustics)]", Range(0, 1)) = 0
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
	//CustomEditor "HGCustomShaderGUI"
}