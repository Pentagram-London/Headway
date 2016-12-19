Shader "Custom/ImageEffect" {
	Properties {
		_MainTex ("BLIT RGB", 2D) = "white" {}
		_GrainTex ("Grain Image", 2D) = "white" {}
		_RandomValue("Random Value", float) = 1.0
		_ColorTint("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_WhiteOut("WhiteOut", Range(0,1)) = 0
	}
	SubShader {
		pass
		{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _GrainTex;
			uniform fixed _RandomValue;
			uniform fixed4 _ColorTint;
			uniform fixed _WhiteOut;

			float4 Overlay(float4 a, float4 b) {
				float4 r = float4(0, 0, 0, 1);
				if (a.r > 0.5) { r.r = 1 - (1 - 2 * (a.r - 0.5))*(1 - b.r); }
				else { r.r = (2 * a.r)*b.r; }
				if (a.g > 0.5) { r.g = 1 - (1 - 2 * (a.g - 0.5))*(1 - b.g); }
				else { r.g = (2 * a.g)*b.g; }
				if (a.b > 0.5) { r.b = 1 - (1 - 2 * (a.b - 0.5))*(1 - b.b); }
				else { r.b = (2 * a.b)*b.b; }
				return r;
			}

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed4 renderTex = tex2D(_MainTex, i.uv); // get the BLIT
				fixed4 tinted = lerp(renderTex, _ColorTint, 0.1);
				half2 grainuv = i.uv.xy + half2(_RandomValue, _RandomValue);
				//fixed4 grainTint = lerp(renderTex, Overlay(tinted, tex2D(_GrainTex, grainuv)), 1.0);

				return lerp(tinted, fixed4(1.0, 1.0, 1.0, 1.0), _WhiteOut);

			}

			

			ENDCG
			
		}
	}
}
