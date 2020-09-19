Shader "Unlit/Planet Core/Type 1"
{
	Properties
	{
		_Noise1("Noise 1", 2D) = "white" {}
		_Noise2("Noise 2", 2D) = "white" {}
		_Noise3("Noise 3", 2D) = "white" {}
		_AmbientTex("Ambient", 2D) = "black" {}
		_Color1("Color Lower", color) = (1,1,1,1)
		_Color2("Color Middle", color) = (1,1,1,1)
		_Color3("Color High", color) = (1,1,1,1)
		_Threshold1("Threshold 1", Range(0.0, 1.0)) = 0.3
		_Threshold2("Threshold 2", Range(0.0, 1.0)) = 0.6
		_Value("Channel Multiplier", Vector) = (1,1,1,1)
		_AmbientColor("Ambient Color", color) = (1,1,1,0)
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
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
					float2 uv3 : TEXCOORD2;
					float2 uvAmbient : TEXCOORD3;
				};

				sampler2D _Noise1;
				float4 _Noise1_ST;
				sampler2D _Noise2;
				float4 _Noise2_ST;
				sampler2D _Noise3;
				float4 _Noise3_ST;
				sampler2D _AmbientTex;
				float4 _AmbientTex_ST;
				fixed4 _Color1;
				fixed4 _Color2;
				fixed4 _Color3;
				fixed4 _AmbientColor;
				float _Threshold1;
				float _Threshold2;
				float4 _Value;
				static const fixed4 clear = fixed4(0.0, 0.0, 0.0, 0.0);

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _Noise1);
					o.uv2 = TRANSFORM_TEX(v.uv, _Noise2);
					o.uv3 = TRANSFORM_TEX(v.uv, _Noise3);
					o.uvAmbient = TRANSFORM_TEX(v.uv, _AmbientTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed r = tex2D(_Noise1, i.uv).r;
					fixed g = tex2D(_Noise2, i.uv2).r;
					fixed b = tex2D(_Noise3, i.uv3).r;
					float sum = r * _Value.x + g * _Value.y + b * _Value.z;
					fixed4 col = clear;
					col += lerp(clear, _Color1, step(sum, _Threshold1));
					col += lerp(clear, _Color2, step(0.0, (_Threshold2 - sum) * sign(sum - _Threshold1)));
					col += lerp(clear, _Color3, step(_Threshold2, sum));

					fixed ambient = tex2D(_AmbientTex, i.uvAmbient).r * _AmbientColor.a;
					return (1.0 - ambient) * col + ambient * _AmbientColor;
				}
				ENDCG
			}
		}
}
