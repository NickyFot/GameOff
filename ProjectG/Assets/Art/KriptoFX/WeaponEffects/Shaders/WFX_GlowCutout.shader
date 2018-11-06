// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/GlowCutout" {
	Properties{
	[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
	_TimeScale("Time Scale", Vector) = (1,1,1,1)
	_MainTex("Noise Texture", 2D) = "white" {}
	_BorderScale("Border Scale (XY) Offset (Z)", Vector) = (0.5,0.05,1,0)
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
						Blend SrcAlpha OneMinusSrcAlpha
						Cull Off
						Lighting Off
						ZWrite Off

			SubShader {
				Pass {


					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					float4 _TintColor;
					float4 _TimeScale;
					float4 _BorderScale;

					struct appdata_t {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						float3 normal : NORMAL;
					};

					struct v2f {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						float3 normal : NORMAL;
						float3 worldPos : TEXCOORD1;
					};

					float4 _MainTex_ST;

					v2f vert(appdata_t v)
					{
						v2f o;
						v.vertex.xyz += v.normal / 100 * _BorderScale.z;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.color = v.color;
						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
						o.worldPos = v.vertex.xyz;
						o.normal = v.normal;
						return o;
					}

					half4 tex2DTriplanar(sampler2D tex, float2 offset, float3 worldPos, float3 normal)
					{
						half2 yUV = worldPos.xz * _MainTex_ST.x;
						half2 xUV = worldPos.zy * _MainTex_ST.x;
						half2 zUV = worldPos.xy * _MainTex_ST.x;

						half4 yDiff = tex2D(tex, yUV + offset);
						half4 xDiff = tex2D(tex, xUV + offset);
						half4 zDiff = tex2D(tex, zUV + offset);

						half3 blendWeights = pow(abs(normal), 1);

						blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

						return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
					}
					sampler2D _CameraDepthTexture;

					half4 frag(v2f i) : COLOR
					{
						half4 mask = tex2DTriplanar(_MainTex, _Time.xx * _TimeScale.xy, i.worldPos, i.normal);
						half4 tex = tex2DTriplanar(_MainTex, mask + _Time.xx * _TimeScale.zw, i.worldPos, i.normal);
						float4 res = 0;
						res.r = step(tex.r, _BorderScale.x);
						res.r -= step(tex.r, _BorderScale.x - _BorderScale.y);
						res.r *= tex.g;
						res = i.color * res.r * _TintColor;
						res.a = saturate(res.a);
						return  res;
					}
					ENDCG
				}
			}
	}

}