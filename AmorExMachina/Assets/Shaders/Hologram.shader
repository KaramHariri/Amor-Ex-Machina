Shader "Custom/Hologram"
{
    Properties
    {
        _RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim power", Range(0.1 , 5)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

		// First Pass : Write depth data of the model into the Z buffer but not gonna write any color data into the buffer.
		Pass
		{
			ZWrite On
			ColorMask 0
			//ColorMask RGB  // visualize what's in the Z buffer in case of some issues to see if we have data in the buffer.
		}

		// Second Pass.
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        float4 _RimColor;
		half _RimPower;

        struct Input
        {
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			o.Alpha = pow(rim, _RimPower);

        }
        ENDCG
    }
    FallBack "Diffuse"
}
