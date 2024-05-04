// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DLNK Shaders/ASE/Nature/LeavesAnim"
{
	Properties
	{
		_ColorA("Color A", Color) = (0.5943396,0.5943396,0.5943396,0)
		_ColorB("Color B", Color) = (1,1,1,0)
		_MainTex("Albedo", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_BumpMap("Normal", 2D) = "white" {}
		_Smoothness("Smoothness", Float) = 0
		_NormalScale("NormalScale", Float) = 0
		_MotionValue("Motion Value", Vector) = (1,1,0,0)
		_Noisespeed("Noise speed", Float) = 1
		_NoiseScale("Noise Scale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float2 _MotionValue;
		uniform float _Noisespeed;
		uniform float _NoiseScale;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap);
		uniform float4 _BumpMap_ST;
		SamplerState sampler_BumpMap;
		uniform float _NormalScale;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex);
		uniform float4 _MainTex_ST;
		SamplerState sampler_MainTex;
		uniform float4 _ColorA;
		uniform float4 _ColorB;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_42_0 = ( v.texcoord.xy.x + -0.5 );
			float4 appendResult27 = (float4(( temp_output_42_0 * _SinTime.y * _MotionValue.x ) , ( v.texcoord.xy.y * _CosTime.y * _MotionValue.y ) , 0.0 , 0.0));
			float4 appendResult39 = (float4(temp_output_42_0 , v.texcoord.xy.y , 0.0 , 0.0));
			float2 temp_cast_0 = (_Noisespeed).xx;
			float2 panner56 = ( 1.0 * _Time.y * temp_cast_0 + v.texcoord.xy);
			float simplePerlin2D57 = snoise( panner56*_NoiseScale );
			simplePerlin2D57 = simplePerlin2D57*0.5 + 0.5;
			v.vertex.xyz += ( appendResult27 * appendResult39 * simplePerlin2D57 ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			o.Normal = UnpackScaleNormal( SAMPLE_TEXTURE2D( _BumpMap, sampler_BumpMap, uv_BumpMap ), _NormalScale );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode7 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, uv_MainTex );
			float temp_output_42_0 = ( i.uv_texcoord.x + -0.5 );
			float4 appendResult39 = (float4(temp_output_42_0 , i.uv_texcoord.y , 0.0 , 0.0));
			float4 lerpResult49 = lerp( _ColorA , _ColorB , appendResult39);
			o.Albedo = ( tex2DNode7 * lerpResult49 ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			clip( tex2DNode7.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "DLNKShaders/Nature/LeavesAnim"
}
/*ASEBEGIN
Version=18400
0;0;1920;1019;1310.438;495.8267;1.261798;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-540.1729,355.8477;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-447.1729,846.8477;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;False;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-282.3127,895.4478;Inherit;False;Property;_Noisespeed;Noise speed;8;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosTime;28;-601.1729,495.8477;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;40;-476.1775,654.2297;Inherit;False;Property;_MotionValue;Motion Value;7;0;Create;True;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-239.1729,716.8477;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;24;-460.1729,490.8477;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-267.1729,501.8477;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;51;-247.9164,-463.9396;Inherit;False;Property;_ColorB;Color B;1;0;Create;True;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;39;67.82715,598.8477;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;50;-256.9837,-640.1237;Inherit;False;Property;_ColorA;Color A;0;0;Create;True;0;0;False;0;False;0.5943396,0.5943396,0.5943396,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;56;-98.0901,827.3105;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-270.1729,371.8477;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-265.9093,977.4645;Inherit;False;Property;_NoiseScale;Noise Scale;9;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-533,-255;Inherit;True;Property;_MainTex;Albedo;2;0;Create;False;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;57;132.8189,786.9335;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-73.17285,393.8477;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-713.1729,223.8477;Inherit;False;Property;_NormalScale;NormalScale;6;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;49;-30.78362,-500.3326;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;128.0871,-252.9495;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-106.7086,118.1799;Inherit;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;97.82715,383.8477;Inherit;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;11;-486,49.5;Inherit;True;Property;_BumpMap;Normal;4;0;Create;False;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;301.2,-172.1;Float;False;True;-1;2;DLNKShaders/Nature/LeavesAnim;0;0;Standard;DLNK Shaders/ASE/Nature/LeavesAnim;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;True;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;18;1
WireConnection;42;1;43;0
WireConnection;26;0;18;2
WireConnection;26;1;28;2
WireConnection;26;2;40;2
WireConnection;39;0;42;0
WireConnection;39;1;18;2
WireConnection;56;0;18;0
WireConnection;56;2;59;0
WireConnection;25;0;42;0
WireConnection;25;1;24;2
WireConnection;25;2;40;1
WireConnection;57;0;56;0
WireConnection;57;1;60;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;49;0;50;0
WireConnection;49;1;51;0
WireConnection;49;2;39;0
WireConnection;48;0;7;0
WireConnection;48;1;49;0
WireConnection;32;0;27;0
WireConnection;32;1;39;0
WireConnection;32;2;57;0
WireConnection;11;5;16;0
WireConnection;0;0;48;0
WireConnection;0;1;11;0
WireConnection;0;4;55;0
WireConnection;0;10;7;4
WireConnection;0;11;32;0
ASEEND*/
//CHKSM=0C76B1CCA094B0A914F1E4B627229D6B904AD8A6