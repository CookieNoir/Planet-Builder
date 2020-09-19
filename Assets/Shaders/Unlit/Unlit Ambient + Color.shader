Shader "Unlit/Ambient + Color"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SecondTex("Ambient", 2D) = "black" {}
		_Color("Color (RGB)", color) = (1,1,1,0)
		_ColorAmbient("Ambient Color (RGB)", color) = (1,1,1,0)
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
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _SecondTex;
				float4 _SecondTex_ST;
				fixed4 _Color;
				fixed4 _ColorAmbient;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv2 = TRANSFORM_TEX(v.uv, _SecondTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed ambient = tex2D(_SecondTex, i.uv2).a * _ColorAmbient.a;
					return (1 - ambient) * tex2D(_MainTex, i.uv) * _Color + ambient * _ColorAmbient;
				}
				ENDCG
			}
		}
}
