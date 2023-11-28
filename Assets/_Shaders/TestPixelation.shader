// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "TestPixelation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelDensity ("Pixel Density", float) = 10
    }

    SubShader
    {
        Tags {"Queue" = "Overlay" }
LOD100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma exclude_renderers gles xbox360 ps3
            #pragma fragment frag

#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};

struct v2f
{
    float4 pos : POSITION;
    float4 color : COLOR;
};

uniform float _PixelDensity;

v2f vert_img(appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = v.normal;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // Calculate pixelation coordinates
    float2 uv = i.pos.xy / i.pos.w;
    float2 pixelCoord = float2(floor(uv.x * _PixelDensity), floor(uv.y * _PixelDensity));

                // Adjust pixelation coordinates for smoother transitions during camera movement
    float2 smoothUV = pixelCoord / _PixelDensity;

                // Sample the texture using the adjusted coordinates
    fixed4 col = tex2D(_MainTex, smoothUV);

    return col;
}
            ENDCG
        }
    }
}