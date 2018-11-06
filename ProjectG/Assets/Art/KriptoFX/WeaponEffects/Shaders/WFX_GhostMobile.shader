// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/GhostMobile" {
	Properties{
			_TintColor("Tint Color", Color) = (1,1,1,1)
			_RimColor("Rim Color", Color) = (1,1,1,0.5)
			_MainTex("Main Texture", 2D) = "white" {}
			_BumpMap("Normalmap", 2D) = "bump" {}
			_Height("_Height", Float) = 0.2
			_Speed("Speed (x, y)", Vector) = (1,0,0,0)
			_FPOW("FPOW Fresnel", Float) = 5.0
			_R0("R0 Fresnel", Float) = 0.05
			_BumpAmt("Distortion", Float) = 1
	}
		Category{

			Tags { "Queue" = "Transparent"  "IgnoreProjector" = "True"  "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off

			SubShader {
				Pass {
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					//#pragma target 3.0
					//#pragma glsl

					#include "UnityCG.cginc"
					#pragma multi_compile DISTORT_ON DISTORT_OFF

					sampler2D _BumpMap;
					sampler2D _MainTex;

					float _BumpAmt;
					sampler2D _GrabTextureMobile;
					float4 _GrabTextureMobile_TexelSize;

					float4 _TintColor;
					float4 _RimColor;
					float _Shininess;
					float _FPOW;
					float _R0;
					float _Height;
					float4 _Speed;
					float _DistortFixScale;

					struct appdata_t {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f {
						half4 vertex : POSITION;
						half2 uv_BumpMap : TEXCOORD1;
						half2 uv_MainTex : TEXCOORD2;
						half4 grab : TEXCOORD3;
						fixed4 color : COLOR;
						half3 normal: TEXCOORD4;
						half3 viewDir : TEXCOORD5;
					};

					float4 _BumpMap_ST;
					float4 _MainTex_ST;

					v2f vert(appdata_full v)
					{
						v2f o;

						o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap) + _Time.xx * _Speed;
						o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

						float4 oPos = UnityObjectToClipPos(v.vertex);

						o.vertex = UnityObjectToClipPos(v.vertex);

						o.grab.xy = (float2(oPos.x, oPos.y * _ProjectionParams.x) + oPos.w) * 0.5;
						o.grab.zw = oPos.w;
		#if UNITY_SINGLE_PASS_STEREO
						o.grab.xy = TransformStereoScreenSpaceTex(o.grab.xy, o.grab.w);
		#endif
						o.color = v.color;
						o.normal = v.normal;

						o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
						return o;
					}

					fixed4 frag(v2f i) : COLOR
					{
						#ifdef DISTORT_OFF 
							return 0;
						#endif
						fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));

						half fresnelRim = saturate(1 - dot(i.normal, i.viewDir));
						fresnelRim = pow(fresnelRim, _FPOW);
						fresnelRim = 1 - saturate(_R0 + (1.0 - _R0) * fresnelRim);
						fresnelRim *= fresnelRim * fresnelRim + fresnelRim;

						half2 offset = normal.rg * _BumpAmt * _GrabTextureMobile_TexelSize.xy * _DistortFixScale;
						i.grab.xy = offset * i.grab.z + i.grab.xy;
						half4 col = tex2Dproj(_GrabTextureMobile, UNITY_PROJ_COORD(i.grab));
						half4 tex = tex2D(_MainTex, i.uv_MainTex + offset);

						float gray = 0.299*tex.r + 0.587*tex.g + 0.114*tex.b;
						fixed3 emission = col.rgb * _TintColor.xyz + gray;

						emission = lerp(col, emission, fresnelRim * 2);
						return fixed4(emission, saturate(_TintColor.a * fresnelRim));
					}
					ENDCG
				}
			}
			}//FallBack "Legacy Shaders/Diffuse"

}