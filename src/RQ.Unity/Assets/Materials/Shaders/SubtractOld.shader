Shader "Custom/SubtractOld" 
{
	Properties 
	{
	    _MainTex ("Base (RGB)", 2D) = "white" {}
		//_Color ("Tint Color", Color) = (1,1,1,1)
	}
	
	SubShader {
        Tags { "Queue" = "Transparent" }
        Pass {
            BlendOp RevSub   // Subtract source from destination.
			Blend One One
            SetTexture [_MainTex] { combine texture }
        }
    }

	Fallback "tk2d/BlendVertexColor", 1
}
