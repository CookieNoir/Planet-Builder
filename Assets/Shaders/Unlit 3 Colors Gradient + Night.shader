Shader "Unlit/3 Colors Gradient + Night"
{
	Properties
	{
		_Color("Color 1 Day", color) = (1,1,1,1)
		_Color2("Color 2 Day", color) = (1,1,1,1)
		_Color3("Color 3 Day", color) = (1,1,1,1)
		_Color4("Color Night", color) = (1,1,1,1)

		_Border("Border", Range(0.0, 1.0)) = 0.3
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			static const fixed4 clear = fixed4(0.0, 0.0, 0.0, 0.0);

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
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
				fixed4 _Color;
				fixed4 _Color2;
				fixed4 _Color3;
				fixed _Border;

				fixed4 frag(v2f i) : SV_Target
				{
					half4 color = clear;
					color += lerp(clear, lerp(_Color, _Color2, i.uv.y / _Border), step(i.uv.y, _Border));
					color += lerp(clear, lerp(_Color2, _Color3, (i.uv.y - _Border) / (1.0 - _Border)), step(_Border, i.uv.y));
					return color;
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
				fixed4 _Color4;

				fixed4 frag(v2f i) : SV_Target
				{
					return lerp(_Color4, clear, sqrt(i.uv.y));
				}
			ENDCG
		}
	}
}
