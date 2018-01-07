﻿Shader "Custom/ripple_shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DirMap ("dir map", 2D) = "white" {}
		_StrMap ("str map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Wind_str("str", range(0,1)) = 0.5
		_Wind_dir("dir", range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DirMap;
		sampler2D _StrMap;
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
			
			float3 wp = mul (unity_ObjectToWorld, v.vertex).xyz;
			float Wind_dir = tex2Dlod( _DirMap , float4(fmod(wp.x/1000 , 1), fmod(wp.z/1000 , 1), 0, 0)).r;
			float Wind_str = tex2Dlod( _StrMap , float4(fmod(wp.x/1000 , 1), fmod(wp.z/1000 , 1), 0, 0)).g*3;


			float offset = sin(2*3.14*_Wind_dir)*wp.x + cos(2*3.14*_Wind_dir)*wp.z;

			float value = Wind_str * sin(_Time.w*Wind_str*2 + Wind_str*offset)  + sin(wp.x*5 + _Time.w)*cos(wp.z*5*Wind_str + _Time.w)/9*Wind_str;

			v.vertex.x -= sin(2*3.14*Wind_dir)*value/2;
			v.vertex.z -= cos(2*3.14*Wind_dir)*value/2;
			v.vertex.y += value;

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
