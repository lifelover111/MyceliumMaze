

Shader "Hidden/LastPixelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
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

        TEXTURE2D(_MainTex);
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;

        //SAMPLER(sampler_MainTex);
        //Texture2D _MainTex;
        //SamplerState sampler_MainTex;

        SamplerState sampler_point_clamp;
        
        uniform float2 _BlockCount;
        uniform float2 _BlockSize;
        uniform float2 _HalfBlockSize;
        uniform float2 _SmoothingDelta;
        uniform float2 _ScreenRes;


        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            return OUT;
        }

        ENDHLSL

        Pass
        {
            Name "LastPixelation"

            HLSLPROGRAM
            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 screenUV = IN.uv * _ScreenRes + _SmoothingDelta;
                float2 newUV = screenUV;
                newUV.x = newUV.x / _ScreenRes.x;
                newUV.y = newUV.y / _ScreenRes.y;

                float2 blockPos = floor(newUV * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize; 
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, blockCenter);
                return tex;
            }
            ENDHLSL
        }

        
    }
}