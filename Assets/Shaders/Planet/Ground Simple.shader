Shader "Unlit/Planet/Ground Simple"
{
	Properties
	{
		_MainTex("Texture (R)", 2D) = "black" {}
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

			Stencil {
				Ref 3
				Comp always
				Pass replace
			}

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float4 uv: TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv: TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				static const fixed4 black = fixed4(0.0, 0.0, 0.0, 1.0);

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 ambient = tex2D(_MainTex, i.uv).r * i.color.a;
					return (1.0 - ambient)*i.color + ambient * black;
				}
				ENDCG
			}
		}
}
