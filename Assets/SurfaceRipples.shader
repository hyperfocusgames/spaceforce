// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Magnet/Surface Ripples" {
	Properties {}
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

			float _MagneticFlag = 0;	// is this object magnetic?
			float _StaticFlag = 0;		// is this object static?

			float _Wavelength;
			float _Speed;
			float _Sharpness;
			float _Radius;
			float _PulseWidth;

			// enabled flags for non static objects
			float _PushEnabledLocal = 0;
			float _PullEnabledLocal = 0;

			// enabled flags for static objects
			float _PushEnabledStatic = 0;
			float _PullEnabledStatic = 0;

			fixed4 _PushColor;
			fixed4 _PullColor;

			float4 _PushCenter;
			float4 _PullCenter;

			struct v2f {
				float4 vertex : SV_POSITION;
				float rim : TEXCOORD0;
				float4 worldVert : TEXCOORD1;
			};

			float effect(v2f i, float4 center, float speedSign) {
				center.w = 1;
				float4 delta = center - i.worldVert;
				float radius2 = _Radius * _Radius;
				float rim = 0;
				if (_StaticFlag == 0) rim = i.rim;
				float dist2 = dot(delta, delta);
				if (dist2 < radius2) {
					float intensity = 1 - (sin(dist2 / _Wavelength + _Time.y * _Speed * speedSign) + _PulseWidth) * _Sharpness;
					float edgeWidth = 0.2 / _Sharpness;
					if (radius2 - dist2 <= edgeWidth) {
						intensity *= smoothstep(0, 1, (radius2 - dist2) /  edgeWidth);
					}
					intensity = clamp(intensity, 0, 1);
					intensity += i.rim;
					return intensity;
				}
				else {
					return rim;
				}
			}

			v2f vert (appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVert = mul(unity_ObjectToWorld, v.vertex);
				float3 view = normalize(ObjSpaceViewDir(v.vertex));
				float t = 1 - dot(v.normal, view);
				float rimWidth = 0.95;
				o.rim = smoothstep(1 - rimWidth, 1, t);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				if (_MagneticFlag > 0) {
					float pushEnabled = _StaticFlag != 0 ? _PushEnabledStatic : _PushEnabledLocal;
					float pullEnabled = _StaticFlag != 0 ? _PullEnabledStatic : _PullEnabledLocal;					
					float push = pushEnabled != 0 ? effect(i, _PushCenter, 1) : 0;
					float pull = pullEnabled != 0 ? effect(i, _PullCenter, -1) : 0;
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
