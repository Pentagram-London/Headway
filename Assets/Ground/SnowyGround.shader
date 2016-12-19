Shader "Custom/SnowyGround" {
	Properties {
		_DispMap ("Ground Displacement Map", 2D) = "white" {}
		_DispStr("Ground Displacement Amount", Range(-10, 10)) = 0.5
		_DispColorMult("Ground Color Multiplier", Range(-5, 5)) = 4
		_DispColorAdd("Ground Color Offset", Range(-5, 5)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert vertex:vert nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _DispMap;

		struct Input {
			fixed3 worldPos : texcoord;
			fixed3 viewDir;
			fixed2 uv_MainTex;
			fixed4 displacementColor;
		};

		fixed _DispStr;
		fixed _DispColorMult;
		fixed _DispColorAdd;

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 windColor = saturate(tex2D(_DispMap, (IN.worldPos.xz * 0.05) + _Time.x * fixed2(5, 5), 0, 0) * 0.43);
			fixed rim = saturate(dot(normalize(IN.viewDir), o.Normal));

			o.Albedo = IN.displacementColor.rgb + (windColor.r * (rim * 0.5 + 0.5));
			o.Alpha = 1;
		}

		void vert (inout appdata_full v, out Input OUT) {

			UNITY_INITIALIZE_OUTPUT(Input, OUT);

			fixed4 pre_vertexLocalPos = v.vertex;

			fixed4 d = tex2Dlod(_DispMap, fixed4(mul(unity_ObjectToWorld, v.vertex).xz * 0.1, 0, 0));
			v.vertex.xyz -= ((v.normal * d) - (v.normal * 0.5)) * _DispStr;

			fixed4 post_vertexLocalPos = v.vertex;

			fixed heightShade = saturate((1 - (((pre_vertexLocalPos.y - post_vertexLocalPos.y)) / _DispStr) - 0.5)) * _DispColorMult + _DispColorAdd;

			OUT.displacementColor = fixed4(heightShade, heightShade, heightShade, 1);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
