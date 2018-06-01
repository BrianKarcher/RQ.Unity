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
			float4 _SinTime;
			float4 screenPos;
		};
		sampler2D _MainTex;
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color * IN._SinTime.y;
			o.Albedo = mainColor.rgb;
			o.Alpha = mainColor.a;
		}
		ENDCG		
	}

	Fallback "tk2d/BlendVertexColor", 1
}
