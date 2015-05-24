Shader "Tri-Planar World" {
  Properties {
		_MainTex("Texture", 2D) = "white" {}
		_Scale("Scale", Float) = 2
		_BumpMap("Bumpmap", 2D) = "white" {}
		_BumpScale("Bumpmap Scale", Float) = 2
		_Color ("Color", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"IgnoreProjector"="False"
			"RenderType"="Opaque"
		}
 
		Cull Back
		ZWrite On
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		#pragma exclude_renderers flash
 
		sampler2D _MainTex, _BumpMap;
		float _Scale, _BumpScale;
		fixed4 _Color;
		float4x4 _Transform;
		
		struct Input {
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};
			
		void surf (Input IN, inout SurfaceOutputStandard o) {
				
			//float3 projNormal = saturate(abs(IN.worldNormal));
			//float3 projNormal = saturate(IN.worldNormal);
			//float3 localPos = mul(float4(IN.worldPos.x, IN.worldPos.y, IN.worldPos.z, 0), _Transform);
			float3 projNormal = saturate(abs(mul(IN.worldNormal, (float3x3) _Transform)));

			float3 localPos = mul(IN.worldPos - float3(_Transform[0][3], _Transform[1][3], _Transform[2][3]), (float3x3) _Transform);
			//float3 localPos = mul(IN.worldPos, (float3x3) _Transform);
			//float3 projNormal = saturate(abs(mul(IN.worldNormal, (float3x3) _Transform)));

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
			//o.Albedo =  _Color;

			
			//if(projNormal.x >=0.9f){
			//	o.Albedo = _Color;
			//}

			if(projNormal.x >=0.9f){
				//o.Normal = bump_x;
			}
			//o.Normal = lerp(bump_z, bump_x, projNormal.x);
			//o.Normal = lerp(o.Normal, bump_y, projNormal.y);
		}  
		ENDCG
	}
	Fallback "Diffuse"
}