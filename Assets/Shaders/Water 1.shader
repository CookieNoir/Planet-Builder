Shader "Unlit/Water 1"
{
    Properties
    {
        _MainTex ("Texture (R)", 2D) = "white" {}
		_Color("Color", color) = (1,1,1,1)
		_Color2("Color 2", color) = (1,1,1,1)

		_Vector("Amplitude, Frequency, Speed, Offset", Vector) = (0,0,0,0)
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _Color2;
			float4 _Vector;
			static const float PI = 3.14159;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.xy += _Vector.x * float2(
					cos(_Vector.y*PI*o.vertex.x + _Time.y*_Vector.z + _Vector.w), 
					sin(_Vector.y*PI*o.vertex.y + _Time.y*_Vector.z + _Vector.w)
					);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed tex = tex2D(_MainTex, i.uv).r;
				fixed4 col = _Color * tex + (1.0 - tex) * _Color2;
                return col;
            }
            ENDCG
        }
    }
}
