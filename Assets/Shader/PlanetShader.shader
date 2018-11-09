// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/PlanetShader" {
	Properties {
		_Color ("Color", Color) = (0,0,1,1)
		_ColorFast ("ColorFast", Color) = (1,0,0,1)
		_Emission ("Emission", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows 
		#pragma multi_compile_instancing
		#pragma vertex vert
		#pragma target 4.0

		#define PI   3.141592

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color: COLOR;
		};

		fixed4 _Color;
		fixed4 _ColorFast;
		fixed4 _Emission;
		float4 cold;

		float maxSpeed = 1;
		float maxMass = 1;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		UNITY_INSTANCING_BUFFER_START(Props)
			  UNITY_DEFINE_INSTANCED_PROP(float3, _Speed)
		#define _Speed_arr Props
			  UNITY_DEFINE_INSTANCED_PROP(float, _Mass)
		#define _Mass_arr Props
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c1 =  lerp(_Color,_ColorFast , length( UNITY_ACCESS_INSTANCED_PROP(_Speed_arr, _Speed))  /  maxSpeed);
			fixed4 c =  c1;
			c = lerp (c,float4(1,1,1,1), UNITY_ACCESS_INSTANCED_PROP(_Mass_arr, _Mass) / (maxMass));
			o.Albedo = c.rgb;
			o.Emission = c.rgb;
		}

        void vert (inout appdata_full v) {
		// Calc radius 
		half rad = pow(UNITY_ACCESS_INSTANCED_PROP(_Mass_arr, _Mass) * 3.0 / (4.0 *  PI), 0.333333) * 0.125;
		 v.vertex.xyz = v.normal * rad ;
        }
		ENDCG
	}
}