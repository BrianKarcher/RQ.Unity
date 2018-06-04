Shader "Custom/RollingColor" 
{
	Properties 
	{
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off Fog { Mode Off }
		LOD 110

		CGPROGRAM
		#pragma surface surf Lambert alpha
		struct Input {
			float2 uv_MainTex;
			fixed4 color : COLOR;
			//float4 _SinTime;
			float4 screenPos;
		};
		sampler2D _MainTex;
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			
			//float timeInc = (_Time.y - (int)_Time.y);
			float timeInc = _Time.y / 25;
			
			float screenAdjustment = (1 + sin((IN.screenPos.x + timeInc) * 350)) / 2;
			//screenAdjustment += timeInc;
			float posAdjustment = screenAdjustment;
			
			fixed4 waveColor = mainColor;
			waveColor *= posAdjustment;
						
			//fixed4 waveColor = mainColor  * ((1 + _SinTime.z) / 2);
			o.Albedo = waveColor.rgb;
			//o.Albedo = mainColor.rgb;
			//o.Alpha = posAdjustment;			
			o.Alpha = mainColor.a;
			//o.Alpha = timeInc;
			//o.Alpha = 1;
		}
		ENDCG		
	}

	Fallback "tk2d/BlendVertexColor", 1
}
