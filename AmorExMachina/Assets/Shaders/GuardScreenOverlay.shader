Shader "Custom/Laser"
{
	Properties
	{
		//General properties
		_Color("Color", COLOR) = (0,0,0,0)
		_MainTex("Hex Texture", 2D) = "white" {}
		_PulseIntensity("Hex Pulse Intensity", float) = 3.0		// controlling the intensity of the hexagons. This is use to animate them fading in and out.
		_PulseTimeScale("Hex Pulse Time Scale", float) = 2.0	// controlling the animation speed.
		_PulsePosScale("Hex Pulse Position Scale", float) = 50.0 // this sort of will control how many lines will move through the object to fade out the hexagon.
	}
		SubShader
	{
		Tags {"RenderType" = "Transparent" "Queue" = "Transparent"} //Make sure the object is rendered after the opaque objects
		Cull Off //Disable backface culling
		Blend SrcAlpha One //Somewhat additive blend mode, you should choose whatever mode you think looks best

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert //Our vertex function is called vert ...
			#pragma fragment frag //... our fragment function frag

			#include "UnityCG.cginc" //Provides lots of helper functions

			//Input values we need from the vertices of the mesh
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			//Data we have to pass from the vertex to the fragment function 
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexObjPos : TEXCOORD1; //Needed for pulse animations. This is the local position of the model.
				float4 screenPos : TEXCOORD2; //Needed for sampling the depth texture
				float depth : TEXCOORD3;
			};

			//General variables
			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _PulseIntensity;
			float _PulseTimeScale;
			float _PulsePosScale;

			//Vertex function
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertexObjPos = v.vertex;
				return o;
			}

			//Fragment function
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				// _Time.y represents the unscaled time
				// _PulseIntensity : is the intensity of each hexagon.
				// _PulseIntensity * abs(sin(_Time.y) : this will animate the hexagons as fade in and out
				fixed4 mainTexColor = mainTex * _Color * _PulseIntensity * abs(sin(_Time.y * _PulseTimeScale + i.vertexObjPos.x * _PulsePosScale));
				return fixed4(_Color.rgb + mainTexColor.rgb, _Color.a);
			}
			ENDHLSL
		}
	}
}