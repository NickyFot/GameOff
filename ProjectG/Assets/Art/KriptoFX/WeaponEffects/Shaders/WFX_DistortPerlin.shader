// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/DistortPerlin" {
	Properties{
			_TintColor("Main Color", Color) = (1,1,1,1)
			_RimColor("Rim Color", Color) = (1,1,1,0.5)
			_BumpMap("Normalmap", 2D) = "bump" {}
			_PerlinNoise("Perlin Noise Map (r)", 2D) = "white" {}
			_DropWavesScale("Waves Scale (X) Height (YZ) Time (W)", Vector) = (1, 1, 1, 1)
			_NoiseScale("Noize Scale (XYZ) Height (W)", Vector) = (1, 1, 1, 0.2)
			_Speed("Distort Direction Speed (XY)", Vector) = (1,0,0,0)
			_FPOW("FPOW Fresnel", Float) = 5.0
			_R0("R0 Fresnel", Float) = 0.05
			_BumpAmt("Distortion Scale", Float) = 10
	}
		Category{

			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
						Blend SrcAlpha OneMinusSrcAlpha
						ZWrite Off
						Cull Back

			SubShader {
				GrabPass {
					"_GrabTexture"
				}
				Pass {
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 3.0

					#include "UnityCG.cginc"

					sampler2D _BumpMap;
					sampler2D _PerlinNoise;
					sampler2D _GrabTexture;

					float4 _GrabTexture_TexelSize;
					float4 _TintColor;
					float4 _RimColor;
					float4 _Speed;
					float4 _DropWavesScale;
					float4 _NoiseScale;
					float4 _BumpMap_ST;
					float4 _Height_ST;
					float4 _LightColor0;

					float _BumpAmt;
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
						half2 uv_BumpMap : TEXCOORD0;
						half4 grab : TEXCOORD1;
						half3 normal: TEXCOORD2;
						half3 viewDir : TEXCOORD3;
						fixed4 color : COLOR;
					};

					v2f vert(appdata_full v)
					{
						v2f o;

						//////// Displacemnt by noise texture (rgb) and drop waves (a)
						float4 oPos = UnityObjectToClipPos(v.vertex);
						float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;

						float4 coordNoise = float4(wpos * _NoiseScale.xyz, 0);
						float4 coordDisplDrop = float4(wpos * _DropWavesScale.x, 0);
						float4 tex1 = tex2Dlod(_PerlinNoise, coordNoise + float4(_Time.x * 2, _Time.x * 4, _Time.x * 1.5, 0) * _DropWavesScale);
						float4 tex2 = tex2Dlod(_PerlinNoise, coordDisplDrop);
						v.vertex.xyz += v.normal * _DropWavesScale.y * (tex2.a * 2 - 0.5) * 0.01;
						v.vertex.xyz += v.normal*(_DropWavesScale.z * 0.005) + tex1.rgb * _NoiseScale.w - _NoiseScale.w / 2;

						o.vertex = UnityObjectToClipPos(v.vertex);
						//////////////////////////////////////////////////////////////

						#if UNITY_UV_STARTS_AT_TOP
							float scale = -1.0;
						#else
							float scale = 1.0;
						#endif

						oPos += o.vertex;
						o.grab.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
						o.grab.zw = oPos.w;
		#if UNITY_SINGLE_PASS_STEREO
						o.grab.xy = TransformStereoScreenSpaceTex(o.grab.xy, o.grab.w);
		#endif

						o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap) + _Time.xx * _Speed.xy * _DropWavesScale;
						o.color = v.color;
						o.normal = v.normal;

						o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
						return o;
					}

					fixed4 frag(v2f i) : COLOR
					{
						fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));

						half fresnelRim = saturate(1 - dot(i.normal, i.viewDir));
						fresnelRim = pow(fresnelRim, _FPOW);
						fresnelRim = saturate(_R0 + (1.0 - _R0) * fresnelRim);
						fresnelRim = fresnelRim*fresnelRim + fresnelRim;

						half2 offset = normal.rg * _BumpAmt * _GrabTexture_TexelSize.xy * i.color.a;
						i.grab.xy = offset * i.grab.z + i.grab.xy;
						half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grab));
						half3 emission = _RimColor * _LightColor0.rgb * _LightColor0.w * 2 * i.color.rgb;
						emission = lerp(col.xyz * _TintColor.xyz, col.xyz * emission + emission / 2, saturate(fresnelRim));
						return fixed4(emission, _TintColor.a * i.color.a);
					}
					ENDCG
				}
			}
			}
}