Shader "Unlit/4 Colors Gradient + Night"
{
	Properties
	{
		_Color("Color 1 Day", color) = (1,1,1,1)
		_Color2("Color 2 Day", color) = (1,1,1,1)
		_Color3("Color 3 Day", color) = (1,1,1,1)
		_Color4("Color 4 Day", color) = (1,1,1,1)

		_Color5("Color Night", color) = (1,1,1,1)

		_Border1("Border 1", Range(0.0, 1.0)) = 0.3
		_Border2("Border 2",  Range(0.0, 1.0)) = 0.7
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
				fixed4 _Color4;
				fixed _Border1;
				fixed _Border2;

				fixed4 frag(v2f i) : SV_Target
				{
					half4 color = clear;
					color += lerp(clear, lerp(_Color, _Color2, i.uv.y / _Border1), step(i.uv.y, _Border1));
					color += lerp(clear, lerp(_Color2, _Color3, (i.uv.y - _Border1) / (_Border2 - _Border1)), step(0.0, (_Border2 - i.uv.y)*sign(i.uv.y - _Border1)));
					color += lerp(clear, lerp(_Color3, _Color4, (i.uv.y - _Border2) / (1.0 - _Border2)), step(_Border2, i.uv.y));
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
				fixed4 _Color5;

				fixed4 frag(v2f i) : SV_Target
				{
					return lerp(_Color5, clear, sqrt(i.uv.y));
				}
			ENDCG
		}
	}
}
