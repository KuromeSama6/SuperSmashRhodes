Shader "Torappu/Particles-L2D/3D/ToonOutline" {
	Properties {
		[KeywordEnum(Default, Custom)] _SunMode ("Dir Light Mode", Float) = 0
		_SunDir ("ViewSpace Sun Direction(xyz<2.0, a: strength)", Vector) = (0,0,1,1)
		_LightColor ("Light Color", Vector) = (1,1,1,1)
		_Color ("Tint Color", Vector) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_EmissionMap ("Emission", 2D) = "white" {}
		_EmissionColor ("Emission Color", Vector) = (1,1,1,1)
		_Ambient ("Ambient", Range(0, 1)) = 0.1
		_Diffuse ("Diffuse", Range(0, 1)) = 1
		_Shininess ("Shiness", Range(0, 20)) = 0.45
		_Specular ("Specular", Range(0, 1)) = 0.2
		_SpecColor ("Specular Color", Vector) = (1,1,1,1)
		[Header(Outline)] _OutlineColor ("Outline Color", Vector) = (0,0,0,1)
		_OutlineWidth ("Outline Width", Range(-1, 1)) = 0.01
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