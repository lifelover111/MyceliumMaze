// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.3587191,fgcg:0.3438582,fgcb:0.3897059,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:479,x:32856,y:32637,varname:node_479,prsc:2|diff-1724-OUT,spec-9909-OUT,gloss-8352-OUT,normal-2662-OUT,emission-5287-OUT,amdfl-9821-OUT,difocc-5420-OUT,clip-153-OUT,voffset-8654-OUT;n:type:ShaderForge.SFN_Multiply,id:1724,x:32521,y:32718,varname:node_1724,prsc:2|A-3829-OUT,B-9680-RGB;n:type:ShaderForge.SFN_Color,id:1237,x:31921,y:32195,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:5164,x:31935,y:33250,ptovrint:False,ptlb:EmissionColor,ptin:_EmissionColor,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9680,x:32118,y:32764,ptovrint:False,ptlb:Main Tex,ptin:_MainTex,varname:node_9680,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:1403,x:32345,y:32808,ptovrint:False,ptlb:Use Alpha,ptin:_UseAlpha,varname:node_1403,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5657-A,B-9680-A;n:type:ShaderForge.SFN_Tex2d,id:5657,x:32117,y:32947,ptovrint:False,ptlb:Opacity Map,ptin:_OpacityMap,varname:node_5657,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Clamp01,id:153,x:32673,y:32919,varname:node_153,prsc:2|IN-7662-OUT;n:type:ShaderForge.SFN_Multiply,id:7662,x:32511,y:32919,varname:node_7662,prsc:2|A-1403-OUT,B-822-OUT;n:type:ShaderForge.SFN_ValueProperty,id:822,x:32341,y:32978,ptovrint:False,ptlb:Cutoff,ptin:_Cutoff,varname:node_822,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:9841,x:32351,y:33183,ptovrint:False,ptlb:Occlusion Map,ptin:_OcclusionMap,varname:node_9841,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b12ca1b0c12efad45aa846dbcc7baf3d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9678,x:31935,y:33415,ptovrint:False,ptlb:Emission Map,ptin:_EmissionMap,varname:node_9678,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8006,x:31921,y:32574,ptovrint:False,ptlb:Bump Map,ptin:_BumpMap,varname:node_8006,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:2028,x:32451,y:32438,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_2028,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7436,x:32451,y:32532,ptovrint:False,ptlb:Glossiness,ptin:_Glossiness,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Multiply,id:20,x:32603,y:33183,varname:node_20,prsc:2|A-9841-R,B-3117-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3117,x:32351,y:33355,ptovrint:False,ptlb:Occlusion Strenght,ptin:_OcclusionStrenght,varname:node_3117,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:1262,x:31935,y:33602,ptovrint:False,ptlb:Emission LM,ptin:_EmissionLM,varname:node_1262,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5287,x:32132,y:33468,varname:node_5287,prsc:2|A-5164-RGB,B-9678-RGB,C-1262-OUT,D-9678-A;n:type:ShaderForge.SFN_ValueProperty,id:8648,x:32862,y:33551,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_8648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Fresnel,id:6007,x:32862,y:33392,varname:node_6007,prsc:2|EXP-8648-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3133,x:33017,y:33392,ptovrint:False,ptlb:Rim Power,ptin:_RimPower,varname:_Fresnel_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Color,id:4976,x:33017,y:33478,ptovrint:False,ptlb:Rim Color,ptin:_RimColor,varname:node_4976,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1308,x:32863,y:33249,varname:node_1308,prsc:2|A-3133-OUT,B-6007-OUT,C-4976-RGB,D-5420-OUT;n:type:ShaderForge.SFN_AmbientLight,id:1590,x:33089,y:32825,varname:node_1590,prsc:2;n:type:ShaderForge.SFN_Add,id:9821,x:33208,y:33023,varname:node_9821,prsc:2|A-1308-OUT,B-1590-RGB;n:type:ShaderForge.SFN_Clamp01,id:7092,x:32603,y:33327,varname:node_7092,prsc:2|IN-20-OUT;n:type:ShaderForge.SFN_Multiply,id:9909,x:32836,y:32303,varname:node_9909,prsc:2|A-2725-R,B-2028-OUT;n:type:ShaderForge.SFN_Multiply,id:8352,x:32836,y:32448,varname:node_8352,prsc:2|A-5300-OUT,B-7436-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5300,x:32836,y:32178,ptovrint:False,ptlb:Metal Alpha Gloss,ptin:_MetalAlphaGloss,varname:node_5300,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1185-R,B-2725-A;n:type:ShaderForge.SFN_Tex2d,id:1185,x:32619,y:32063,ptovrint:False,ptlb:Smoothness Map,ptin:_SmoothnessMap,varname:node_1185,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2725,x:32619,y:32246,ptovrint:False,ptlb:Metallic Gloss Map,ptin:_MetallicGlossMap,varname:node_2725,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2662,x:32112,y:32574,varname:node_2662,prsc:2|A-8920-OUT,B-8006-RGB,T-6239-OUT;n:type:ShaderForge.SFN_Vector3,id:8920,x:31921,y:32729,varname:node_8920,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_ValueProperty,id:6239,x:31921,y:32839,ptovrint:False,ptlb:Bump Scale,ptin:_BumpScale,varname:node_6239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:6436,x:33224,y:33876,varname:node_6436,prsc:2|A-4834-OUT,B-6383-OUT,C-7815-OUT;n:type:ShaderForge.SFN_TexCoord,id:5559,x:32275,y:34206,varname:node_5559,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Blend,id:8413,x:32649,y:34198,varname:node_8413,prsc:2,blmd:17,clmp:True|SRC-5559-U,DST-9924-OUT;n:type:ShaderForge.SFN_OneMinus,id:9924,x:32456,y:34216,varname:node_9924,prsc:2|IN-5559-U;n:type:ShaderForge.SFN_Multiply,id:6050,x:32836,y:34198,varname:node_6050,prsc:2|A-8413-OUT,B-6833-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6833,x:32836,y:34344,ptovrint:False,ptlb:Lenght Anim,ptin:_LenghtAnim,varname:node_6833,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6383,x:32682,y:33916,ptovrint:False,ptlb:Anim Intensity,ptin:_AnimIntensity,varname:node_6383,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:7716,x:32456,y:34037,varname:node_7716,prsc:2|A-5559-V,B-6815-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6815,x:32275,y:34037,ptovrint:False,ptlb:Weight Anim,ptin:_WeightAnim,varname:_LenghtAnim_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:6222,x:32847,y:34008,varname:node_6222,prsc:2|A-7716-OUT,B-6050-OUT;n:type:ShaderForge.SFN_NormalVector,id:7815,x:33224,y:34018,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:5991,x:32631,y:33660,varname:node_5991,prsc:2|A-6222-OUT,B-5978-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5978,x:32631,y:33790,ptovrint:False,ptlb:Movement AO Mask,ptin:_MovementAOMask,varname:node_5978,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Blend,id:5420,x:32617,y:33460,varname:node_5420,prsc:2,blmd:10,clmp:True|SRC-20-OUT,DST-6204-OUT;n:type:ShaderForge.SFN_Sin,id:3073,x:32939,y:34503,varname:node_3073,prsc:2|IN-7216-OUT;n:type:ShaderForge.SFN_Time,id:7435,x:32589,y:34503,varname:node_7435,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:3777,x:32626,y:34658,ptovrint:False,ptlb:UpDown Velocity,ptin:_UpDownVelocity,varname:node_3777,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7216,x:32781,y:34503,varname:node_7216,prsc:2|A-7435-TSL,B-3777-OUT;n:type:ShaderForge.SFN_Multiply,id:8341,x:32268,y:34516,varname:node_8341,prsc:2|A-4992-OUT,B-8398-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:1054,x:31926,y:34516,varname:node_1054,prsc:2;n:type:ShaderForge.SFN_Append,id:8398,x:32100,y:34516,varname:node_8398,prsc:2|A-1054-X,B-1054-Z;n:type:ShaderForge.SFN_ValueProperty,id:9917,x:32264,y:34782,ptovrint:False,ptlb:WorldNoise Velocity,ptin:_WorldNoiseVelocity,varname:_UpDownVelocity_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:1194,x:32425,y:34385,ptovrint:False,ptlb:WorldNoise,ptin:_WorldNoise,varname:node_1194,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8341-OUT;n:type:ShaderForge.SFN_Multiply,id:4992,x:32264,y:34634,varname:node_4992,prsc:2|A-7435-TSL,B-9917-OUT;n:type:ShaderForge.SFN_Lerp,id:3829,x:32112,y:32440,varname:node_3829,prsc:2|A-1237-RGB,B-532-RGB,T-6204-OUT;n:type:ShaderForge.SFN_Color,id:532,x:31921,y:32351,ptovrint:False,ptlb:Color Fade,ptin:_ColorFade,varname:node_532,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:4508,x:33299,y:34264,varname:node_4508,prsc:2|A-2992-OUT,B-2086-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7612,x:32275,y:34118,ptovrint:False,ptlb:Weight Offset,ptin:_WeightOffset,varname:_WeightAnim_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:4734,x:32836,y:34418,ptovrint:False,ptlb:Length Offset,ptin:_LengthOffset,varname:_WeightOffset_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4834,x:33306,y:34520,varname:node_4834,prsc:2|A-1194-A,B-3073-OUT,C-6222-OUT;n:type:ShaderForge.SFN_Multiply,id:2992,x:33097,y:34344,varname:node_2992,prsc:2|A-8413-OUT,B-4734-OUT;n:type:ShaderForge.SFN_Multiply,id:2086,x:33097,y:34220,varname:node_2086,prsc:2|A-7716-OUT,B-7612-OUT;n:type:ShaderForge.SFN_Multiply,id:3632,x:33426,y:34018,varname:node_3632,prsc:2|A-7815-OUT,B-4508-OUT;n:type:ShaderForge.SFN_Add,id:8654,x:33426,y:33876,varname:node_8654,prsc:2|A-6436-OUT,B-3632-OUT;n:type:ShaderForge.SFN_Clamp01,id:6204,x:32836,y:33645,varname:node_6204,prsc:2|IN-5991-OUT;proporder:1237-532-9680-1403-822-2028-2725-5300-7436-1185-8006-6239-5657-9841-3117-5978-5164-9678-1262-8648-3133-4976-6383-6833-6815-3777-9917-1194-7612-4734;pass:END;sub:END;*/

Shader "DLNK/FX/AdvancedLeaves" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _ColorFade ("Color Fade", Color) = (0.5,0.5,0.5,1)
        _MainTex ("Main Tex", 2D) = "white" {}
        [MaterialToggle] _UseAlpha ("Use Alpha", Float ) = 0
        _Cutoff ("Cutoff", Float ) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
        _MetallicGlossMap ("Metallic Gloss Map", 2D) = "white" {}
        [MaterialToggle] _MetalAlphaGloss ("Metal Alpha Gloss", Float ) = 0
        _Glossiness ("Glossiness", Range(0, 1)) = 0.2
        _SmoothnessMap ("Smoothness Map", 2D) = "white" {}
        _BumpMap ("Bump Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float ) = 1
        _OpacityMap ("Opacity Map", 2D) = "white" {}
        _OcclusionMap ("Occlusion Map", 2D) = "white" {}
        _OcclusionStrenght ("Occlusion Strenght", Float ) = 1
        _MovementAOMask ("Movement AO Mask", Float ) = 1
        _EmissionColor ("EmissionColor", Color) = (0.5,0.5,0.5,1)
        _EmissionMap ("Emission Map", 2D) = "white" {}
        _EmissionLM ("Emission LM", Float ) = 0
        _Fresnel ("Fresnel", Float ) = 0
        _RimPower ("Rim Power", Float ) = 0
        _RimColor ("Rim Color", Color) = (0.5,0.5,0.5,1)
        _AnimIntensity ("Anim Intensity", Float ) = 0
        _LenghtAnim ("Lenght Anim", Float ) = 1
        _WeightAnim ("Weight Anim", Float ) = 1
        _UpDownVelocity ("UpDown Velocity", Float ) = 1
        _WorldNoiseVelocity ("WorldNoise Velocity", Float ) = 1
        _WorldNoise ("WorldNoise", 2D) = "white" {}
        _WeightOffset ("Weight Offset", Float ) = 1
        _LengthOffset ("Length Offset", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float4 _EmissionColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed _UseAlpha;
            uniform sampler2D _OpacityMap; uniform float4 _OpacityMap_ST;
            uniform float _Cutoff;
            uniform sampler2D _OcclusionMap; uniform float4 _OcclusionMap_ST;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _OcclusionStrenght;
            uniform float _EmissionLM;
            uniform float _Fresnel;
            uniform float _RimPower;
            uniform float4 _RimColor;
            uniform fixed _MetalAlphaGloss;
            uniform sampler2D _SmoothnessMap; uniform float4 _SmoothnessMap_ST;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform float _BumpScale;
            uniform float _LenghtAnim;
            uniform float _AnimIntensity;
            uniform float _WeightAnim;
            uniform float _MovementAOMask;
            uniform float _UpDownVelocity;
            uniform float _WorldNoiseVelocity;
            uniform sampler2D _WorldNoise; uniform float4 _WorldNoise_ST;
            uniform float4 _ColorFade;
            uniform float _WeightOffset;
            uniform float _LengthOffset;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
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
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_7435 = _Time;
                float2 node_8341 = ((node_7435.r*_WorldNoiseVelocity)*float2(mul(unity_ObjectToWorld, v.vertex).r,mul(unity_ObjectToWorld, v.vertex).b));
                float4 _WorldNoise_var = tex2Dlod(_WorldNoise,float4(TRANSFORM_TEX(node_8341, _WorldNoise),0.0,0));
                float node_7716 = (o.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(o.uv0.r-(1.0 - o.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                v.vertex.xyz += (((_WorldNoise_var.a*sin((node_7435.r*_UpDownVelocity))*node_6222)*_AnimIntensity*v.normal)+(v.normal*((node_8413*_LengthOffset)+(node_7716*_WeightOffset))));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = lerp(float3(0,0,1),_BumpMap_var.rgb,_BumpScale);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float4 _OpacityMap_var = tex2D(_OpacityMap,TRANSFORM_TEX(i.uv0, _OpacityMap));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(saturate((lerp( _OpacityMap_var.a, _MainTex_var.a, _UseAlpha )*_Cutoff)) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _SmoothnessMap_var = tex2D(_SmoothnessMap,TRANSFORM_TEX(i.uv0, _SmoothnessMap));
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(i.uv0, _MetallicGlossMap));
                float gloss = (lerp( _SmoothnessMap_var.r, _MetallicGlossMap_var.a, _MetalAlphaGloss )*_Glossiness);
                float perceptualRoughness = 1.0 - (lerp( _SmoothnessMap_var.r, _MetallicGlossMap_var.a, _MetalAlphaGloss )*_Glossiness);
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
                float3 specularColor = (_MetallicGlossMap_var.r*_Metallic);
                float specularMonochrome;
                float node_7716 = (i.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(i.uv0.r-(1.0 - i.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                float node_6204 = saturate((node_6222*_MovementAOMask));
                float3 diffuseColor = (lerp(_Color.rgb,_ColorFade.rgb,node_6204)*_MainTex_var.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
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
                float4 _OcclusionMap_var = tex2D(_OcclusionMap,TRANSFORM_TEX(i.uv0, _OcclusionMap));
                float node_20 = (_OcclusionMap_var.r*_OcclusionStrenght);
                float node_5420 = saturate(( node_6204 > 0.5 ? (1.0-(1.0-2.0*(node_6204-0.5))*(1.0-node_20)) : (2.0*node_6204*node_20) ));
                indirectDiffuse += ((_RimPower*pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*_RimColor.rgb*node_5420)+UNITY_LIGHTMODEL_AMBIENT.rgb); // Diffuse Ambient Light
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= node_5420; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float3 emissive = (_EmissionColor.rgb*_EmissionMap_var.rgb*_EmissionLM*_EmissionMap_var.a);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
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
            Cull Off
            
            
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float4 _EmissionColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed _UseAlpha;
            uniform sampler2D _OpacityMap; uniform float4 _OpacityMap_ST;
            uniform float _Cutoff;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _EmissionLM;
            uniform fixed _MetalAlphaGloss;
            uniform sampler2D _SmoothnessMap; uniform float4 _SmoothnessMap_ST;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform float _BumpScale;
            uniform float _LenghtAnim;
            uniform float _AnimIntensity;
            uniform float _WeightAnim;
            uniform float _MovementAOMask;
            uniform float _UpDownVelocity;
            uniform float _WorldNoiseVelocity;
            uniform sampler2D _WorldNoise; uniform float4 _WorldNoise_ST;
            uniform float4 _ColorFade;
            uniform float _WeightOffset;
            uniform float _LengthOffset;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_7435 = _Time;
                float2 node_8341 = ((node_7435.r*_WorldNoiseVelocity)*float2(mul(unity_ObjectToWorld, v.vertex).r,mul(unity_ObjectToWorld, v.vertex).b));
                float4 _WorldNoise_var = tex2Dlod(_WorldNoise,float4(TRANSFORM_TEX(node_8341, _WorldNoise),0.0,0));
                float node_7716 = (o.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(o.uv0.r-(1.0 - o.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                v.vertex.xyz += (((_WorldNoise_var.a*sin((node_7435.r*_UpDownVelocity))*node_6222)*_AnimIntensity*v.normal)+(v.normal*((node_8413*_LengthOffset)+(node_7716*_WeightOffset))));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = lerp(float3(0,0,1),_BumpMap_var.rgb,_BumpScale);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _OpacityMap_var = tex2D(_OpacityMap,TRANSFORM_TEX(i.uv0, _OpacityMap));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(saturate((lerp( _OpacityMap_var.a, _MainTex_var.a, _UseAlpha )*_Cutoff)) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _SmoothnessMap_var = tex2D(_SmoothnessMap,TRANSFORM_TEX(i.uv0, _SmoothnessMap));
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(i.uv0, _MetallicGlossMap));
                float gloss = (lerp( _SmoothnessMap_var.r, _MetallicGlossMap_var.a, _MetalAlphaGloss )*_Glossiness);
                float perceptualRoughness = 1.0 - (lerp( _SmoothnessMap_var.r, _MetallicGlossMap_var.a, _MetalAlphaGloss )*_Glossiness);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = (_MetallicGlossMap_var.r*_Metallic);
                float specularMonochrome;
                float node_7716 = (i.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(i.uv0.r-(1.0 - i.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                float node_6204 = saturate((node_6222*_MovementAOMask));
                float3 diffuseColor = (lerp(_Color.rgb,_ColorFade.rgb,node_6204)*_MainTex_var.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
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
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed _UseAlpha;
            uniform sampler2D _OpacityMap; uniform float4 _OpacityMap_ST;
            uniform float _Cutoff;
            uniform float _LenghtAnim;
            uniform float _AnimIntensity;
            uniform float _WeightAnim;
            uniform float _UpDownVelocity;
            uniform float _WorldNoiseVelocity;
            uniform sampler2D _WorldNoise; uniform float4 _WorldNoise_ST;
            uniform float _WeightOffset;
            uniform float _LengthOffset;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_7435 = _Time;
                float2 node_8341 = ((node_7435.r*_WorldNoiseVelocity)*float2(mul(unity_ObjectToWorld, v.vertex).r,mul(unity_ObjectToWorld, v.vertex).b));
                float4 _WorldNoise_var = tex2Dlod(_WorldNoise,float4(TRANSFORM_TEX(node_8341, _WorldNoise),0.0,0));
                float node_7716 = (o.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(o.uv0.r-(1.0 - o.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                v.vertex.xyz += (((_WorldNoise_var.a*sin((node_7435.r*_UpDownVelocity))*node_6222)*_AnimIntensity*v.normal)+(v.normal*((node_8413*_LengthOffset)+(node_7716*_WeightOffset))));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _OpacityMap_var = tex2D(_OpacityMap,TRANSFORM_TEX(i.uv0, _OpacityMap));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(saturate((lerp( _OpacityMap_var.a, _MainTex_var.a, _UseAlpha )*_Cutoff)) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float4 _EmissionColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float _Metallic;
            uniform float _Glossiness;
            uniform float _EmissionLM;
            uniform fixed _MetalAlphaGloss;
            uniform sampler2D _SmoothnessMap; uniform float4 _SmoothnessMap_ST;
            uniform sampler2D _MetallicGlossMap; uniform float4 _MetallicGlossMap_ST;
            uniform float _LenghtAnim;
            uniform float _AnimIntensity;
            uniform float _WeightAnim;
            uniform float _MovementAOMask;
            uniform float _UpDownVelocity;
            uniform float _WorldNoiseVelocity;
            uniform sampler2D _WorldNoise; uniform float4 _WorldNoise_ST;
            uniform float4 _ColorFade;
            uniform float _WeightOffset;
            uniform float _LengthOffset;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_7435 = _Time;
                float2 node_8341 = ((node_7435.r*_WorldNoiseVelocity)*float2(mul(unity_ObjectToWorld, v.vertex).r,mul(unity_ObjectToWorld, v.vertex).b));
                float4 _WorldNoise_var = tex2Dlod(_WorldNoise,float4(TRANSFORM_TEX(node_8341, _WorldNoise),0.0,0));
                float node_7716 = (o.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(o.uv0.r-(1.0 - o.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                v.vertex.xyz += (((_WorldNoise_var.a*sin((node_7435.r*_UpDownVelocity))*node_6222)*_AnimIntensity*v.normal)+(v.normal*((node_8413*_LengthOffset)+(node_7716*_WeightOffset))));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                o.Emission = (_EmissionColor.rgb*_EmissionMap_var.rgb*_EmissionLM*_EmissionMap_var.a);
                
                float node_7716 = (i.uv0.g*_WeightAnim);
                float node_8413 = saturate(abs(i.uv0.r-(1.0 - i.uv0.r)));
                float node_6222 = (node_7716+(node_8413*_LenghtAnim));
                float node_6204 = saturate((node_6222*_MovementAOMask));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffColor = (lerp(_Color.rgb,_ColorFade.rgb,node_6204)*_MainTex_var.rgb);
                float specularMonochrome;
                float3 specColor;
                float4 _MetallicGlossMap_var = tex2D(_MetallicGlossMap,TRANSFORM_TEX(i.uv0, _MetallicGlossMap));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, (_MetallicGlossMap_var.r*_Metallic), specColor, specularMonochrome );
                float4 _SmoothnessMap_var = tex2D(_SmoothnessMap,TRANSFORM_TEX(i.uv0, _SmoothnessMap));
                float roughness = 1.0 - (lerp( _SmoothnessMap_var.r, _MetallicGlossMap_var.a, _MetalAlphaGloss )*_Glossiness);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "ShaderForgeMaterialInspector"
}
