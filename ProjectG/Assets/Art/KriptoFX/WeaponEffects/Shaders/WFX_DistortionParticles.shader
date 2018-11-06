// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/FPS_Pack/DistortionParticles" {
	Properties{
			_MainTex("Normalmap (RG) & CutOut (B)", 2D) = "black" {}
			_BumpAmt("Distortion", Float) = 10
			_InvFade("Soft Particles Factor", Float) = 0.5
	}

		Category{

			Tags { "Queue" = "Transparent"  "IgnoreProjector" = "True"  "RenderType" = "Opaque" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off}

			SubShader {
				GrabPass {
					"_GrabTexture"
				}
				Pass {
					Name "BASE"
					Tags { "LightMode" = "Always" }

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_particles
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
			fixed4 color : COLOR;
			#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD3;
			#endif
			#if UNITY_VERSION >= 520
			float2 dist:TEXCOORD4;
			#endif
		};

		sampler2D _MainTex;

		float _BumpAmt;
		sampler2D _GrabTexture;
		float4 _GrabTexture_TexelSize;


		float4 _MainTex_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
			#endif
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
			o.uvbump = TRANSFORM_TEX(v.texcoord, _MainTex);
			#if UNITY_VERSION >= 520
			o.dist.x = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
			o.dist.x = saturate(0.01 + 1 / (o.dist.x * o.dist.x / 15));
			o.dist.y = 0;
			#endif
			return o;
		}

		sampler2D _CameraDepthTexture;
		float _InvFade;

		half4 frag(v2f i) : COLOR
		{
			#ifdef SOFTPARTICLES_ON
				if (_InvFade > 0.0001) {
				float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
				float partZ = i.projPos.z;
				float fade = saturate(_InvFade * (sceneZ - partZ));
				i.color.a *= fade;
			}
			#endif

			half3 bump = (tex2D(_MainTex, i.uvbump)).rgb;
			bump.rg = bump.rg * 2 - 1;
			float2 offset;
			offset = bump.rg * _BumpAmt * _GrabTexture_TexelSize.xy * i.color.a * i.dist.x;
			i.grab.xy = offset * i.grab.z * bump.b + i.grab.xy;

			half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grab));
			col.a = saturate(col.a * bump.b);
			return col;
		}
		ENDCG
				}
			}
			}

}
