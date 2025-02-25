Shader "SSR/UI/Grayscale Adjustable"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrayscaleAmount ("Grayscale Amount", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Correct transparency blending
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Get Image component's color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR; // Pass UI color to fragment
            };

            sampler2D _MainTex;
            float _GrayscaleAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Pass the UI Image's color to fragment
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Apply UI Image color (tint)
                texColor *= i.color;

                // Convert to grayscale
                float gray = dot(texColor.rgb, float3(0.3, 0.59, 0.11));

                // Blend between original and grayscale
                float3 finalColor = lerp(texColor.rgb, gray.xxx, _GrayscaleAmount);

                return fixed4(finalColor, texColor.a); // Preserve alpha
            }
            ENDCG
        }
    }
}
