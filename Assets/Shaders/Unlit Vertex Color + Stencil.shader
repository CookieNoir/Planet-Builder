Shader "Unlit/Vertex Color + Stencil"
{
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG
		}
	}
}
