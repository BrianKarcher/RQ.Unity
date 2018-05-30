// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LitPremul2DToolkitAnimatedTexture"
{
	// BASED ON LitPremulVertexColor
	// Needs pre-multiplied Alpha value

	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		// Create the inspector values
		_FrameWidth("Frame Width", Float) = 0.0
			_TotalFrames("Total Frames", Float) = 0.0
			_FPS("FPS", Float) = 0.0
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off Blend One OneMinusSrcAlpha Cull Off Fog{ Mode Off }
		LOD 110


		// ------------------------------------------------------------
		// Surface shader code generated out of a CGPROGRAM block:
		ZWrite Off ColorMask RGB


		// ---- forward rendering base pass:
		Pass{
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma debug
#pragma multi_compile_fog
#pragma multi_compile_fwdbasealpha noshadow
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
			// Surface shader code generated based on:
			// writes to per-pixel normal: no
			// writes to emission: no
			// needs world space reflection vector: no
			// needs world space normal vector: no
			// needs screen space position: no
			// needs world space position: no
			// needs view direction: no
			// needs world space view direction: no
			// needs world space position for lighting: YES
			// needs world space view direction for lighting: no
			// needs world space view direction for lightmaps: no
			// needs vertex color: YES
			// needs VFACE: no
			// passes tangent-to-world matrix to pixel shader: no
			// reads from normal: no
			// 1 texcoords actually used
			//   float2 _MainTex
#define UNITY_PASS_FORWARDBASE
#define _ALPHAPREMULTIPLY_ON 1
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

			// Original surface shader snippet:
#line 12 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

			//#pragma surface surf Lambert alpha
			//#pragma debug
			struct Input
			{
				float2 uv_MainTex;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
				o.Albedo = mainColor.rgb;
				o.Alpha = mainColor.a;
			}


			// vertex-to-fragment interpolation data
			// no lightmaps:
#ifdef LIGHTMAP_OFF
			struct v2f_surf {
				float4 pos : SV_POSITION;
				float2 pack0 : TEXCOORD0; // _MainTex
				half3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				fixed4 color : COLOR0;
#if UNITY_SHOULD_SAMPLE_SH
				half3 sh : TEXCOORD3; // SH
#endif
				UNITY_FOG_COORDS(4)
#if SHADER_TARGET >= 30
					float4 lmap : TEXCOORD5;
#endif
			};
#endif
			// with lightmaps:
#ifndef LIGHTMAP_OFF
			struct v2f_surf {
				float4 pos : SV_POSITION;
				float2 pack0 : TEXCOORD0; // _MainTex
				half3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				fixed4 color : COLOR0;
				float4 lmap : TEXCOORD3;
				UNITY_FOG_COORDS(4)
#ifdef DIRLIGHTMAP_COMBINED
					fixed3 tSpace0 : TEXCOORD5;
				fixed3 tSpace1 : TEXCOORD6;
				fixed3 tSpace2 : TEXCOORD7;
#endif
			};
#endif
			float4 _MainTex_ST;
			//Create the connection to the properties inside of the   
			//CG program 
			float _FrameWidth;
			float _TotalFrames;
			float _FPS;
			float _Tick;

			// vertex shader
			v2f_surf vert_surf(appdata_full v) {
				v2f_surf o;
				UNITY_INITIALIZE_OUTPUT(v2f_surf, o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
#if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
#endif
#if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
#endif
				o.worldPos = worldPos;
				o.worldNormal = worldNormal;
				o.color = v.color;
#ifndef DYNAMICLIGHTMAP_OFF
				o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif
#ifndef LIGHTMAP_OFF
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

				// SH/ambient and vertex lights
#ifdef LIGHTMAP_OFF
#if UNITY_SHOULD_SAMPLE_SH
#if UNITY_SAMPLE_FULL_SH_PER_PIXEL
				o.sh = 0;
#elif (SHADER_TARGET < 30)
				o.sh = ShadeSH9(float4(worldNormal, 1.0));
#else
				o.sh = ShadeSH3Order(half4(worldNormal, 1.0));
#endif
				// Add approximated illumination from non-important point lights
#ifdef VERTEXLIGHT_ON
				o.sh += Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, worldPos, worldNormal);
#endif
#endif
#endif // LIGHTMAP_OFF

				UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
				return o;
			}

			// fragment shader
			fixed4 frag_surf(v2f_surf IN) : SV_Target{
				// prepare and unpack data
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT(Input, surfIN);




				//ANIMATION  
				float2 spriteUV = IN.pack0;
					_Tick = frac(_Time.y * _FPS);
				spriteUV.x = spriteUV.x + (floor(_Tick * _TotalFrames) * _FrameWidth);

				//fixed4 col = tex2D(_MainTex, spriteUV);




				surfIN.uv_MainTex = spriteUV.xy;// IN.pack0.xy;
				float3 worldPos = IN.worldPos;
#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
					fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
				surfIN.color = IN.color;
#ifdef UNITY_COMPILER_HLSL
				SurfaceOutput o = (SurfaceOutput)0;
#else
				SurfaceOutput o;
#endif
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				fixed3 normalWorldVertex = fixed3(0, 0, 1);
				o.Normal = IN.worldNormal;
				normalWorldVertex = IN.worldNormal;

				// call surface function
				surf(surfIN, o);

				// compute lighting & shadowing factor
				UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
					fixed4 c = 0;

				// Setup lighting environment
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
#if !defined(LIGHTMAP_ON)
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = lightDir;
				gi.light.ndotl = LambertTerm(o.Normal, gi.light.dir);
#endif
				// Call GI (lightmaps/SH/reflections) lighting function
				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldPos = worldPos;
				giInput.atten = atten;
#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
				giInput.lightmapUV = IN.lmap;
#else
				giInput.lightmapUV = 0.0;
#endif
#if UNITY_SHOULD_SAMPLE_SH
				giInput.ambient = IN.sh;
#else
				giInput.ambient.rgb = 0.0;
#endif
				giInput.probeHDR[0] = unity_SpecCube0_HDR;
				giInput.probeHDR[1] = unity_SpecCube1_HDR;
#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
				giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
				giInput.boxMax[0] = unity_SpecCube0_BoxMax;
				giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
				giInput.boxMax[1] = unity_SpecCube1_BoxMax;
				giInput.boxMin[1] = unity_SpecCube1_BoxMin;
				giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif
				LightingLambert_GI(o, giInput, gi);

				// realtime lighting: call lighting function
				c += LightingLambert(o, gi);
				UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
				return c;
			}

				ENDCG

		}

		// ---- forward rendering additive lights pass:
		Pass{
				Name "FORWARD"
				Tags{ "LightMode" = "ForwardAdd" }
				ZWrite Off Blend One One
				Blend One One

				CGPROGRAM
				// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma debug
#pragma multi_compile_fog
#pragma multi_compile_fwdadd noshadow
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
				// Surface shader code generated based on:
				// writes to per-pixel normal: no
				// writes to emission: no
				// needs world space reflection vector: no
				// needs world space normal vector: no
				// needs screen space position: no
				// needs world space position: no
				// needs view direction: no
				// needs world space view direction: no
				// needs world space position for lighting: YES
				// needs world space view direction for lighting: no
				// needs world space view direction for lightmaps: no
				// needs vertex color: YES
				// needs VFACE: no
				// passes tangent-to-world matrix to pixel shader: no
				// reads from normal: no
				// 1 texcoords actually used
				//   float2 _MainTex
#define UNITY_PASS_FORWARDADD
#define _ALPHAPREMULTIPLY_ON 1
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

				// Original surface shader snippet:
#line 12 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

				//#pragma surface surf Lambert alpha
				//#pragma debug
				struct Input
				{
					float2 uv_MainTex;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				void surf(Input IN, inout SurfaceOutput o)
				{
					fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
					o.Albedo = mainColor.rgb;
					o.Alpha = mainColor.a;
				}


				// vertex-to-fragment interpolation data
				struct v2f_surf {
					float4 pos : SV_POSITION;
					float2 pack0 : TEXCOORD0; // _MainTex
					half3 worldNormal : TEXCOORD1;
					float3 worldPos : TEXCOORD2;
					fixed4 color : COLOR0;
					UNITY_FOG_COORDS(3)
				};
				float4 _MainTex_ST;

				// vertex shader
				v2f_surf vert_surf(appdata_full v) {
					v2f_surf o;
					UNITY_INITIALIZE_OUTPUT(v2f_surf, o);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
						fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = worldPos;
					o.worldNormal = worldNormal;
					o.color = v.color;

					UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
					return o;
				}

				// fragment shader
				fixed4 frag_surf(v2f_surf IN) : SV_Target{
					// prepare and unpack data
					Input surfIN;
					UNITY_INITIALIZE_OUTPUT(Input, surfIN);
					surfIN.uv_MainTex = IN.pack0.xy;
					float3 worldPos = IN.worldPos;
#ifndef USING_DIRECTIONAL_LIGHT
						fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
						fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
					surfIN.color = IN.color;
#ifdef UNITY_COMPILER_HLSL
					SurfaceOutput o = (SurfaceOutput)0;
#else
					SurfaceOutput o;
#endif
					o.Albedo = 0.0;
					o.Emission = 0.0;
					o.Specular = 0.0;
					o.Alpha = 0.0;
					o.Gloss = 0.0;
					fixed3 normalWorldVertex = fixed3(0, 0, 1);
					o.Normal = IN.worldNormal;
					normalWorldVertex = IN.worldNormal;

					// call surface function
					surf(surfIN, o);
					UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
						fixed4 c = 0;

					// Setup lighting environment
					UnityGI gi;
					UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
					gi.indirect.diffuse = 0;
					gi.indirect.specular = 0;
#if !defined(LIGHTMAP_ON)
					gi.light.color = _LightColor0.rgb;
					gi.light.dir = lightDir;
					gi.light.ndotl = LambertTerm(o.Normal, gi.light.dir);
#endif
					gi.light.color *= atten;
					c += LightingLambert(o, gi);
					UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
					return c;
				}

					ENDCG

			}

			// ---- meta information extraction pass:
			Pass{
					Name "Meta"
					Tags{ "LightMode" = "Meta" }
					Cull Off

					CGPROGRAM
					// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma debug

#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
					// Surface shader code generated based on:
					// writes to per-pixel normal: no
					// writes to emission: no
					// needs world space reflection vector: no
					// needs world space normal vector: no
					// needs screen space position: no
					// needs world space position: no
					// needs view direction: no
					// needs world space view direction: no
					// needs world space position for lighting: YES
					// needs world space view direction for lighting: no
					// needs world space view direction for lightmaps: no
					// needs vertex color: YES
					// needs VFACE: no
					// passes tangent-to-world matrix to pixel shader: no
					// reads from normal: no
					// 1 texcoords actually used
					//   float2 _MainTex
#define UNITY_PASS_META
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

					// Original surface shader snippet:
#line 12 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

					//#pragma surface surf Lambert alpha
					//#pragma debug
					struct Input
					{
						float2 uv_MainTex;
						fixed4 color : COLOR;
					};

					sampler2D _MainTex;
					void surf(Input IN, inout SurfaceOutput o)
					{
						fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
						o.Albedo = mainColor.rgb;
						o.Alpha = mainColor.a;
					}

#include "UnityMetaPass.cginc"

					// vertex-to-fragment interpolation data
					struct v2f_surf {
						float4 pos : SV_POSITION;
						float2 pack0 : TEXCOORD0; // _MainTex
						float3 worldPos : TEXCOORD1;
						fixed4 color : COLOR0;
					};
					float4 _MainTex_ST;

					// vertex shader
					v2f_surf vert_surf(appdata_full v) {
						v2f_surf o;
						UNITY_INITIALIZE_OUTPUT(v2f_surf, o);
						o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
						o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
						float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
							fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
						o.worldPos = worldPos;
						o.color = v.color;
						return o;
					}

					// fragment shader
					fixed4 frag_surf(v2f_surf IN) : SV_Target{
						// prepare and unpack data
						Input surfIN;
						UNITY_INITIALIZE_OUTPUT(Input, surfIN);
						surfIN.uv_MainTex = IN.pack0.xy;
						float3 worldPos = IN.worldPos;
#ifndef USING_DIRECTIONAL_LIGHT
							fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
							fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
						surfIN.color = IN.color;
#ifdef UNITY_COMPILER_HLSL
						SurfaceOutput o = (SurfaceOutput)0;
#else
						SurfaceOutput o;
#endif
						o.Albedo = 0.0;
						o.Emission = 0.0;
						o.Specular = 0.0;
						o.Alpha = 0.0;
						o.Gloss = 0.0;
						fixed3 normalWorldVertex = fixed3(0, 0, 1);

						// call surface function
						surf(surfIN, o);
						UnityMetaInput metaIN;
						UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
						metaIN.Albedo = o.Albedo;
						metaIN.Emission = o.Emission;
						return UnityMetaFragment(metaIN);
					}

						ENDCG

				}

				// ---- end of surface shader generated code

				#LINE 30

	}


	Fallback "tk2d/PremulVertexColor", 1
}
