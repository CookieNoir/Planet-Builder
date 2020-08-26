Shader "Unlit/Sprite Shader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_MaskTex("Mask Texture", 2D) = "black" {}
		[HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil("Stencil ID", Float) = 0
		[HideInInspector] _StencilOp("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255

		[HideInInspector] _ColorMask("Color Mask", Float) = 15

		[HideInInspector][Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
				Name "Default"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
				#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
					float4 worldPosition : TEXCOORD2;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				sampler2D _MaskTex;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				float4 _MainTex_ST;
				float4 _MaskTex_ST;

				v2f vert(appdata_t v)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.worldPosition = v.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
					OUT.color = v.color;
					OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					OUT.texcoord2 = TRANSFORM_TEX(v.texcoord2, _MaskTex);
					return OUT;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					half4 t1 = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;
					half t2 = tex2D(_MaskTex, IN.texcoord2);
					half4 color = (t1 * (1 - t2) + t1 * IN.color*t2*1.1)*IN.color.a;
					#ifdef UNITY_UI_CLIP_RECT
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
					#endif

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif

					return color;
				}
			ENDCG
			}
		}
}
