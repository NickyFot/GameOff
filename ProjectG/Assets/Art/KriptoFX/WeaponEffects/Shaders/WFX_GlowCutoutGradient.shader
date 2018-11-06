// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/GlowCutoutGradient" {
	Properties{
	[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
	_TimeScale("Time Scale", Vector) = (1,1,1,1)
	_MainTex("Noise Texture", 2D) = "white" {}
	_BorderScale("Border Scale (XY) Offset (Z)", Vector) = (0.5,0.05,1,1)
	}

		SubShader{
			Pass {
				Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
				Blend SrcAlpha One
				Cull Off
				Lighting Off
				ZWrite Off
				Offset -1, -1

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
					float3 worldPos : TEXCOORD1;
					float3 normal : NORMAL;
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

				sampler2D _CameraDepthTexture;

				half4 tex2DTriplanar(sampler2D tex, float2 offset, float3 worldPos, float3 normal)
				{
					half2 yUV = worldPos.xz;
					half2 xUV = worldPos.zy;
					half2 zUV = worldPos.xy;

					half4 yDiff = tex2D(tex, yUV * _MainTex_ST.xy + offset);
					half4 xDiff = tex2D(tex, xUV * _MainTex_ST.xy + offset);
					half4 zDiff = tex2D(tex, zUV * _MainTex_ST.xy + offset);

					half3 blendWeights = abs(normal);

					blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z)*1.1;

					return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
				}

				half4 frag(v2f i) : COLOR
				{
					half4 mask = tex2DTriplanar(_MainTex, _Time.xx * _TimeScale.xy, i.worldPos, i.normal);
					half4 tex = tex2DTriplanar(_MainTex, _Time.xx * _TimeScale.zw, i.worldPos, i.normal);
					half alphaMask = tex2DTriplanar(_MainTex, 0.3, i.worldPos, i.normal).b;
					//tex *= mask;
					float4 res = 0;
					//res.r = tex.r * step(tex.r, _BorderScale.x);
					//res.r -= tex.r * step(tex.r, _BorderScale.x - _BorderScale.y);
					//res.g = tex.r * step(tex.g, _BorderScale.x);
					//res.g -= step(tex.r, _BorderScale.x - _BorderScale.y);
					//res.r *= tex.g;
					res.r = tex.r * mask.r;
					res = i.color * _TintColor * lerp(0, res.r, alphaMask);
					res.rgb = pow(res.rgb, _BorderScale.w);
					res.a = saturate(res.a * 4);
					return  res;
				}
				ENDCG
			}

			Pass {
				Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
				Blend DstColor Zero
				Cull Off
				Lighting Off
				ZWrite Off
				Offset -1, -1

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
					float3 worldPos : TEXCOORD1;
					float3 normal : NORMAL;
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
					half2 yUV = worldPos.xz;
					half2 xUV = worldPos.zy;
					half2 zUV = worldPos.xy;

					half4 yDiff = tex2D(tex, yUV * _MainTex_ST.xy + offset);
					half4 xDiff = tex2D(tex, xUV * _MainTex_ST.xy + offset);
					half4 zDiff = tex2D(tex, zUV * _MainTex_ST.xy + offset);

					half3 blendWeights = abs(normal);

					blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z)*1.1;

					return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
				}
				sampler2D _CameraDepthTexture;

				half4 frag(v2f i) : COLOR
				{
					half4 mask = tex2DTriplanar(_MainTex, _Time.xx * _TimeScale.xy, i.worldPos, i.normal);
					half4 tex = tex2DTriplanar(_MainTex, _Time.xx * _TimeScale.zw, i.worldPos, i.normal);
					half alphaMask = tex2DTriplanar(_MainTex, 0.3, i.worldPos, i.normal).b;
					//tex *= mask;
					float4 res = 0;
					//res.r = tex.r * step(tex.r, _BorderScale.x);
					//res.r -= tex.r * step(tex.r, _BorderScale.x - _BorderScale.y);
					//res.g = tex.r * step(tex.g, _BorderScale.x);
					//res.g -= step(tex.r, _BorderScale.x - _BorderScale.y);
					//res.r *= tex.g;
					res.r = tex.r * mask.r;
					res = i.color * _TintColor * lerp(0, res.r, alphaMask);
					res.rgb *= res.rgb;
					res.a = saturate(res.a * 4);
					return  0.5 + saturate(res)*0.5;
				}
				ENDCG
			}
	}


}