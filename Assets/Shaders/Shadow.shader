Shader "Unlit/Shadow"
{
	Properties
	{
		_Color("Color", Color) = (0.0, 0.0, 0.0, 0.5)
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		CGINCLUDE
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			static const half4 empty = half4(0.0, 0.0, 0.0, 0.0);
		ENDCG

		Pass
		{
			Stencil {
				Ref 3
				Comp equal
			}

			CGPROGRAM
				fixed4 _Color;

				fixed4 frag(v2f i) : COLOR
				{
					return _Color;
				}
			ENDCG
		}
		Pass
		{
			Stencil {
				Ref 3
				Comp Greater
				Pass IncrSat
			}

			CGPROGRAM
				fixed4 frag(v2f i) : COLOR
				{
					return empty;
				}
			ENDCG
		}
	}
}
