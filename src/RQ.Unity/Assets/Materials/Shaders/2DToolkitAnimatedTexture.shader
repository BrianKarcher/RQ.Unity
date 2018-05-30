// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/2DToolkit_AnimatedTexture"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Ambient("Ambient Color", Color) = (0.3, 0.3, 0.3, 1)

		// Create the inspector values
		_FrameWidth("Frame Width", Float) = 0.0
			_TotalFrames("Total Frames", Float) = 0.0
			_FPS("FPS", Float) = 0.0
	}

	SubShader
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			//Tags{ "RenderType" = "Opaque" }//otherwise no light related values will be filled

			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				//Tags{ "LightMode" = "Vertex" }//otherwise no light related values will be filled

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					//fixed4 position : POSITION;
					//half2 uv_MainTex:TEXCOORD0;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				fixed4 _Ambient;
				float4 _MainTex_ST;

				//Create the connection to the properties inside of the   
				//CG program 
				float _FrameWidth;
				float _TotalFrames;
				float _FPS;

				float _Tick;

				v2f vert(appdata_base v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					//o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					//o.texcoord = mul(_Object2World, v.vertex).xy * _MainTex_ST.xy + _MainTex_ST.zw;
					//o.texcoord = (mul(_Object2World, v.vertex).xy * _MainTex_ST.xy + _MainTex_ST.zw);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					// per vertex light calc
					//fixed3 lightDirection;
					//fixed attenuation;
					//// add diffuse
					//if (unity_LightPosition[0].w == 0.0)//directional light
					//{
					//	attenuation = 2;
					//	lightDirection = normalize(mul(unity_LightPosition[0], UNITY_MATRIX_IT_MV).xyz);
					//}
					//else// point or spot light
					//{
					//	lightDirection = normalize(mul(unity_LightPosition[0], UNITY_MATRIX_IT_MV).xyz - v.vertex.xyz);
					//	attenuation = 1.0 / (length(mul(unity_LightPosition[0], UNITY_MATRIX_IT_MV).xyz - v.vertex.xyz)) * 0.5;
					//}
					//fixed3 normalDirction = normalize(v.normal);
					//fixed3 diffuseLight = unity_LightColor[0].xyz * max(dot(normalDirction, lightDirection), 0);
					//// combine the lights (diffuse + ambient)
					//o.color.xyz = diffuseLight * attenuation + _Ambient.xyz;
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					//ANIMATION  
					float2 spriteUV = i.texcoord;
					_Tick = frac(_Time.y * _FPS);
					spriteUV.x = spriteUV.x + (floor(_Tick * _TotalFrames) * _FrameWidth);

					fixed4 col = tex2D(_MainTex, spriteUV);
					//fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
					//o.Albedo = mainColor.rgb;
					//o.Alpha = mainColor.a;
					return col; // *i.color;
				}
					ENDCG
			}
		}

//		SubShader
//				{
//					Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
//					ZWrite Off Blend One OneMinusSrcAlpha Cull Off Fog{ Mode Off }
//					LOD 110
//
//					CGPROGRAM
//#pragma surface surf Lambert alpha
//					struct Input
//					{
//						float2 uv_MainTex;
//						fixed4 color : COLOR;
//					};
//
//					sampler2D _MainTex;
//					void surf(Input IN, inout SurfaceOutput o)
//					{
//
//					}
//					ENDCG
//				}
}