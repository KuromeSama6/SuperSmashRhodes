Shader "SSR/2DTextureGlowSimple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Float) = 1.0
        _MainTexMix ("Texture Mix", Range(0, 1)) = 1
        _GlowThickness ("Glow Thickness", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _MainTexMix;
            float _GlowThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 texColor = tex2D(_MainTex, uv) * i.color;

                // Sobel filter kernel offsets
                float2 offsets[8] = {
                    float2(-_GlowThickness, _GlowThickness), // top-left
                    float2(0, _GlowThickness),              // top
                    float2(_GlowThickness, _GlowThickness), // top-right
                    float2(-_GlowThickness, 0),            // left
                    float2(_GlowThickness, 0),             // right
                    float2(-_GlowThickness, -_GlowThickness), // bottom-left
                    float2(0, -_GlowThickness),            // bottom
                    float2(_GlowThickness, -_GlowThickness)  // bottom-right
                };

                // Sobel kernels
                float Gx[8] = {-1, 0, 1, -2, 2, -1, 0, 1};
                float Gy[8] = {1, 2, 1, 0, 0, -1, -2, -1};

                // Calculate gradients
                float gradientX = 0.0;
                float gradientY = 0.0;
                for (int j = 0; j < 8; j++)
                {
                    float alpha = tex2D(_MainTex, uv + offsets[j]).a;
                    gradientX += Gx[j] * alpha;
                    gradientY += Gy[j] * alpha;
                }

                // Compute edge strength
                float edge = sqrt(gradientX * gradientX + gradientY * gradientY);

                // Apply glow effect based on edge strength
                float4 glow = _GlowColor * i.color * _GlowIntensity * edge;

                // Combine texture and glow
                float4 result = texColor * _MainTexMix + glow;
                result.a = texColor.a; // Preserve original alpha

                return result;
            }
            ENDCG
        }
    }
}
