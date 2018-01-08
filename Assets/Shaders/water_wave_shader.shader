Shader "Custom/water_wave_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DirMap ("dir map", 2D) = "white" {}
		_StrMap ("str map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MapScale("map scale", float) = 10000
	}
	SubShader {
		Tags { "RenderType"="Opaque" "DisableBatching"="True"}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DirMap, _StrMap; //In the temp solution with predefined maps that have rgb values per pixel, having two maps makes no sense. This is a relic from how it SHOULD be.
		float _Wind_str, _Wind_dir, _MapScale;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void vert (inout appdata_full v) {
			
			float3 wp = mul (unity_ObjectToWorld, v.vertex).xyz;
			float Wind_dir = tex2Dlod( _DirMap , float4(fmod(wp.x/_MapScale , 1), fmod(wp.z/_MapScale , 1), 0, 0)).r;
			float Wind_str = tex2Dlod( _StrMap , float4(fmod(wp.x/_MapScale , 1), fmod(wp.z/_MapScale , 1), 0, 0)).g;

			half value = Wind_str * sin(_Time.w*Wind_str*2 + Wind_str*(sin(2*3.14*_Wind_dir)*wp.x + cos(2*3.14*_Wind_dir)*wp.z))  + sin(wp.x*5 + _Time.w)*cos(wp.z*5*Wind_str + _Time.w)/9*Wind_str;

			wp.x -= sin(2*3.14*Wind_dir)*value/2;
			wp.z -= cos(2*3.14*Wind_dir)*value/2;
			wp.y += value;

			v.vertex = mul (unity_WorldToObject, wp);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
