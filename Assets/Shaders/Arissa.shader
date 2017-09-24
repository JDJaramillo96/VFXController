Shader "Custom/Arissa"
{	
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_MainTexFactor("Albedo Factor", Range(0,1)) = 1.0
		
		[Space()][Space()]

		[Normal]
		_NormalMap("Normal map", 2D) = "bump" {}
		_NormalStrength("Strength", Range (0,2)) = 1.0
		
		[Space()][Space()]

		_OcclusionMap("Occlusion map", 2D) = "white" {}
		_OcclusionStrength("Strength", Range(0,1)) = 1.0

		[Space()][Space()]

		_Border("Border", Range(0,1)) = 1.0
		_BorderFactor("Border Factor", Range(0,1)) = 0.0

		[Space()][Space()]

		_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0

		[Space()][Space()]

		_ComplementColor("Complement Color", Color) = (0,0,0,1) //Black
		_Color("Color", Color) = (1,1,1,1) //White
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		//Extra pass
		Pass
		{
			ZWrite On
			ColorMask 0
		}

		Cull Back

		LOD 200
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha:blend
		#pragma target 3.0

		// --- PROPERTIES!

		sampler2D _MainTex;
		half _MainTexFactor;
		sampler2D _NormalMap;
		half _NormalStrength;
		sampler2D _OcclusionMap;
		half _OcclusionStrength;
		half _Border;
		half _BorderFactor;
		half _Glossiness;
		half _Metallic;
		float4 _ComplementColor;
		float4 _Color;

		// --- STRUCTS!

		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal; INTERNAL_DATA
		};

		// --- SUBSHADER FUNCTIONS!

		void surf (Input IN, inout SurfaceOutputStandard output)
		{
			//Textures info
			float4 texInfo = tex2D (_MainTex, IN.uv_MainTex);
			float4 normalInfo = tex2D (_NormalMap, IN.uv_MainTex);
			float4 occlusionInfo = tex2D(_OcclusionMap, IN.uv_MainTex);
			//Normal setup
			float3 normal = UnpackNormal (normalInfo).rgb;
			normal.rg *= _NormalStrength;
			//Color setup
			float3 color1 = texInfo.rgb * _MainTexFactor;
			float3 color2 = _ComplementColor.rgb * (1 - _MainTexFactor);
			//Output
			output.Albedo = (color1 + color2) * _Color.rgb;
			output.Occlusion = occlusionInfo.r * _OcclusionStrength;
			output.Metallic = _Metallic;
			output.Smoothness = _Glossiness;
			output.Normal = normalize(normal);			

			//Normal vector based on per pixel normal map
			float3 perPixelNormalMap = WorldNormalVector(IN, output.Normal);
			//Dot product between face normal and view direction
			half dotProduct = abs(dot(perPixelNormalMap, IN.viewDir));
			half customBorder = dotProduct / _Border;
			customBorder = clamp(customBorder, 0, 1);
			//Alpha setup
			half alpha1 = customBorder * _BorderFactor;
			half alpha2 = 1 - _BorderFactor;
			//Output alpha
			output.Alpha = (alpha1 + alpha2) * _Color.a;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
