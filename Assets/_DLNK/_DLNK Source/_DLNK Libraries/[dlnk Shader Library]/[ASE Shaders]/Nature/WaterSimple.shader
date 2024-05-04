// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DLNK Shaders/ASE/Nature/WaterSimple"
{
	Properties
	{
		_UVScale("UVScale", Float) = 1
		_ColorA("Color A", Color) = (0.2971698,0.6247243,1,0)
		_ColorB("Color B", Color) = (0.09838911,0.1034623,0.3113208,0)
		_NormalA("Normal A", 2D) = "bump" {}
		_NormalB("Normal B", 2D) = "bump" {}
		_NormalScale("NormalScale", Float) = 1
		_SpecXYSnsZW("Spec(XY)Sns(ZW)", Vector) = (0.1,0,0.5,0.2)
		_VelocityXYFoamZ("Velocity(XY)Foam(Z)", Vector) = (0.03,-0.05,0.04,0)
		_Depth("Depth", Float) = 0.9
		_Falloff("Falloff", Float) = -3
		_Distorsion("Distorsion", Float) = 0.1
		_ColorFoam("ColorFoam", Color) = (0.9386792,0.9671129,1,0)
		_FoamMask("FoamMask", 2D) = "white" {}
		_FoamTiling("FoamTiling", Float) = 1
		_FoamDepth("FoamDepth", Float) = 0.9
		_FoamFalloff("FoamFalloff", Float) = -3
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf StandardSpecular keepalpha 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform sampler2D _NormalA;
		uniform half3 _VelocityXYFoamZ;
		uniform half _UVScale;
		uniform half _NormalScale;
		uniform sampler2D _NormalB;
		uniform half4 _ColorA;
		uniform half4 _ColorB;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _Depth;
		uniform half _Falloff;
		uniform half4 _ColorFoam;
		uniform half _FoamDepth;
		uniform half _FoamFalloff;
		uniform sampler2D _FoamMask;
		SamplerState sampler_FoamMask;
		uniform half _FoamTiling;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform half _Distorsion;
		uniform half4 _SpecXYSnsZW;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			half2 temp_cast_0 = (_VelocityXYFoamZ.x).xx;
			float3 ase_worldPos = i.worldPos;
			half4 appendResult34 = (half4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			half4 temp_output_35_0 = ( appendResult34 * _UVScale );
			half2 panner12 = ( 1.0 * _Time.y * temp_cast_0 + temp_output_35_0.xy);
			half2 temp_cast_2 = (_VelocityXYFoamZ.y).xx;
			half2 panner13 = ( 1.0 * _Time.y * temp_cast_2 + temp_output_35_0.xy);
			half3 temp_output_17_0 = BlendNormals( UnpackScaleNormal( tex2D( _NormalA, panner12 ), _NormalScale ) , UnpackScaleNormal( tex2D( _NormalB, panner13 ), _NormalScale ) );
			o.Normal = temp_output_17_0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			half eyeDepth3 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			half temp_output_5_0 = abs( ( eyeDepth3 - ase_screenPos.w ) );
			half temp_output_10_0 = saturate( pow( ( temp_output_5_0 + _Depth ) , _Falloff ) );
			half4 lerpResult27 = lerp( _ColorA , _ColorB , temp_output_10_0);
			half2 temp_cast_4 = (_VelocityXYFoamZ.z).xx;
			half2 panner61 = ( 1.0 * _Time.y * temp_cast_4 + ( temp_output_35_0 * _FoamTiling ).xy);
			half temp_output_63_0 = ( saturate( pow( ( temp_output_5_0 + _FoamDepth ) , _FoamFalloff ) ) * tex2D( _FoamMask, panner61 ).r );
			half4 lerpResult64 = lerp( lerpResult27 , _ColorFoam , temp_output_63_0);
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			half4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			half4 screenColor43 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( half3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( temp_output_17_0 * _Distorsion ) ).xy);
			half4 lerpResult44 = lerp( lerpResult64 , screenColor43 , temp_output_10_0);
			o.Albedo = lerpResult44.rgb;
			half lerpResult67 = lerp( _SpecXYSnsZW.x , _SpecXYSnsZW.y , temp_output_63_0);
			half3 temp_cast_9 = (lerpResult67).xxx;
			o.Specular = temp_cast_9;
			half lerpResult68 = lerp( _SpecXYSnsZW.z , _SpecXYSnsZW.w , temp_output_63_0);
			o.Smoothness = lerpResult68;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
0;6;1920;1013;893.627;389.041;1.3;True;True
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-525.4226,88.02217;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;33;-721.8076,590.5832;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScreenPosInputsNode;1;-527.4226,262.0221;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;3;-524.4226,5.022179;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-539.2219,722.4264;Inherit;False;Property;_UVScale;UVScale;0;0;Create;True;0;0;False;0;False;1;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;34;-538.2219,572.4264;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;4;-287.4228,49.02216;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;62;-531.2057,824.3724;Inherit;False;Property;_VelocityXYFoamZ;Velocity(XY)Foam(Z);7;0;Create;True;0;0;False;0;False;0.03,-0.05,0.04;0.03,-0.015,0.05;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-380.2219,622.4264;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-118.2469,903.2474;Inherit;False;Property;_FoamDepth;FoamDepth;14;0;Create;True;0;0;False;0;False;0.9;-3.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;5;-277.4228,147.0222;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-103.943,160.8063;Inherit;False;Property;_Depth;Depth;8;0;Create;True;0;0;False;0;False;0.9;-0.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-14.10339,615.3041;Inherit;False;Property;_NormalScale;NormalScale;5;0;Create;True;0;0;False;0;False;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;13;-23.10339,473.3041;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;12;-17.80053,319.5826;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-459.6713,1019.83;Inherit;False;Property;_FoamTiling;FoamTiling;13;0;Create;True;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;38.63753,897.5466;Inherit;False;Property;_FoamFalloff;FoamFalloff;15;0;Create;True;0;0;False;0;False;-3;-1.78;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-292.6713,981.8298;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-84.44211,59.47009;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;52.94149,155.1055;Inherit;False;Property;_Falloff;Falloff;9;0;Create;True;0;0;False;0;False;-3;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-98.74604,801.9112;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;153.8966,451.3041;Inherit;True;Property;_NormalB;Normal B;4;0;Create;True;0;0;False;0;False;-1;c19b1da79c825924b9d57bdeae872539;c19b1da79c825924b9d57bdeae872539;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;151.4966,261.604;Inherit;True;Property;_NormalA;Normal A;3;0;Create;True;0;0;False;0;False;-1;054d24f51dd34704f82c7edd81e94a43;054d24f51dd34704f82c7edd81e94a43;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;61;-163.2056,1017.173;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BlendNormalsNode;17;446.8966,387.3041;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;42;120.1355,-461.7363;Inherit;False;Property;_Distorsion;Distorsion;10;0;Create;True;0;0;False;0;False;0.1;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;37;15.2009,-639.9988;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;8;53.47051,54.2658;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;58;39.16655,796.7069;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-34.22198,-361.5736;Inherit;False;Property;_ColorA;Color A;1;0;Create;True;0;0;False;0;False;0.2971698,0.6247243,1,0;0.1504984,0.2474039,0.3584906,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;60;56.99449,998.5726;Inherit;True;Property;_FoamMask;FoamMask;12;0;Create;True;0;0;False;0;False;-1;None;9b800aabfad72b44fb51b9ef18670860;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;308.5119,-536.3284;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;39;231.3911,-619.7705;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;59;208.6602,890.1505;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-31.47352,-184.2734;Inherit;False;Property;_ColorB;Color B;2;0;Create;True;0;0;False;0;False;0.09838911,0.1034623,0.3113208,0;0.1541919,0.3301887,0.2697903,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;10;203.4642,51.50956;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;476.6599,-599.5419;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;27;267.0054,-309.5601;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;395.6944,950.4725;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;65;206.3709,-142.317;Inherit;False;Property;_ColorFoam;ColorFoam;11;0;Create;True;0;0;False;0;False;0.9386792,0.9671129,1,0;0.8773585,0.8773585,0.8773585,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;43;608.1441,-633.6773;Inherit;False;Global;_GrabScreen1;Grab Screen 1;9;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;64;529.1179,-180.1812;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;66;560.9224,513.2822;Inherit;False;Property;_SpecXYSnsZW;Spec(XY)Sns(ZW);6;0;Create;True;0;0;False;0;False;0.1,0,0.5,0.2;0.1,0,0.5,0.2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;67;768.2148,79.30556;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;817.1596,-251.1686;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;68;771.035,261.1941;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1004.598,-75.80824;Half;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;DLNK Shaders/ASE/Nature/WaterSimple;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;False;0;False;Opaque;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;0
WireConnection;34;0;33;1
WireConnection;34;1;33;3
WireConnection;4;0;3;0
WireConnection;4;1;1;4
WireConnection;35;0;34;0
WireConnection;35;1;36;0
WireConnection;5;0;4;0
WireConnection;13;0;35;0
WireConnection;13;2;62;2
WireConnection;12;0;35;0
WireConnection;12;2;62;1
WireConnection;69;0;35;0
WireConnection;69;1;70;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;57;0;5;0
WireConnection;57;1;55;0
WireConnection;25;1;13;0
WireConnection;25;5;26;0
WireConnection;21;1;12;0
WireConnection;21;5;26;0
WireConnection;61;0;69;0
WireConnection;61;2;62;3
WireConnection;17;0;21;0
WireConnection;17;1;25;0
WireConnection;8;0;6;0
WireConnection;8;1;9;0
WireConnection;58;0;57;0
WireConnection;58;1;56;0
WireConnection;60;1;61;0
WireConnection;41;0;17;0
WireConnection;41;1;42;0
WireConnection;39;0;37;0
WireConnection;59;0;58;0
WireConnection;10;0;8;0
WireConnection;40;0;39;0
WireConnection;40;1;41;0
WireConnection;27;0;28;0
WireConnection;27;1;29;0
WireConnection;27;2;10;0
WireConnection;63;0;59;0
WireConnection;63;1;60;1
WireConnection;43;0;40;0
WireConnection;64;0;27;0
WireConnection;64;1;65;0
WireConnection;64;2;63;0
WireConnection;67;0;66;1
WireConnection;67;1;66;2
WireConnection;67;2;63;0
WireConnection;44;0;64;0
WireConnection;44;1;43;0
WireConnection;44;2;10;0
WireConnection;68;0;66;3
WireConnection;68;1;66;4
WireConnection;68;2;63;0
WireConnection;0;0;44;0
WireConnection;0;1;17;0
WireConnection;0;3;67;0
WireConnection;0;4;68;0
ASEEND*/
//CHKSM=EF0148B9F941BAAB2BA718210EEC47D8AFDB49B9