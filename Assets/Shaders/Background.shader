Shader "Unlit/Background"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Noise("Noise Texture", 2D) = "black" {}
		_DimColor("Dim Color", color) = (0.0, 0.0, 0.0, 0.0)
		_Color("Color", color) = (1.0, 1.0, 1.0, 1.0)
		_Color2("Color 2", color) = (1.0, 1.0, 1.0, 1.0)
		_Threshold("Blending Threshold", Range(0.0, 1.0)) = 0.0
		_StepX("Noise X-Offset Per Second", float) = 0.01
		_StepY("Noise Y-Offset Per Second", float) = 0.01
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
                float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _Noise;
			float4 _Noise_ST;
			fixed4 _DimColor;
			fixed4 _Color;
			fixed4 _Color2;
			float _Threshold;
			float _StepX;
			float _StepY;
			static const float PI = 3.14159;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _Noise);
				o.uv2.x += _Time.y * _StepX;
				o.uv2.y += _Time.y * _StepY;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed noise = tex2D(_Noise, i.uv2).r;
				half f = (noise - _Threshold) / (1.0 - _Threshold);
				f *= f;
				half4 blend = lerp(_Color, _Color2, step(_Threshold, noise)*f);
                fixed4 col = lerp(tex2D(_MainTex, i.uv1), _DimColor, sqrt(noise) * _DimColor.a) + noise * blend * blend.a;
                return col;
            }
            ENDCG
        }
    }
}
