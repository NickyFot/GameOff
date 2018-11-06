// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/Ice" {
	Properties{
			_TintColor("Main Color", Color) = (1,1,1,1)
			_RimColor("Rim Color", Color) = (1,1,1,0.5)
			_MainTex("MainTex (r)", 2D) = "black" {}
			_BumpMap("Normalmap", 2D) = "bump" {}
			_HeightMap("_HeightMap (r)", 2D) = "black" {}
			_Height("_Height", Float) = 0.1
			_FPOW("FPOW Fresnel", Float) = 5.0
			_R0("R0 Fresnel", Float) = 0.05
			_BumpAmt("Distortion", Float) = 10
			_IceStrength("Ice Strength", Float) = 2
	}
		Category{

			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
						Blend SrcAlpha OneMinusSrcAlpha
						ZWrite Off
						Offset -1,-1
						Cull Back

			SubShader {
				GrabPass {
					Name "BASE"
					Tags { "LightMode" = "Always" }
				}
				Pass {
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 3.0

					#include "UnityCG.cginc"

					sampler2D _BumpMap;
					sampler2D _HeightMap;
					sampler2D _MainTex;

					float _BumpAmt;
					sampler2D _GrabTexture;
					float4 _GrabTexture_TexelSize;

					float4 _TintColor;
					float4 _RimColor;
					float _FPOW;
					float _R0;
					float _Cutoff;
					float _Height;
					float _IceStrength;
					float4 _LightColor0;

					struct appdata_t {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f {
						half4 vertex : POSITION;
						half2 uv_MainTex : TEXCOORD1;
						half2 uv_BumpMap : TEXCOORD2;
						half2 uv_Height : TEXCOORD3;
						half4 grab : TEXCOORD4;
						fixed4 color : COLOR;
						half3 normal: TEXCOORD5;
						half3 viewDir : TEXCOORD6;
					};

					float4 _BumpMap_ST;
					float4 _Height_ST;
					float4 _MainTex_ST;

					v2f vert(appdata_full v)
					{
						v2f o;

						o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
						o.uv_Height = TRANSFORM_TEX(v.texcoord, _Height);
						o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

						float4 oPos = UnityObjectToClipPos(v.vertex);

						float4 coord = float4(v.texcoord.xy, 0 ,0);
						float4 tex = tex2Dlod(_HeightMap, coord);
						v.vertex.xyz += v.normal * _Height * tex.r;

						o.vertex = UnityObjectToClipPos(v.vertex);

						#if UNITY_UV_STARTS_AT_TOP
							float scale = -1.0;
						#else
							float scale = 1.0;
						#endif
						o.grab.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
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
						fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));

						half fresnelRim = saturate(0.7 - dot(i.normal, i.viewDir));
						fresnelRim = pow(fresnelRim, _FPOW);
						fresnelRim = _R0 + (1.0 - _R0) * fresnelRim;

						half2 offset = normal.rg * _BumpAmt * _GrabTexture_TexelSize.xy;
						i.grab.xy = offset * i.grab.z + i.grab.xy;
						half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grab));

						half4 tex = tex2D(_MainTex, i.uv_MainTex);
						fixed3 emission = col.xyz* _TintColor.xyz + saturate(fresnelRim * _RimColor + tex.r * tex.g * _TintColor.xyz * _IceStrength) * i.color.a;
						float3 light = _LightColor0.rgb * _LightColor0.w;
						emission.rgb *= saturate(light * 2);
						return fixed4(emission, _TintColor.a * i.color.a);
					}
					ENDCG
				}
			}
			}
}