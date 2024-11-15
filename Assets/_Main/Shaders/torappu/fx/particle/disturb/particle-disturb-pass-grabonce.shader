Shader "Torappu/Particles/Disturb/Disturb GrabPassOnce" {
	Properties {
		_DisturTex ("Disturb Texture", 2D) = "white" {}
		_Intensity ("Disturb Intensity", Range(0, 2)) = 0
		_NoiseSpeed ("噪声流动速度(x : u方向速度  y : v方向速度)", Vector) = (0,0,0,0)
		[MyToggle] _ShowNoise ("显示噪声", Float) = 0
		[Enum(Directional,1,Center,2)] _DirMode ("扭曲模式", Float) = 1
		_DisturbDir ("扭曲方向(模型空间)[PartOf(_DirMode, 1)]", Vector) = (1,1,1,1)
		_DisturbCenter ("扭曲中心(模型空间)[PartOf(_DirMode, 2)]", Vector) = (0.5,0.5,1,1)
		_Mask ("遮罩[Feature]", Float) = 0
		_MaskTex ("遮罩贴图[PartOf(_Mask)]", 2D) = "black" {}
		_MaskIntensity ("遮罩强度[PartOf(_Mask)]", Range(0, 1)) = 1
		[MyToggle] _PolarCoord ("UV极坐标[PartOf(_Mask)]", Float) = 0
		_PolarRadius ("极坐标半径[PartOf(_PolarCoord)]", Range(0, 5)) = 1
		_MaskSpeed ("遮罩流动速度(x : u方向速度 y : v方向速度)[PartOf(_Mask)]", Vector) = (0,0,0,0)
		[MyToggle] _ShowMask ("显示遮罩[PartOf(_Mask)]", Float) = 0
		[Toggle] _CUSTOMDATA ("粒子系统自定义参数", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	//CustomEditor "HGCustomShaderGUI"
}