Shader "Magnet/Surface Ripples" {
	Properties {
		_RippleColor ("Ripple Color", Color) = (1, 1, 1, 1)
		_RippleCenter ("Ripple Center", Vector) = (1, 1, 1, 1)
		_Wavelength ("Wavelength", Float) = 1
		_Speed ("Speed", Float) = 5
		_Sharpness ("Sharpness", Float) = 1
		_Radius ("Radius", Float) = 3
		_PulseWidth ("Pulse Width", Range(-1, 1)) = 0
	}
	SubShader {

		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			fixed4 _RippleColor;
			float4 _RippleCenter;
			float _Wavelength;
			float _Speed;
			float _Sharpness;
			float _Radius;
			float _PulseWidth;

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 worldVert : TEXCOORD0;
			};

			v2f vert (appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVert = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				float4 delta = _RippleCenter - i.worldVert;
				float radius2 = _Radius * _Radius;
				float dist2 = dot(delta, delta);
				if (dist2 < radius2) {
					fixed4 color = _RippleColor;
					float intensity = 1 - (sin(dist2 / _Wavelength + _Time.y * _Speed) + _PulseWidth) * _Sharpness;
					float edgeWidth = 0.2 / _Sharpness;
					if (radius2 - dist2 <= edgeWidth) {
						intensity *= smoothstep(0, 1, (radius2 - dist2) /  edgeWidth);
					}
					color.w *= intensity;
					return color;
				}
				else {
					return 0;
				}
			}
			ENDCG
		}
	}
}
