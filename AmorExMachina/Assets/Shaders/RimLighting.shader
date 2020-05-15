Shader "Custom/Rim_Lighting"
{
	Properties
	{
		_MainTex ("Main_Texture", 2D) = "white" {}
		_RimColor("Rim_Color", Color) = (1,1,1,1)
		_RimPower("Rim_Power", Range(0.5, 5)) = 1
	}
    SubShader
    {
		CGPROGRAM

		#pragma surface surf Lambert

        float4 _RimColor;
		float _RimPower;
		sampler2D _MainTex;

        struct Input
        {
			float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {	
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			// saturate makes the dot between [0 , 1] instead of [-1 , 1].
			half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
			// we multiply the rim by power of RimPower to control how much the gradient is spread over the model.
			//the higher RimPower the less spread from the edges of the model.
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
