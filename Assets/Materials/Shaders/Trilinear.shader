Shader "Tri-Planar World" {
  Properties {
		_MainTex("Texture", 2D) = "white" {}
		_Scale("Scale", Float) = 2
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_BumpScale("Bumpmap Scale", Float) = 0.2
		_Color ("Color", Color) = (1,1,1,1)

		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0

	}
	
	SubShader {
		Tags {
			"RenderType"="Opaque"
		}
 
		Cull Back
		ZWrite On
		
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_builtin

		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma exclude_renderers flash
 
		sampler2D _MainTex, _BumpMap;
		float _Scale, _BumpScale, _RimPower;
		fixed4 _Color, _RimColor;
		float4x4 _Transform;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
			float3 normalW;
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};
			
		void vert (inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.normalW = mul((float3x3)_Object2World, v.normal);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 localNormal = mul(IN.normalW, (float3x3) _Transform);
			float3 projNormal = saturate(abs(localNormal));

			float3 localPos = mul(IN.worldPos - float3(_Transform[0][3], _Transform[1][3], _Transform[2][3]), (float3x3) _Transform);

			// X sides
			float3 main_x = tex2D(_MainTex, frac(localPos.zy * _Scale));
			float3 bump_x = UnpackNormal (tex2D(_BumpMap, frac(localPos.zy * _BumpScale)));
			
			// Y sides
			float3 main_y = tex2D(_MainTex, frac(localPos.zx * _Scale));
			float3 bump_y = UnpackNormal (tex2D(_BumpMap, frac(localPos.zx * _BumpScale)));
			
			// Z sides	
			float3 main_z = tex2D(_MainTex, frac(localPos.xy * _Scale));
			float3 bump_z = UnpackNormal (tex2D(_BumpMap, frac(localPos.xy * _BumpScale)));

			o.Albedo = lerp(main_z, main_x, projNormal.x);
			o.Albedo = lerp(o.Albedo, main_y, projNormal.y);
			o.Albedo = o.Albedo * _Color;

			o.Normal = lerp(bump_z, bump_x, projNormal.x);
			o.Normal = lerp(o.Normal, bump_y, projNormal.y);

		}  
		ENDCG
	}
	Fallback "Diffuse"
}