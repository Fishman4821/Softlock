Shader "Unlit/Background" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Pass {
			CGPROGRAM
				#pragma vertex vertexFunction
				#pragma fragment fragmentFunction

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				float4 _Color;

				v2f vertexFunction (appdata IN) {
					v2f OUT;

					OUT.position = UnityObjectToClipPos(IN.vertex);

					return OUT;
				}

				fixed4 fragmentFunction (v2f IN) : SV_TARGET {
					return _Color;
				}
			ENDCG
		}
	}
}
