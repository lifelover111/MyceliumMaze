Shader "MBS/Debug/VertexColor"
{
  Properties
  {  }
 
  SubShader
  {
    Tags { "RenderType"="Opaque" }
   
    CGPROGRAM
    #pragma vertex vert
    #pragma surface surf BlinnPhong

    struct Input
    {
        float3 vertColor;
    };
 
    void vert(inout appdata_full v, out Input OUT)
    {
        OUT.vertColor = v.color.rgb;
    }
 
 
    void surf (Input IN, inout SurfaceOutput OUT)
    {
        OUT.Albedo = IN.vertColor.rgb;
    }
    ENDCG
  }
 
  Fallback "Diffuse"
}