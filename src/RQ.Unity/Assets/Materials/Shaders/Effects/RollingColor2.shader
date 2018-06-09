Shader "Custom/RollingColor2" 
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
			
			float timeInc = _Time.y / 7;
			
			float screenAdjustment = (1 + sin((IN.screenPos.x + IN.screenPos.y + timeInc) * 50)) / 2;
			//float screenAdjustmentNoTime = (1 + sin((IN.screenPos.x + IN.screenPos.y) * 50)) / 2;
			
			//screenAdjustment = dot(float3(0,1,0), float3((1 + sin((IN.screenPos.x + timeInc) * 350)) / 2, screenAdjustment, 0));
			
			float posAdjustment = screenAdjustment;
			
			fixed4 waveColor;
			if (screenAdjustment > .5)
				waveColor = mainColor + fixed4(posAdjustment * .25,posAdjustment * .25, posAdjustment, 0);
			else
				waveColor = mainColor;
						
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
