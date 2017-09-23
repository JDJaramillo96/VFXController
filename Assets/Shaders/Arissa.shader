Shader "Custom/Surface/Arissa"
{	
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Normal]
		_Normal ("Normal", 2D) = "bump" {}
		_NormalStrength ("Normal Strength", Range (0,2)) = 1.0
		_MainTexFactor ("Albedo Factor", Range(0,1)) = 1.0
		_AlphaFactor ("Alpha Factor", Range(0,1)) = 0.0
		_BorderFactor ("Border Factor", Range(0.1,1)) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ComplementColor ("Complement Color", Color) = (1,1,1,1) //White
		_Color ("Color", Color) = (1,1,1,1) //White
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		LOD 200

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
		sampler2D _Normal;
		half _NormalStrength;
		half _MainTexFactor;
		half _AlphaFactor;
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
			float4 normalInfo = tex2D (_Normal, IN.uv_MainTex);
			//Normal setup
			float3 normal = UnpackNormal (normalInfo).rgb;
			normal.rg *= _NormalStrength;
			//Color setup
			float4 color1 = texInfo * _MainTexFactor;
			float4 color2 = _ComplementColor * (1 - _MainTexFactor);
			//Output
			output.Albedo = (color1 + color2) * _Color;
			output.Metallic = _Metallic;
			output.Smoothness = _Glossiness;
			output.Normal = normalize(normal);

			//Normal setup
			float3 perPixelNormalMap = WorldNormalVector(IN, output.Normal);
			//Dot product between face normal and view direction
			float dotProduct = abs(dot(perPixelNormalMap, IN.viewDir));
			float customBorder = dotProduct / _BorderFactor;
			//Alpha setup
			float alpha1 = customBorder * _AlphaFactor;
			float alpha2 = 1 - _AlphaFactor;
			//Output
			output.Alpha = alpha1 + alpha2;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
