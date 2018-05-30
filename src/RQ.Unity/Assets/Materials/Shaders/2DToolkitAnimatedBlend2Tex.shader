// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex color, 2 textures, alpha blended
// cull off

Shader "Custom/Lit2DToolkitAnimatedTexture" 
{
	// Based on Blend2TexVertexColor shader
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_GradientTex ("Gradient (RGBA)", 2D) = "white" {}
		// Create the inspector values
		_FrameWidth("Frame Width", Float) = 0.0
			_TotalFrames("Total Frames", Float) = 0.0
			_FPS("FPS", Float) = 0.0
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vctt
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _GradientTex;
			//Create the connection to the properties inside of the   
			//CG program 
			float _FrameWidth;
			float _TotalFrames;
			float _FPS;

			float _Tick;

			struct vin_vctt
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f_vctt
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float4 texcoord01 : TEXCOORD0;
			};

			v2f_vctt vert_vctt(vin_vctt v)
			{
				v2f_vctt o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord01.xy = v.texcoord;
				o.texcoord01.zw = v.texcoord1;
				return o;
			}

			fixed4 frag(v2f_vctt i) : SV_Target
			{
				//ANIMATION  
				float4 spriteUV = i.texcoord01;
				_Tick = frac(_Time.y * _FPS);
				spriteUV.x = spriteUV.x + (floor(_Tick * _TotalFrames) * _FrameWidth);

				//fixed4 col = tex2D(_MainTex, spriteUV);
				fixed4 col = tex2D(_MainTex, spriteUV.xy) * tex2D(_GradientTex, spriteUV.zw) * i.color;
				return col;
			}
			
			ENDCG
		} 
	}
}
