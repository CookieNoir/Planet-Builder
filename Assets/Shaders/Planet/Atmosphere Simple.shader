Shader "Unlit/Planet/Atmosphere Simple"
{
	Properties
	{
		_Color("Night Color", color) = (0.0, 0.0, 0.0, 1)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		CGINCLUDE
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = v.uv;
				return o;
			}
		ENDCG

		Pass
		{
			Stencil {
				Ref 0
				Comp Equal
			}

			CGPROGRAM
				fixed4 frag(v2f i) : COLOR
				{
					return i.color;
				}
			ENDCG
		}

		Pass
		{
			Stencil {
				Ref 1
				Comp Equal
			}

			CGPROGRAM
				static const fixed4 clear = fixed4(0.0, 0.0, 0.0, 0.0);

				fixed4 _Color;

				fixed4 frag(v2f i) : COLOR
				{
					return lerp(_Color, clear, i.uv.y * i.uv.y * (3.0 - 2.0 * i.uv.y));
				}
			ENDCG
		}
	}
}
