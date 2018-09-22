Shader "Custom/FarAlphaSurf" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalGlobal ("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MinDistance ("Min Distance", Range(0, 500)) = 50
		_MaxDistance ("Max Distance", Range(0, 500)) = 150
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows keepalpha alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_Control : TEXCOORD0;
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _MinDistance;
		half _MaxDistance;
		sampler2D _NormalGlobal;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//float dist = clamp(distance(_WorldSpaceCameraPos, IN.worldPos), _MinDistance, _MaxDistance);
			//float dist01Inv = 1 - ((dist - _MinDistance) / _MaxDistance);
			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			float dist01Inv = 1;
			if(dist >= _MaxDistance * 4)
			{
				dist01Inv = 0.01;
			}
			else if(dist >= _MaxDistance)
			{
				dist01Inv = (dist - _MaxDistance) / (_MaxDistance * 5 - _MaxDistance) * 0.1;
			}
			else if(dist <= _MinDistance)
			{
				dist01Inv = 1;
			}
			else
			{
				dist01Inv = (_MaxDistance - dist) / (_MaxDistance - _MinDistance);
			}

			fixed4 global = ( tex2D (_NormalGlobal, IN.uv_MainTex) );
			half3 normalClose = UnpackNormal(global);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Normal = normalClose;
			o.Alpha = (c.a) * dist01Inv;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
