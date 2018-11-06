// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/WaterDropsSimple" {
Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (1,1,1,0.5)
        _BumpMap ("Normalmap", 2D) = "bump" {}
		_Speed ("Distort Direction Speed (XY)", Vector) = (1,0,0,0)
        _FPOW("FPOW Fresnel", Float) = 5.0
        _R0("R0 Fresnel", Float) = 0.05
		_BumpAmt ("Distortion Scale", Float) = 10
}
Category {
	
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
				Blend SrcAlpha One
				ZWrite Off
				Cull Back

	SubShader {
GrabPass {							
			//Name "_GrabTex"
			//Tags { "LightMode" = "Always" }
 		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _BumpMap;
			sampler2D _GrabTexture;

			float4 _GrabTexture_TexelSize;
			float4 _Color;
			float4 _RimColor;
			float4 _BumpMap_ST;
			float4 _LightColor0;
			float4 _Speed;

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
				half4 grab : TEXCOORD0;
				half3 normal: TEXCOORD1;
				half3 viewDir : TEXCOORD2;
				fixed4 color : COLOR;
			};

			v2f vert (appdata_full v)
			{
				v2f o;

				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.grab.zw = o.vertex.w;
#if UNITY_SINGLE_PASS_STEREO
				o.grab.xy = TransformStereoScreenSpaceTex(o.grab.xy, o.grab.w);
#endif

				o.color = v.color;
				o.normal = v.normal;

				o.viewDir  = normalize(ObjSpaceViewDir(v.vertex));
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				half fresnelRim = saturate(1 - dot(i.normal, i.viewDir));
				fresnelRim = pow(fresnelRim, _FPOW);
				fresnelRim = saturate( _R0 + (1.0 - _R0) * fresnelRim);
				fresnelRim=fresnelRim*fresnelRim;

				fixed3 emission = fresnelRim *_RimColor;
				float3 light = _LightColor0.rgb * _LightColor0.w;
				emission.rgb *= saturate(light*2);
				return fixed4 (emission * i.color.rgb, _Color.a * i.color.a);
			}
			ENDCG 
		}
		
		Pass {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off
				Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _BumpMap;
			sampler2D _GrabTexture;

			float4 _GrabTexture_TexelSize;
			float4 _Color;
			float4 _RimColor;
			float4 _BumpMap_ST;
			float4 _LightColor0;
			float4 _Speed;

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
				fixed4 color : COLOR;
			};

			v2f vert (appdata_full v)
			{
				v2f o;

				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.grab.zw = o.vertex.w;
#if UNITY_SINGLE_PASS_STEREO
				o.grab.xy = TransformStereoScreenSpaceTex(o.grab.xy, o.grab.w);
#endif

				o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap) +_Time.xx * _Speed.xy;
				o.color = v.color;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));
				
				half2 offset = normal.rg * _BumpAmt * _GrabTexture_TexelSize.xy * i.color.a;
				i.grab.xy = offset * i.grab.z + i.grab.xy;
				half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grab));

				fixed3 emission = col.xyz * _Color.xyz;
				return fixed4 (emission * i.color.rgb, _Color.a * i.color.a);
			}
			ENDCG 
		}
	}	
}
}