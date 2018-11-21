// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GaryDave/Ocean_Simple"
{
	Properties
	{
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 0
		_Speed("Speed", Float) = 0.1
		_EdgeDistance("Edge Distance", Range( 0 , 10)) = 0
		_Spec("Spec", Float) = 0.73
		_Vector0("Vector 0", Vector) = (0,1,0,0)
		_EdgeColour("Edge Colour", Color) = (0.4716981,0.4716981,0.4716981,0)
		_WaterDepth("Water Depth", Float) = 0
		_WaveTile("Wave Tile", Vector) = (5,1,0,0)
		_NormalsTile("Normals Tile", Float) = 2
		_WaveSpeed("Wave Speed", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		AlphaToMask On
		GrabPass{ "_GrabScreen0" }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float3 _Vector0;
		uniform float2 _WaveTile;
		uniform float _WaveSpeed;
		uniform sampler2D _NormalMap;
		uniform float _NormalsTile;
		uniform float _Speed;
		uniform float _NormalStrength;
		uniform sampler2D _CameraDepthTexture;
		uniform float _WaterDepth;
		uniform sampler2D _GrabScreen0;
		uniform float4 _EdgeColour;
		uniform float _EdgeDistance;
		uniform float _Spec;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_0 = (32.0).xxxx;
			return temp_cast_0;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 panner149 = ( 1.0 * _Time.y * float2( 0,-0.1 ) + float2( 0,0 ));
			float2 uv_TexCoord141 = v.texcoord.xy * _WaveTile + ( _WaveSpeed * panner149 );
			float simplePerlin2D153 = snoise( uv_TexCoord141 );
			float2 panner174 = ( 1.0 * _Time.y * float2( 0,0.1 ) + float2( 0,0 ));
			float2 uv_TexCoord175 = v.texcoord.xy * _WaveTile + ( ( _WaveSpeed / 2.0 ) * panner174 );
			float simplePerlin2D176 = snoise( uv_TexCoord175 );
			float3 offset127 = ( _Vector0 * ( ( _SinTime.w * simplePerlin2D153 ) + ( simplePerlin2D176 * _SinTime.y ) ) );
			v.vertex.xyz += offset127;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_NormalsTile).xx;
			float2 panner37 = ( 1.0 * _Time.y * float2( 0,-0.5 ) + float2( 0,0 ));
			float2 uv_TexCoord36 = i.uv_texcoord * temp_cast_0 + ( panner37 * _Speed );
			float3 tex2DNode9 = UnpackNormal( tex2D( _NormalMap, uv_TexCoord36 ) );
			float2 temp_cast_1 = (( _NormalsTile * 5.0 )).xx;
			float2 panner90 = ( 1.0 * _Time.y * float2( 0,2 ) + float2( 0,0 ));
			float2 uv_TexCoord92 = i.uv_texcoord * temp_cast_1 + ( _Speed * panner90 );
			float4 lerpResult43 = lerp( float4(0.5019608,0.5019608,1,0) , float4( BlendNormals( tex2DNode9 , UnpackNormal( tex2D( _NormalMap, uv_TexCoord92 ) ) ) , 0.0 ) , _NormalStrength);
			float4 Normals56 = ( lerpResult43 * _NormalStrength );
			o.Normal = Normals56.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth192 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth192 = abs( ( screenDepth192 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _WaterDepth ) );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor21 = tex2D( _GrabScreen0, (ase_grabScreenPosNorm).xyzw.xy );
			float4 clampResult196 = clamp( ( ( 1.0 - distanceDepth192 ) * screenColor21 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 Albedo15 = clampResult196;
			o.Albedo = Albedo15.rgb;
			float screenDepth157 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth157 = abs( ( screenDepth157 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float2 panner166 = ( _Time.y * float2( 0.2,0.2 ) + float2( 0,0 ));
			float2 uv_TexCoord161 = i.uv_texcoord * float2( 80,80 ) + panner166;
			float simplePerlin2D160 = snoise( uv_TexCoord161 );
			float clampResult159 = clamp( ( ( 1.0 - distanceDepth157 ) - simplePerlin2D160 ) , 0.0 , 1.0 );
			float4 Emission154 = ( _EdgeColour * clampResult159 );
			o.Emission = Emission154.rgb;
			o.Metallic = 0.0;
			o.Smoothness = _Spec;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
384;80;1107;527;6210.951;-973.3915;1.979881;True;False
Node;AmplifyShaderEditor.CommentaryNode;189;-5123.521,1037.835;Float;False;3636.982;1124.281;Comment;19;149;174;175;141;176;180;136;153;178;179;177;132;152;127;239;246;248;249;250;Waves;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;249;-5040.22,1094.183;Float;False;Property;_WaveSpeed;Wave Speed;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;190;-3888.397,-290.1672;Float;False;2479.211;1065.204;Comment;17;56;33;43;10;42;34;1;65;92;91;9;36;8;90;38;37;39;Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;191;-3770.061,-2589.083;Float;False;2870.384;973.9268;Comment;12;154;170;171;159;165;160;158;161;157;156;166;167;Edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;167;-3720.062,-2539.083;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;240;-4394.548,-104.6807;Float;False;Property;_NormalsTile;Normals Tile;9;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;250;-5044.8,1638.632;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-3815.563,51.6737;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;0.1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;174;-5058.266,1863.411;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;37;-3834.225,-231.6266;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;149;-5078.557,1398.445;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-4389.405,275.82;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;90;-3831.262,320.2122;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-3699.078,-1960.857;Float;False;Property;_EdgeDistance;Edge Distance;3;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-4176.016,103.5663;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3522.682,293.4322;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;222;-3743.411,-1202.172;Float;False;2059.99;661.1801;Comment;9;12;17;21;195;196;15;194;192;193;Depth/Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3500.005,-60.22018;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-4719.809,1762.133;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;239;-4718.833,1458.965;Float;False;Property;_WaveTile;Wave Tile;8;0;Create;True;0;0;False;0;5,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;248;-4756.354,1191.159;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;166;-3523.646,-2335.186;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;161;-3148.242,-2397.347;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;80,80;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;157;-3337.449,-1962.364;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;193;-3450.246,-1152.172;Float;False;Property;_WaterDepth;Water Depth;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;12;-3693.411,-802.8347;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;175;-4450.476,1743.282;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;5,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;92;-3245.233,393.8705;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;10,10;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-3232.993,-195.4251;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;141;-4490.765,1385.236;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;5,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;8;-3251.167,151.1816;Float;True;Property;_NormalMap;Normal Map;0;0;Create;True;0;0;False;0;1b04b795694c7bd4a8e0d7e24af22d7a;1b04b795694c7bd4a8e0d7e24af22d7a;True;bump;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ComponentMaskNode;17;-3330.336,-781.4262;Float;False;True;True;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SinTimeNode;180;-4127.845,1983.116;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2928.841,165.6597;Float;True;Property;_Normals;Normals;2;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;192;-3173.423,-1130.791;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;160;-2847.633,-2345.762;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;136;-4203.192,1096.828;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;153;-4213.797,1370.206;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-2931.657,-80.71342;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;158;-2921.246,-1995.16;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;176;-4173.507,1728.252;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;42;-2484.471,-19.27861;Float;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;False;0;0.5019608,0.5019608,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;165;-2573.833,-2103.392;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;10;-2551.237,170.8247;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenColorNode;21;-2876.495,-813.8912;Float;False;Global;_GrabScreen0;Grab Screen 0;5;0;Create;True;0;0;False;0;Object;-1;True;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;194;-2873.387,-1087.773;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-2534.925,480.6693;Float;False;Property;_NormalStrength;Normal Strength;1;0;Create;True;0;0;False;0;0;0.584;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-3864.027,1255.254;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;-3815.666,1831.506;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-2543.529,-825.7365;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;171;-2267.151,-2169.051;Float;False;Property;_EdgeColour;Edge Colour;6;0;Create;True;0;0;False;0;0.4716981,0.4716981,0.4716981,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;132;-3004.677,1191.203;Float;False;Property;_Vector0;Vector 0;5;0;Create;True;0;0;False;0;0,1,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;177;-3517.655,1555.177;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;43;-2199.627,165.0701;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;159;-2242.508,-1931.5;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-2693.759,1535.266;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1922.667,-1948.139;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1911.535,381.7227;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;196;-2265.686,-816.8758;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-56.82852,403.8534;Float;False;56;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-32.92911,617.0234;Float;False;Constant;_Float3;Float 3;6;0;Create;True;0;0;False;0;32;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-220.2382,308.0793;Float;False;154;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-51.7529,162.5083;Float;False;Property;_Spec;Spec;4;0;Create;True;0;0;False;0;0.73;0.73;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-47.65982,69.73351;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;154;-1279.051,-1936.331;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;127;-2329.575,1546.638;Float;False;offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-58.62402,536.7428;Float;False;127;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-2534.287,-251.1766;Float;False;refractNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1625.287,362.4107;Float;False;Normals;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-63.74612,243.8153;Float;False;15;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-1926.422,-798.9922;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;402.283,240.9124;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;GaryDave/Ocean_Simple;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;12;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;250;0;249;0
WireConnection;241;0;240;0
WireConnection;241;1;242;0
WireConnection;91;0;39;0
WireConnection;91;1;90;0
WireConnection;38;0;37;0
WireConnection;38;1;39;0
WireConnection;246;0;250;0
WireConnection;246;1;174;0
WireConnection;248;0;249;0
WireConnection;248;1;149;0
WireConnection;166;1;167;0
WireConnection;161;1;166;0
WireConnection;157;0;156;0
WireConnection;175;0;239;0
WireConnection;175;1;246;0
WireConnection;92;0;241;0
WireConnection;92;1;91;0
WireConnection;36;0;240;0
WireConnection;36;1;38;0
WireConnection;141;0;239;0
WireConnection;141;1;248;0
WireConnection;17;0;12;0
WireConnection;1;0;8;0
WireConnection;1;1;92;0
WireConnection;192;0;193;0
WireConnection;160;0;161;0
WireConnection;153;0;141;0
WireConnection;9;0;8;0
WireConnection;9;1;36;0
WireConnection;158;0;157;0
WireConnection;176;0;175;0
WireConnection;165;0;158;0
WireConnection;165;1;160;0
WireConnection;10;0;9;0
WireConnection;10;1;1;0
WireConnection;21;0;17;0
WireConnection;194;0;192;0
WireConnection;178;0;136;4
WireConnection;178;1;153;0
WireConnection;179;0;176;0
WireConnection;179;1;180;2
WireConnection;195;0;194;0
WireConnection;195;1;21;0
WireConnection;177;0;178;0
WireConnection;177;1;179;0
WireConnection;43;0;42;0
WireConnection;43;1;10;0
WireConnection;43;2;34;0
WireConnection;159;0;165;0
WireConnection;152;0;132;0
WireConnection;152;1;177;0
WireConnection;170;0;171;0
WireConnection;170;1;159;0
WireConnection;33;0;43;0
WireConnection;33;1;34;0
WireConnection;196;0;195;0
WireConnection;154;0;170;0
WireConnection;127;0;152;0
WireConnection;65;0;9;0
WireConnection;56;0;33;0
WireConnection;15;0;196;0
WireConnection;0;0;14;0
WireConnection;0;1;57;0
WireConnection;0;2;155;0
WireConnection;0;3;6;0
WireConnection;0;4;7;0
WireConnection;0;11;128;0
WireConnection;0;14;106;0
ASEEND*/
//CHKSM=7C895768468776286D50FC2D7C6FC5EF93C3E18D