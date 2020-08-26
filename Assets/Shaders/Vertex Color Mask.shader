Shader "Custom/VertexColorMask"
{
    Properties
    {
        _MainTex ("Albedo (RGB) (R mask)", 2D) = "white" {}
		_Color1 ("Color (G)", color) = (1,1,1,1)
		_Color2 ("Color (B)", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
			fixed4 _Color1;
			fixed4 _Color2;

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
				return i.color.r * tex2D(_MainTex, i.uv) + i.color.g * _Color1 + i.color.b * _Color2;
			}
		ENDCG
		}
    }
}
