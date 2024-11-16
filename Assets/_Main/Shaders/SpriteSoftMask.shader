Shader "SSR/SoftMaskWithInvert"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MaskScale ("Mask Scale", Float) = 1.0
        _InvertMask ("Invert Mask", Float) = 0.0
        _AlphaOffset("Alpha Offset", Range(-1, 1)) = 0
        _AlphaRaise("Alpha Raise", Float) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0; // UV for _MainTex
                float2 texcoord1 : TEXCOORD1; // UV for _MaskTex
                float4 color : COLOR0;
            };

            struct v2f
            {
                float2 mainUV : TEXCOORD0; // UV for _MainTex
                float2 maskUV : TEXCOORD1; // UV for _MaskTex
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _MaskScale;
            float _InvertMask;
            float _AlphaOffset;
            float _AlphaRaise;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.mainUV = v.texcoord;
                o.maskUV = (v.texcoord - 0.5) / _MaskScale + 0.5;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.mainUV) * i.color;
                fixed4 mask = tex2D(_MaskTex, i.maskUV);
                float maskAlpha = _InvertMask > 0.5 ? (1.0 - mask.a) : mask.a;

                col.a *= max(0, maskAlpha - _AlphaOffset);
                col.a *= _AlphaRaise;
                
                return col;
            }
            ENDCG
        }
    }
}
