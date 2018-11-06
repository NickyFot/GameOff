// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/SlimeMobile" {
	Properties{
			_TintColor("Main Color", Color) = (1,1,1,1)
			_MainTex("Base (RGB) Emission Tex (A)", 2D) = "white" {}
			_CutOut("CutOut (A)", 2D) = "white" {}
			_BumpMap("Normalmap", 2D) = "bump" {}
			_BumpAmt("Distortion", Float) = 10
	}
		Category{

			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
						Blend SrcAlpha OneMinusSrcAlpha
						ZWrite Off
						Offset -1,-1
						Cull Off
						Fog { Mode Off}

			SubShader {
				Pass {
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					//#pragma target 3.0

					#include "UnityCG.cginc"
					#pragma multi_compile DISTORT_ON DISTORT_OFF

					sampler2D _MainTex;
					sampler2D _BumpMap;
					sampler2D _CutOut;
					samplerCUBE _Cube;

					float _BumpAmt;
					sampler2D _GrabTextureMobile;
					float4 _GrabTextureMobile_TexelSize;

					float4 _TintColor;
					float _FPOW;
					float _R0;

					struct appdata_t {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f {
						half4 vertex : POSITION;
						half2 uv_MainTex: TEXCOORD0;
						half2 uv_BumpMap : TEXCOORD1;
						half2 uv_CutOut : TEXCOORD2;
						half4 grab : TEXCOORD3;
						fixed4 color : COLOR;
					};

					float4 _MainTex_ST;
					float4 _BumpMap_ST;
					float4 _CutOut_ST;

					v2f vert(appdata_full v)
					{
						v2f o;

						o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
						o.uv_CutOut = TRANSFORM_TEX(v.texcoord, _CutOut);

						o.vertex = UnityObjectToClipPos(v.vertex);

						o.grab.xy = (half2(o.vertex.x, o.vertex.y * _ProjectionParams.x) + o.vertex.w) * 0.5;
						o.grab.zw = o.vertex.w;
		#if UNITY_SINGLE_PASS_STEREO
						o.grab.xy = TransformStereoScreenSpaceTex(o.grab.xy, o.grab.w);
		#endif

						o.color = v.color;

						return o;
					}

					fixed4 frag(v2f i) : COLOR
					{
					#ifdef DISTORT_OFF 
						return 0;
					#endif
						half4 tex = tex2D(_MainTex, i.uv_MainTex);
						half4 c = tex * _TintColor;
						half4 cut = tex2D(_CutOut, i.uv_CutOut);

						half3 normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));

						half2 offset = normal.rg * _BumpAmt * _GrabTextureMobile_TexelSize.xy * i.color.a;
						i.grab.xy = offset * i.grab.z + i.grab.xy;
						half4 col = tex2Dproj(_GrabTextureMobile, UNITY_PROJ_COORD(i.grab));

						fixed gray = col.r * 0.3 + col.g * 0.59 + col.b * 0.11;
						half3 emission = col.rgb*_TintColor.rgb;

						return fixed4(emission, cut.a * _TintColor.a * i.color.r * i.color.a);
					}
					ENDCG
				}
			}
			}
}