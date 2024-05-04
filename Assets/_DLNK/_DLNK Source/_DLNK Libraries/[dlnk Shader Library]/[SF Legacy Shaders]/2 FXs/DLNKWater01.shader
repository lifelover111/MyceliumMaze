// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.3587191,fgcg:0.3438582,fgcb:0.3897059,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4544,x:32803,y:32436,varname:node_4544,prsc:2|diff-5388-OUT,spec-8476-OUT,gloss-9000-OUT,normal-7340-OUT,alpha-5549-OUT,refract-576-OUT;n:type:ShaderForge.SFN_Blend,id:1461,x:32299,y:32685,varname:node_1461,prsc:2,blmd:0,clmp:True|SRC-9562-OUT,DST-1981-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7198,x:31703,y:32744,ptovrint:False,ptlb:Main Texture,ptin:_MainTexture,varname:node_7198,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7986,x:31930,y:32649,varname:node_7986,prsc:2,ntxv:0,isnm:False|UVIN-9126-OUT,MIP-8736-OUT,TEX-7198-TEX;n:type:ShaderForge.SFN_Tex2d,id:2094,x:31932,y:32792,varname:node_2094,prsc:2,ntxv:0,isnm:False|UVIN-3194-OUT,MIP-8736-OUT,TEX-7198-TEX;n:type:ShaderForge.SFN_Time,id:8481,x:31688,y:33289,varname:node_8481,prsc:2;n:type:ShaderForge.SFN_Append,id:1325,x:31920,y:33351,varname:node_1325,prsc:2|A-9442-X,B-9442-Y;n:type:ShaderForge.SFN_Multiply,id:3966,x:31920,y:33214,varname:node_3966,prsc:2|A-8481-TSL,B-1308-OUT,C-1325-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1308,x:31688,y:33442,ptovrint:False,ptlb:Time Size,ptin:_TimeSize,varname:node_1308,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:9126,x:31920,y:33087,varname:node_9126,prsc:2|A-2122-OUT,B-3966-OUT;n:type:ShaderForge.SFN_TexCoord,id:7461,x:31851,y:33743,varname:node_7461,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:9987,x:31851,y:33909,ptovrint:False,ptlb:Master Tiling,ptin:_MasterTiling,varname:node_9987,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2225,x:31851,y:33986,ptovrint:False,ptlb:Detail Tiling,ptin:_DetailTiling,varname:node_2225,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:2122,x:32029,y:33743,varname:node_2122,prsc:2|A-7461-UVOUT,B-9987-OUT;n:type:ShaderForge.SFN_Multiply,id:6351,x:32194,y:33743,varname:node_6351,prsc:2|A-2122-OUT,B-2225-OUT;n:type:ShaderForge.SFN_Vector4Property,id:9442,x:31920,y:33524,ptovrint:False,ptlb:MainV(XY) DetV(ZW),ptin:_MainVXYDetVZW,varname:node_9442,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1,v2:1,v3:0.5,v4:0.5;n:type:ShaderForge.SFN_Append,id:8444,x:32104,y:33351,varname:node_8444,prsc:2|A-9442-Z,B-9442-W;n:type:ShaderForge.SFN_Multiply,id:2703,x:32104,y:33214,varname:node_2703,prsc:2|A-8481-TSL,B-1308-OUT,C-8444-OUT;n:type:ShaderForge.SFN_Add,id:3194,x:32104,y:33087,varname:node_3194,prsc:2|A-6351-OUT,B-2703-OUT;n:type:ShaderForge.SFN_Desaturate,id:9562,x:32132,y:32649,varname:node_9562,prsc:2|COL-7986-RGB;n:type:ShaderForge.SFN_Desaturate,id:1981,x:32132,y:32792,varname:node_1981,prsc:2|COL-2094-RGB;n:type:ShaderForge.SFN_Blend,id:4016,x:32299,y:32527,varname:node_4016,prsc:2,blmd:6,clmp:True|SRC-9562-OUT,DST-1981-OUT;n:type:ShaderForge.SFN_Color,id:5039,x:32299,y:32202,ptovrint:False,ptlb:Color A,ptin:_ColorA,varname:node_5039,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7867647,c2:0.7867647,c3:0.7867647,c4:1;n:type:ShaderForge.SFN_OneMinus,id:1419,x:32299,y:32846,varname:node_1419,prsc:2|IN-9562-OUT;n:type:ShaderForge.SFN_Lerp,id:4795,x:32457,y:32731,varname:node_4795,prsc:2|A-4016-OUT,B-1461-OUT,T-1419-OUT;n:type:ShaderForge.SFN_Multiply,id:4087,x:32466,y:33035,varname:node_4087,prsc:2|A-1461-OUT,B-2837-OUT;n:type:ShaderForge.SFN_Multiply,id:9928,x:32466,y:33150,varname:node_9928,prsc:2|A-4795-OUT,B-2765-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2837,x:32299,y:33090,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_2837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2765,x:32299,y:33160,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_2765,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2dAsset,id:8853,x:32347,y:33330,ptovrint:False,ptlb:Bump Map,ptin:_BumpMap,varname:node_8853,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:8730,x:32550,y:33288,varname:node_8730,prsc:2,ntxv:0,isnm:False|UVIN-9126-OUT,TEX-8853-TEX;n:type:ShaderForge.SFN_Tex2d,id:2158,x:32550,y:33414,varname:node_2158,prsc:2,ntxv:0,isnm:False|UVIN-3194-OUT,TEX-8853-TEX;n:type:ShaderForge.SFN_Lerp,id:2179,x:32938,y:33665,varname:node_2179,prsc:2|A-8730-RGB,B-2158-RGB,T-1419-OUT;n:type:ShaderForge.SFN_RemapRange,id:8193,x:32980,y:33414,varname:node_8193,prsc:2,frmn:0,frmx:255,tomn:0,tomx:1|IN-7858-OUT;n:type:ShaderForge.SFN_Lerp,id:7340,x:32980,y:33288,varname:node_7340,prsc:2|A-6925-OUT,B-8193-OUT,T-8327-OUT;n:type:ShaderForge.SFN_Color,id:2506,x:32299,y:32365,ptovrint:False,ptlb:Color B,ptin:_ColorB,varname:_ColorA_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1691176,c2:0.1691176,c3:0.1691176,c4:1;n:type:ShaderForge.SFN_Power,id:9448,x:32154,y:32335,varname:node_9448,prsc:2|VAL-4795-OUT,EXP-2471-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2471,x:32154,y:32473,ptovrint:False,ptlb:Mix Power,ptin:_MixPower,varname:node_2471,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:5388,x:32490,y:32298,varname:node_5388,prsc:2|A-5039-RGB,B-2506-RGB,T-9448-OUT;n:type:ShaderForge.SFN_Blend,id:9000,x:32727,y:33047,varname:node_9000,prsc:2,blmd:10,clmp:True|SRC-9928-OUT,DST-9755-OUT;n:type:ShaderForge.SFN_Slider,id:9755,x:32648,y:33218,ptovrint:False,ptlb:Gloss Smooth,ptin:_GlossSmooth,varname:node_9755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:8476,x:32466,y:32913,varname:node_8476,prsc:2|IN-4087-OUT;n:type:ShaderForge.SFN_Vector3,id:6925,x:32779,y:33288,varname:node_6925,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_ValueProperty,id:8327,x:32789,y:33593,ptovrint:False,ptlb:Bump Scale,ptin:_BumpScale,varname:node_8327,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Slider,id:8736,x:31553,y:33049,ptovrint:False,ptlb:Maps Smooth,ptin:_MapsSmooth,varname:node_8736,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Multiply,id:5524,x:33057,y:32424,varname:node_5524,prsc:2|A-2059-OUT,B-9702-Y;n:type:ShaderForge.SFN_Power,id:2059,x:33057,y:32308,varname:node_2059,prsc:2|VAL-4795-OUT,EXP-9702-X;n:type:ShaderForge.SFN_Vector4Property,id:9702,x:33401,y:32099,ptovrint:False,ptlb:Opac Power(R) Mult(G) Add(B) Depth(W),ptin:_OpacPowerRMultGAddBDepthW,varname:node_9702,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1,v2:1,v3:0,v4:0;n:type:ShaderForge.SFN_Add,id:874,x:33057,y:32193,varname:node_874,prsc:2|A-5524-OUT,B-9702-Z,C-8934-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6019,x:32980,y:33148,varname:node_6019,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7340-OUT;n:type:ShaderForge.SFN_Multiply,id:6998,x:33608,y:33333,varname:node_6998,prsc:2|A-6019-OUT,B-9396-OUT,C-3683-OUT,D-4565-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5021,x:33155,y:33633,ptovrint:False,ptlb:Affect Depth,ptin:_AffectDepth,varname:node_5021,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:9396,x:33321,y:33514,varname:node_9396,prsc:2|A-5021-OUT,B-9844-OUT;n:type:ShaderForge.SFN_Clamp01,id:977,x:33402,y:33376,varname:node_977,prsc:2|IN-9396-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3683,x:33321,y:33650,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_3683,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Depth,id:687,x:32605,y:31936,varname:node_687,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8934,x:32940,y:31936,varname:node_8934,prsc:2|A-8250-OUT,B-9702-W,C-936-OUT,D-1461-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6080,x:32605,y:32081,ptovrint:False,ptlb:Depth Range,ptin:_DepthRange,varname:node_6080,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:5549,x:33214,y:32193,varname:node_5549,prsc:2|IN-874-OUT;n:type:ShaderForge.SFN_Power,id:8250,x:32783,y:31936,varname:node_8250,prsc:2|VAL-687-OUT,EXP-6080-OUT;n:type:ShaderForge.SFN_Vector1,id:936,x:32605,y:32140,varname:node_936,prsc:2,v1:0.01;n:type:ShaderForge.SFN_NormalVector,id:4565,x:33321,y:33710,prsc:2,pt:False;n:type:ShaderForge.SFN_ComponentMask,id:576,x:33608,y:33179,varname:node_576,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6998-OUT;n:type:ShaderForge.SFN_NormalBlend,id:7858,x:32739,y:33409,varname:node_7858,prsc:2|BSE-8730-RGB,DTL-2158-RGB;n:type:ShaderForge.SFN_OneMinus,id:4603,x:33155,y:33476,varname:node_4603,prsc:2|IN-8934-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:9844,x:33208,y:33284,ptovrint:False,ptlb:Invert Depth,ptin:_InvertDepth,varname:node_9844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8934-OUT,B-4603-OUT;proporder:7198-5039-2506-2471-9987-2225-8853-8327-2837-2765-9755-1308-9442-8736-9702-6080-5021-3683-9844;pass:END;sub:END;*/

Shader "DLNK/FX/DLNKWater01" {
    Properties {
        _MainTexture ("Main Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (0.7867647,0.7867647,0.7867647,1)
        _ColorB ("Color B", Color) = (0.1691176,0.1691176,0.1691176,1)
        _MixPower ("Mix Power", Float ) = 1
        _MasterTiling ("Master Tiling", Float ) = 1
        _DetailTiling ("Detail Tiling", Float ) = 2
        _BumpMap ("Bump Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float ) = 1
        _Specular ("Specular", Float ) = 1
        _Gloss ("Gloss", Float ) = 1
        _GlossSmooth ("Gloss Smooth", Range(-1, 1)) = 0
        _TimeSize ("Time Size", Float ) = 1
        _MainVXYDetVZW ("MainV(XY) DetV(ZW)", Vector) = (1,1,0.5,0.5)
        _MapsSmooth ("Maps Smooth", Range(0, 10)) = 0
        _OpacPowerRMultGAddBDepthW ("Opac Power(R) Mult(G) Add(B) Depth(W)", Vector) = (1,1,0,0)
        _DepthRange ("Depth Range", Float ) = 1
        _AffectDepth ("Affect Depth", Float ) = 0
        _Refraction ("Refraction", Float ) = 1
        [MaterialToggle] _InvertDepth ("Invert Depth", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _TimeSize;
            uniform float _MasterTiling;
            uniform float _DetailTiling;
            uniform float4 _MainVXYDetVZW;
            uniform float4 _ColorA;
            uniform float _Specular;
            uniform float _Gloss;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float4 _ColorB;
            uniform float _MixPower;
            uniform float _GlossSmooth;
            uniform float _BumpScale;
            uniform float _MapsSmooth;
            uniform float4 _OpacPowerRMultGAddBDepthW;
            uniform float _AffectDepth;
            uniform float _Refraction;
            uniform float _DepthRange;
            uniform fixed _InvertDepth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD11;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_2122 = (i.uv0*_MasterTiling);
                float4 node_8481 = _Time;
                float2 node_9126 = (node_2122+(node_8481.r*_TimeSize*float2(_MainVXYDetVZW.r,_MainVXYDetVZW.g)));
                float3 node_8730 = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_9126, _BumpMap)));
                float2 node_3194 = ((node_2122*_DetailTiling)+(node_8481.r*_TimeSize*float2(_MainVXYDetVZW.b,_MainVXYDetVZW.a)));
                float3 node_2158 = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_3194, _BumpMap)));
                float3 node_7858_nrm_base = node_8730.rgb + float3(0,0,1);
                float3 node_7858_nrm_detail = node_2158.rgb * float3(-1,-1,1);
                float3 node_7858_nrm_combined = node_7858_nrm_base*dot(node_7858_nrm_base, node_7858_nrm_detail)/node_7858_nrm_base.z - node_7858_nrm_detail;
                float3 node_7858 = node_7858_nrm_combined;
                float3 node_7340 = lerp(float3(0,0,1),(node_7858*0.003921569+0.0),_BumpScale);
                float3 normalLocal = node_7340;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_7986 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_9126, _MainTexture),0.0,_MapsSmooth));
                float node_9562 = dot(node_7986.rgb,float3(0.3,0.59,0.11));
                float4 node_2094 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_3194, _MainTexture),0.0,_MapsSmooth));
                float node_1981 = dot(node_2094.rgb,float3(0.3,0.59,0.11));
                float node_1461 = saturate(min(node_9562,node_1981));
                float node_8934 = (pow(partZ,_DepthRange)*_OpacPowerRMultGAddBDepthW.a*0.01*node_1461);
                float node_9396 = (_AffectDepth*lerp( node_8934, (1.0 - node_8934), _InvertDepth ));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (float3(node_7340.rg,0.0)*node_9396*_Refraction*i.normalDir).rg;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float node_1419 = (1.0 - node_9562);
                float node_4795 = lerp(saturate((1.0-(1.0-node_9562)*(1.0-node_1981))),node_1461,node_1419);
                float gloss = saturate(( _GlossSmooth > 0.5 ? (1.0-(1.0-2.0*(_GlossSmooth-0.5))*(1.0-(node_4795*_Gloss))) : (2.0*_GlossSmooth*(node_4795*_Gloss)) ));
                float perceptualRoughness = 1.0 - saturate(( _GlossSmooth > 0.5 ? (1.0-(1.0-2.0*(_GlossSmooth-0.5))*(1.0-(node_4795*_Gloss))) : (2.0*_GlossSmooth*(node_4795*_Gloss)) ));
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float node_8476 = saturate((node_1461*_Specular));
                float3 specularColor = float3(node_8476,node_8476,node_8476);
                float specularMonochrome;
                float3 diffuseColor = lerp(_ColorA.rgb,_ColorB.rgb,pow(node_4795,_MixPower)); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,saturate(((pow(node_4795,_OpacPowerRMultGAddBDepthW.r)*_OpacPowerRMultGAddBDepthW.g)+_OpacPowerRMultGAddBDepthW.b+node_8934))),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _TimeSize;
            uniform float _MasterTiling;
            uniform float _DetailTiling;
            uniform float4 _MainVXYDetVZW;
            uniform float4 _ColorA;
            uniform float _Specular;
            uniform float _Gloss;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float4 _ColorB;
            uniform float _MixPower;
            uniform float _GlossSmooth;
            uniform float _BumpScale;
            uniform float _MapsSmooth;
            uniform float4 _OpacPowerRMultGAddBDepthW;
            uniform float _AffectDepth;
            uniform float _Refraction;
            uniform float _DepthRange;
            uniform fixed _InvertDepth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_2122 = (i.uv0*_MasterTiling);
                float4 node_8481 = _Time;
                float2 node_9126 = (node_2122+(node_8481.r*_TimeSize*float2(_MainVXYDetVZW.r,_MainVXYDetVZW.g)));
                float3 node_8730 = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_9126, _BumpMap)));
                float2 node_3194 = ((node_2122*_DetailTiling)+(node_8481.r*_TimeSize*float2(_MainVXYDetVZW.b,_MainVXYDetVZW.a)));
                float3 node_2158 = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_3194, _BumpMap)));
                float3 node_7858_nrm_base = node_8730.rgb + float3(0,0,1);
                float3 node_7858_nrm_detail = node_2158.rgb * float3(-1,-1,1);
                float3 node_7858_nrm_combined = node_7858_nrm_base*dot(node_7858_nrm_base, node_7858_nrm_detail)/node_7858_nrm_base.z - node_7858_nrm_detail;
                float3 node_7858 = node_7858_nrm_combined;
                float3 node_7340 = lerp(float3(0,0,1),(node_7858*0.003921569+0.0),_BumpScale);
                float3 normalLocal = node_7340;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_7986 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_9126, _MainTexture),0.0,_MapsSmooth));
                float node_9562 = dot(node_7986.rgb,float3(0.3,0.59,0.11));
                float4 node_2094 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_3194, _MainTexture),0.0,_MapsSmooth));
                float node_1981 = dot(node_2094.rgb,float3(0.3,0.59,0.11));
                float node_1461 = saturate(min(node_9562,node_1981));
                float node_8934 = (pow(partZ,_DepthRange)*_OpacPowerRMultGAddBDepthW.a*0.01*node_1461);
                float node_9396 = (_AffectDepth*lerp( node_8934, (1.0 - node_8934), _InvertDepth ));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (float3(node_7340.rg,0.0)*node_9396*_Refraction*i.normalDir).rg;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float node_1419 = (1.0 - node_9562);
                float node_4795 = lerp(saturate((1.0-(1.0-node_9562)*(1.0-node_1981))),node_1461,node_1419);
                float gloss = saturate(( _GlossSmooth > 0.5 ? (1.0-(1.0-2.0*(_GlossSmooth-0.5))*(1.0-(node_4795*_Gloss))) : (2.0*_GlossSmooth*(node_4795*_Gloss)) ));
                float perceptualRoughness = 1.0 - saturate(( _GlossSmooth > 0.5 ? (1.0-(1.0-2.0*(_GlossSmooth-0.5))*(1.0-(node_4795*_Gloss))) : (2.0*_GlossSmooth*(node_4795*_Gloss)) ));
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float node_8476 = saturate((node_1461*_Specular));
                float3 specularColor = float3(node_8476,node_8476,node_8476);
                float specularMonochrome;
                float3 diffuseColor = lerp(_ColorA.rgb,_ColorB.rgb,pow(node_4795,_MixPower)); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * saturate(((pow(node_4795,_OpacPowerRMultGAddBDepthW.r)*_OpacPowerRMultGAddBDepthW.g)+_OpacPowerRMultGAddBDepthW.b+node_8934)),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _TimeSize;
            uniform float _MasterTiling;
            uniform float _DetailTiling;
            uniform float4 _MainVXYDetVZW;
            uniform float4 _ColorA;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float4 _ColorB;
            uniform float _MixPower;
            uniform float _GlossSmooth;
            uniform float _MapsSmooth;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float2 node_2122 = (i.uv0*_MasterTiling);
                float4 node_8481 = _Time;
                float2 node_9126 = (node_2122+(node_8481.r*_TimeSize*float2(_MainVXYDetVZW.r,_MainVXYDetVZW.g)));
                float4 node_7986 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_9126, _MainTexture),0.0,_MapsSmooth));
                float node_9562 = dot(node_7986.rgb,float3(0.3,0.59,0.11));
                float2 node_3194 = ((node_2122*_DetailTiling)+(node_8481.r*_TimeSize*float2(_MainVXYDetVZW.b,_MainVXYDetVZW.a)));
                float4 node_2094 = tex2Dlod(_MainTexture,float4(TRANSFORM_TEX(node_3194, _MainTexture),0.0,_MapsSmooth));
                float node_1981 = dot(node_2094.rgb,float3(0.3,0.59,0.11));
                float node_1461 = saturate(min(node_9562,node_1981));
                float node_1419 = (1.0 - node_9562);
                float node_4795 = lerp(saturate((1.0-(1.0-node_9562)*(1.0-node_1981))),node_1461,node_1419);
                float3 diffColor = lerp(_ColorA.rgb,_ColorB.rgb,pow(node_4795,_MixPower));
                float node_8476 = saturate((node_1461*_Specular));
                float3 specColor = float3(node_8476,node_8476,node_8476);
                float specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);
                diffColor *= (1.0-specularMonochrome);
                float roughness = 1.0 - saturate(( _GlossSmooth > 0.5 ? (1.0-(1.0-2.0*(_GlossSmooth-0.5))*(1.0-(node_4795*_Gloss))) : (2.0*_GlossSmooth*(node_4795*_Gloss)) ));
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
