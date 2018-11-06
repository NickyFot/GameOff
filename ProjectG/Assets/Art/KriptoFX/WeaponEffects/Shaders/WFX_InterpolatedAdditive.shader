// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/WeaponFX/Interpolation/Additive" {
Properties {
	[HDR] _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Tiling("PS Tiling (XY), Tex Tiling (XY)", Vector) = (400, 400, 4, 4)
	_InvFade ("Soft Particles Factor", Float) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#pragma multi_compile_fog
			#pragma target 3.0

			#include "UnityCG.cginc"
			//#include "WFX_PSInterpolation.cginc"

			sampler2D _MainTex;
			float4 _TintColor;
			float4 _Tiling;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			

			inline float4 Tex2DInterpolated(sampler2D Tex, float2 TexCoord, float4 _Tiling)
			{
				float2 grid = (TexCoord * _Tiling.xy - float2(0, _Tiling.y / _Tiling.w));
				float2 gridFloor = floor(grid);

				float frameWithLerp = (((gridFloor.x + (_Tiling.x * (_Tiling.y - gridFloor.y)) / (_Tiling.x * _Tiling.y)) * (_Tiling.z * _Tiling.w)));
				float frame = floor(frameWithLerp);
				float lerpVal = ceil(frameWithLerp);

				float2 prefOffset;
				float texCell = floor(_Tiling.z);
				prefOffset.x = ((float((float(frame) % float(texCell)))) / _Tiling.z);
				prefOffset.y = ((_Tiling.w - floor(float(frame / texCell))) / _Tiling.w);

				float2 nextOffset;
				nextOffset.x = ((float((float(lerpVal) % float(texCell)))) / _Tiling.z);
				nextOffset.y = ((_Tiling.w - floor(float(lerpVal / texCell))) / _Tiling.w);
				float2 tiling = ((grid - gridFloor) / _Tiling.zw);

				float d = 1;
				float2 edge = 2.0 / _Tiling.xy;
				d *= step(edge.x, tiling.x);
				d *= step(tiling.x, 1.0 / _Tiling.z - edge.x);
				d *= step(edge.y, tiling.y);
				d *= step(tiling.y, 1.0 / _Tiling.w - edge.y);
				float4 tex1 = tex2D(Tex, tiling + prefOffset);
				float4 tex2 = tex2D(Tex, tiling + nextOffset);
				return lerp(tex1, tex2, frameWithLerp - frame) * d;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				fixed4 col = 2.0f * i.color * _TintColor * Tex2DInterpolated(_MainTex, i.texcoord, _Tiling);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); 
				return col;
			}
			ENDCG 
		}
	}	
}
}
