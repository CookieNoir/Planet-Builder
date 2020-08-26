Shader "Unlit/Planet/Border Ring Simple"
{
    Properties
    {
        _MainTex ("Texture Mask (G)", 2D) = "white" {}
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

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
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);;
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = i.color;
				col.a *= tex2D(_MainTex, i.uv).g;
                return col;
            }
            ENDCG
        }
    }
}
