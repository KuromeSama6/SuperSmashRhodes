Shader "Torappu/Unlit/Fire" {
	Properties {
		[HDR] _Color ("纹理颜色", Vector) = (1,1,1,1)
		_MainTex ("火焰纹理", 2D) = "white" {}
		[MyToggle] _Flipbook ("开启序列帧", Range(0, 1)) = 0
		[MyToggle] _TexAlpha ("A通道透明度(默认灰度表示透明度)", Range(0, 1)) = 0
		_FlipbookNum ("序列帧数量[PartOf(_Flipbook)]", Vector) = (1,1,1,1)
		_FlipSpeed ("序列帧速度[PartOf(_Flipbook)]", Range(0, 100)) = 10
		_MaskTex ("透明度遮罩", 2D) = "white" {}
		[MyToggle] _Billboard ("开启告示牌", Range(0, 1)) = 0
		_Vertical ("垂直约束[PartOf(_Billboard)]", Range(0, 1)) = 0
		_Noise ("噪声[Feature]", Float) = 1
		_NoiseTex ("X轴噪声贴图[PartOf(_Noise)]", 2D) = "white" {}
		_NoiseTex2 ("Y轴噪声贴图[PartOf(_Noise)]", 2D) = "white" {}
		_NoiseRamp ("噪声强度渐变(x : x轴 y : y轴)[PartOf(_Noise)]", Vector) = (1,1,1,1)
		_NoiseIntensity ("X轴噪声强度[PartOf(_Noise)]", Range(0, 1)) = 0.4
		_NoiseIntensity2 ("Y轴噪声强度[PartOf(_Noise)]", Range(0, 1)) = 0.2
		_NoiseSpeed ("噪声流动速度[PartOf(_Noise)]", Range(0, 4)) = 1
		_CustomFire ("自定义火焰[Feature]", Float) = 0
		_ColorRamp ("火焰过渡色[PartOf(_CustomFire)]", 2D) = "white" {}
		_FireIntensity ("火焰透明度[PartOf(_CustomFire)]", Range(0, 2)) = 1
		_FireCenter ("焰心范围[PartOf(_CustomFire)]", Range(0, 2)) = 1
		_AlphaIntensity ("外焰柔和度[PartOf(_CustomFire)]", Range(0, 2)) = 1
		[HDR] _BloomColor ("辉光颜色[PartOf(_CustomFire)]", Vector) = (1,1,1,1)
		_BloomIntensity ("辉光范围[PartOf(_CustomFire)]", Range(0, 1)) = 0
		_FireBrightness ("辉光亮度[PartOf(_CustomFire)]", Range(0, 2)) = 1
		[MyToggle] _ShowMask ("显示火焰灰度", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
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