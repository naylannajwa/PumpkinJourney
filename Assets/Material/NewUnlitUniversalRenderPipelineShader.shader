Shader "UI/UIPanelBlurURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 2.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BlurPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = float4(0,0,0,0);

                float blur = _BlurSize * 0.002;

                col += tex2D(_MainTex, uv + float2(-blur, -blur));
                col += tex2D(_MainTex, uv + float2( blur, -blur));
                col += tex2D(_MainTex, uv + float2(-blur,  blur));
                col += tex2D(_MainTex, uv + float2( blur,  blur));

                col += tex2D(_MainTex, uv + float2(-blur, 0));
                col += tex2D(_MainTex, uv + float2( blur, 0));
                col += tex2D(_MainTex, uv + float2(0, -blur));
                col += tex2D(_MainTex, uv + float2(0,  blur));

                col /= 8.0;
                return col;
            }
            ENDHLSL
        }
    }
}
