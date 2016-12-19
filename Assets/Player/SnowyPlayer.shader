// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SnowyPlayer" {
	Properties {
		_BaseColor ("Base Albedo", Color) = (1,1,1,1)
		_Snow("Level of snow", Range(1, -1)) = 1
		_SnowColor("Color of snow", Color) = (1.0, 1.0, 1.0, 1.0)
		_SnowDirection("Direction of snow", Vector) = (0, 1, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed _Snow;
	fixed4 _SnowColor;
		fixed4 _SnowDirection;
		fixed _SnowDepth;

		struct Input {
			fixed2 uv_MainTex;
			fixed2 uv_Bump;
			fixed3 worldNormal;
			fixed3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed d = dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz);
			if (d >= _Snow)
			{
				o.Albedo = saturate(lerp(saturate(pow(1 - IN.worldPos.yyy + 0.6, 3.0)), _SnowColor.rgb, saturate(d - _Snow)));
			}
			else
			{

				o.Albedo = saturate(pow(1 - IN.worldPos.yyy + 0.3, 3.0));
				o.Metallic = 0.0f;
				o.Smoothness = 0.37f;
			}

			o.Alpha = 1;
		}

		void vert (inout appdata_full v) {


			fixed d = dot(mul(unity_ObjectToWorld, v.normal), _SnowDirection.xyz);

			if (d >= _Snow)
				v.vertex.xyz += v.normal * 0.05f * (d - _Snow);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
