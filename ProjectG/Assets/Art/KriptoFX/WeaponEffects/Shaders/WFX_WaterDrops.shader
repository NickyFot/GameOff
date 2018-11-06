// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/WaterDrops" {
	Properties{
			_TintColor("Tint Color", Color) = (1,1,1,1)
			_MainTex("Base (RGB) Gloss (A)", 2D) = "black" {}
			_CutOut("CutOut (A)", 2D) = "black" {}
			_BumpMap("Normalmap", 2D) = "bump" {}
			_ColorStrength("Color Strength", Float) = 1
			_BumpAmt("Distortion", Float) = 10
	}

		Category{

			Tags { "Queue" = "Transparent"  "IgnoreProjector" = "True"  "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off}
			Offset -1, -1

			SubShader {
				GrabPass {
				//"_GrabTexture"
			}
			Pass {
				Name "BASE"
				Tags { "LightMode" = "Always" }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord: TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f {
		float4 vertex : POSITION;
		float4 grab : TEXCOORD0;
		float2 uvbump : TEXCOORD1;
		float2 uvmain : TEXCOORD2;
		float2 uvcutout : TEXCOORD3;
		fixed4 color : COLOR;
	};

	sampler2D _MainTex;
	sampler2D _CutOut;
	sampler2D _BumpMap;

	float _BumpAmt;
	float _ColorStrength;
	sampler2D _GrabTexture;
	fixed4 _TintColor;


	float4 _BumpMap_ST;
	float4 _MainTex_ST;
	float4 _CutOut_ST;

	v2f vert(appdata_full v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.color = v.color;
		#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
		#else
		float scale = 1.0;
		#endif
		o.grab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
		o.grab.zw = o.vertex.w;
	#if UNITY_SINGLE_PASS_STEREO
		o.grab.xy = TransformStereoScreenSpaceTex(o.grab.xy, o.grab.w);
	#endif
		o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
		o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.uvcutout = TRANSFORM_TEX(v.texcoord, _CutOut);

		return o;
	}

	sampler2D _CameraDepthTexture;
	float _InvFade;

	half4 frag(v2f i) : COLOR
	{
		#ifdef SOFTPARTICLES_ON
			float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
			float partZ = i.projPos.z;
			float fade = saturate(_InvFade * (sceneZ - partZ));
			float fadeStep = step(0.001, _InvFade);
			i.color.a *= lerp(1, fade, fadeStep);
		#endif

			// calculate perturbed coordinates
			half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg;
			float2 offset = bump * _BumpAmt * i.color.r;
			i.grab.xy = offset * i.grab.z + i.grab.xy;

			half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grab));
			//half4 tint = tex2D( _MainTex, i.uvmain );
			//return col * tint;
			fixed4 tex = tex2D(_MainTex, i.uvmain) * i.color;
			fixed4 cut = tex2D(_CutOut, i.uvcutout) * i.color;
			fixed4 emission = col + tex * _ColorStrength * _TintColor;
			emission.a = _TintColor.a * i.color.a * (cut.a);
			return emission;
		}
		ENDCG
				}
			}
			}

}

