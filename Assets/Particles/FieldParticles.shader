Shader "Custom/FadeParticles" {
	Properties {
		_Alpha ("Albedo (RGB)", 2D) = "white" {}
		_MaxDistance("Max Distance", Range(0, 50)) = 5
		_MinDistance("Max Distance", Range(0, 2)) = 2
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _Alpha;
		fixed _MaxDistance;
		fixed _MinDistance;

		struct Input {
			float2 uv_Alpha;
			float3 worldPos;
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed alphaMap = tex2D (_Alpha, IN.uv_Alpha).r;
			float maxDistance = saturate(1 - pow(length(IN.worldPos - _WorldSpaceCameraPos) / _MaxDistance, 0.5));
			float minDistance = saturate(1 - pow(length(IN.worldPos - _WorldSpaceCameraPos) / _MinDistance, 1));
			o.Albedo = IN.color.rgb;
			o.Alpha = IN.color.a * alphaMap * (maxDistance - minDistance);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
