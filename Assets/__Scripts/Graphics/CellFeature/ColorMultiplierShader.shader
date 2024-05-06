Shader "Hidden/ColorMultiplierShader"
{
    Properties
    {
        _Multiplier ("Multiplier", Int) = 1
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int _Multiplier;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,1);
                col *= 5;
                col = round(col); 
                col /= 5; 
                return col;
            }
            ENDCG
        }
    }
}