Shader "Unlit/Three Color Vertex"
{
	Properties
	{
		_Color1("Color (0)", color) = (1,1,1,1)
		_Color2("Color (R)", color) = (1,1,1,1)
		_Color3("Color (G)", color) = (1,1,1,1)
		_MainTex("Ambient (R)", 2D) = "black" {}
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
				float4 color : COLOR;
				float4 uv: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv: TEXCOORD0;
			};

			fixed4 _Color1;
			fixed4 _Color2;
			fixed4 _Color3;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _ColorAmbient;

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
				fixed4 ambient = tex2D(_MainTex, i.uv).r *_ColorAmbient.a;
				return (1 - ambient) * (i.color.r >= 0.5 ? _Color2 : i.color.g >= 0.5 ? _Color3 : _Color1) + ambient * _ColorAmbient;
			}
		ENDCG
		}
	}
}
