Shader "Custom/Lighting/BlinnPhongLighting"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_SpecColor("SpecColor", Color) = (1,1,1,1)
		_Spec("Specular", Range(0,1)) = 0.5	// specular power in [0 - 1] range, less value spreads the specular highlight more.
		_Gloss("Gloss", Range(0,1)) = 0.5	// the intensity of the specular light.
	}
		SubShader
	{
		Tags { "Queue" = "Geometry" }

		CGPROGRAM
		#pragma surface surf BlinnPhong

		float4 _Color;
		half _Spec;
		fixed _Gloss;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = _Color.rgb;
			o.Specular = _Spec;
			o.Gloss = _Gloss;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
