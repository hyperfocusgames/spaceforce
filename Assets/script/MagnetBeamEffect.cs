using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetBeamEffect : MonoBehaviour {

	public float normalStrength = 0.25f;

	public const string POINTS_ARRAY_NAME = "curve_points";
	public const int POINT_COUNT = 4;

	MagnetController controller;
	MagnetController.State state;
	MeshRenderer render;

	MaterialPropertyBlock propertyBlock;
	Vector4[] curvePoints;

	int pointsArrayID;

	void Awake() {
		render = GetComponent<MeshRenderer>();
		curvePoints = new Vector4[POINT_COUNT];
		pointsArrayID = Shader.PropertyToID(POINTS_ARRAY_NAME);
		propertyBlock = new MaterialPropertyBlock();
	}

	public void Initialize(MagnetController controller, MagnetController.State state) {
		this.controller = controller;
		this.state = state;
	}

	void Update() {
		render.enabled = state.isActive;
		if (state.isActive) {
			float distance = (state.worldSpaceAnchor - transform.position).magnitude;
			curvePoints[0] = transform.position;
			curvePoints[1] = transform.position + transform.forward * normalStrength * distance;
			curvePoints[2] = state.worldSpaceAnchor + state.normal * normalStrength * distance;
			curvePoints[3] = state.worldSpaceAnchor;
			// make sure the last component of the vectors is correct for positions
			for (int i = 0; i < curvePoints.Length; i ++) {
				curvePoints[i].w = 1;
			}
			propertyBlock.SetVectorArray(pointsArrayID, curvePoints);
			render.SetPropertyBlock(propertyBlock);
		}
	}


}
