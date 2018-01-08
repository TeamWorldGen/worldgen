Shader "Custom/leaf_wind_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DirMap ("dir map", 2D) = "white" {}
		_StrMap ("str map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
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
		float _Wind_str, _Wind_dir;

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
			
			float4 wp = mul (unity_ObjectToWorld, v.vertex);
			float Wind_dir = tex2Dlod( _DirMap , float4(fmod(wp.x/1000 , 1), fmod(wp.z/1000 , 1), 0, 0)).r;
			float Wind_str = tex2Dlod( _StrMap , float4(fmod(wp.x/1000 , 1), fmod(wp.z/1000 , 1), 0, 0)).g;

			half wind = Wind_str * (sin(wp.x + _Time.z * 5 * Wind_str)*sin(wp.z + _Time.w * 5 * Wind_str)*sin(wp.y + _Time.x * 5 * Wind_str)) / 5;
			
			wp.x -= wind * sin(2*3.14* Wind_dir);
			wp.z -= wind * cos(2*3.14*Wind_dir);

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
