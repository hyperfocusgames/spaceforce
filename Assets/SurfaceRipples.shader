Shader "Magnet/Surface Ripples" {
	Properties {
	}
	SubShader {

		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite Off
		ZTest Always

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float _MagneticFlag = 0;

			float _Wavelength;
			float _Speed;
			float _Sharpness;
			float _Radius;
			float _PulseWidth;

			float _PushEnabled = 0;
			float _PullEnabled = 0;

			fixed4 _PushColor;
			fixed4 _PullColor;

			float4 _PushCenter;
			float4 _PullCenter;

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 worldVert : TEXCOORD0;
			};

			float ripple(float4 center, float4 worldVert, float speedSign) {
				center.w = 1;
				float4 delta = center - worldVert;
				float radius2 = _Radius * _Radius;
				float dist2 = dot(delta, delta);
				if (dist2 < radius2) {
					float intensity = 1 - (sin(dist2 / _Wavelength + _Time.y * _Speed * speedSign) + _PulseWidth) * _Sharpness;
					float edgeWidth = 0.2 / _Sharpness;
					if (radius2 - dist2 <= edgeWidth) {
						intensity *= smoothstep(0, 1, (radius2 - dist2) /  edgeWidth);
					}
					return clamp(intensity, 0, 1);
				}
				else {
					return 0;
				}
			}

			v2f vert (appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVert = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				if (_MagneticFlag != 0) {
					float push = _PushEnabled != 0 ? ripple(_PushCenter, i.worldVert, 1) : 0;
					float pull = _PullEnabled != 0 ? ripple(_PullCenter, i.worldVert, -1) : 0;
					fixed4 pushColor = _PushColor;
					fixed4 pullColor = _PullColor;
					pushColor.w *= push;
					pullColor.w *= pull;
					float t = (push - pull + 1) / 2;
					return pushColor * t + pullColor * (1 - t);
				}
				else {
					return 0;
				}
			}
			ENDCG
		}
	}
}
