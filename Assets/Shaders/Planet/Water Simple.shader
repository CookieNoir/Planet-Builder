Shader "Unlit/Planet/Water Simple"
{
    Properties
    {
        _MainTex ("Texture (R)", 2D) = "white" {}
		_Color ("Color", color) = (0.0, 0.0, 0.0, 0.0)
		_Amplitude ("Amplitude", float) = 0.0
		_Frequency ("Frequency", float) = 0.0
		_Speed ("Speed", float) = 0.0
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Stencil {
			Ref 2
			Comp Greater
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
			fixed4 _Color;
			float _Amplitude;
			float _Frequency;
			float _Speed;
			static const float PI = 3.14159;

            v2f vert (appdata v)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.xy += _Amplitude * float2(
					cos(_Frequency*PI*o.vertex.x + _Time.y*_Speed),
					sin(_Frequency*PI*o.vertex.y + _Time.y*_Speed)
					);
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed col = tex2D(_MainTex, i.uv).r;
                return col * i.color + (1.0 - col) * _Color;
            }
            ENDCG
        }
    }
}
