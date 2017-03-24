Shader "Magnet/Bezier Beam" {
	Properties {
		_Color ("Beam Color", Color) = (1, 1, 1, 1)
		_BeamWidth ("Beam Width", Float) = 0.25
		_FadeLength ("Fade Length", Float) = 0.5
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

			#define POINT_COUNT 4

			sampler2D _CameraDepthTexture;

			float4 curve_points[POINT_COUNT];

			fixed4 _Color;
			float _BeamWidth;
			float _FadeLength;

			struct v2f {
				float4 pos: SV_POSITION;
				float curvelength: TEXCOORD0;
				float t: TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};

			struct bezier_point {
				float4 pos;
				float4 tan;
				float4 norm1;
				float4 norm2;
			};

			bezier_point bezier(float t) {
				float s = 1 - t;
				float s2 = s * s;
				float s3 = s2 * s;
				float t2 = t * t;
				float t3 = t2 * t;
				bezier_point bp;
				bp.pos
					= s3 * curve_points[0]
					+ 3 * s2 * t * curve_points[1]
					+ 3 * s * t2 * curve_points[2]
					+ t3 * curve_points[3];
				bp.tan
					= 3 * s2 * (curve_points[1] - curve_points[0])
					+ 6 * s * t * (curve_points[2] - curve_points[1])
					+ 3 * t2 * (curve_points[3] - curve_points[2]);
				bp.tan = normalize(bp.tan);
				bp.norm1.xyz = normalize(cross(bp.tan.xyz, float3(1, 0, 0)));
				bp.norm2.xyz = normalize(cross(bp.tan.xyz, bp.norm1.xyz));
				bp.norm1.w = bp.norm2.w = 0;
				return bp;
			}

			v2f vert (float4 v : POSITION) {
				v2f o;
				o.t = v.z;
				bezier_point bp = bezier(o.t);
				v
					= bp.pos
					+ bp.norm1 * v.x * _BeamWidth
					+ bp.norm2 * v.y * _BeamWidth;
				o.pos = mul(UNITY_MATRIX_VP, v);
				o.curvelength = 0;
				for (int i = 1; i < POINT_COUNT; i ++) {
					o.curvelength += distance(curve_points[i], curve_points[i - 1]);
				}
				o.screenPos = ComputeScreenPos(o.pos);
				// o.screenPos.y = 1 - o.screenPos.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 color = _Color;
				color.w *= smoothstep(0, 1, i.t * i.curvelength / _FadeLength);
				color.w *= smoothstep(0, 1, (1 - i.t) * i.curvelength / _FadeLength);
				return color;
				// TODO: depth stuff
				float sceneZ = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
    			float objectZ = i.pos.z;
    			// float intensityFactor = 1 - saturate(());
				// float depth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
				if (saturate(objectZ - sceneZ) < 0.1) {
					return fixed4(1, 0, 0, 1);
				}
				else {
					return fixed4(0, 0, 1, 1);
				}
			}
ENDCG
		}
	}
	CustomEditor "BezierBeamEditor"
}
