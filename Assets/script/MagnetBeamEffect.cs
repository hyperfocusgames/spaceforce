using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetBeamEffect : MonoBehaviour {

	public float normalStrength = 0.25f;

	public const string POINTS_ARRAY_NAME = "curve_points";
	public const int POINT_COUNT = 4;

	MagnetController controller;
	MeshRenderer render;

	MaterialPropertyBlock propertyBlock;
	Vector4[] curvePoints;

	int pointsArrayID;

	void Awake() {
		controller = GetComponentInParent<MagnetController>();
		render = GetComponent<MeshRenderer>();
		curvePoints = new Vector4[POINT_COUNT];
		pointsArrayID = Shader.PropertyToID(POINTS_ARRAY_NAME);
		propertyBlock = new MaterialPropertyBlock();
	}

	void Update() {
		render.enabled = controller.isActive;
		if (controller.isActive) {
			float distance = (controller.worldSpaceAnchor - transform.position).magnitude;
			curvePoints[0] = transform.position;
			curvePoints[1] = transform.position + transform.forward * normalStrength * distance;
			curvePoints[2] = controller.worldSpaceAnchor + controller.normal * normalStrength * distance;
			curvePoints[3] = controller.worldSpaceAnchor;
			// make sure the last component of the vectors is correct for positions
			for (int i = 0; i < curvePoints.Length; i ++) {
				curvePoints[i].w = 1;
			}
			propertyBlock.SetVectorArray(pointsArrayID, curvePoints);
			render.SetPropertyBlock(propertyBlock);
		}
	}


}
