using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraExtras : MonoBehaviour {

	public DepthTextureMode depthTextureMode;

	Camera cam;

	void Awake() {
		cam = GetComponent<Camera>();
		ApplyExtras();
	}

	void ApplyExtras() {
		cam.depthTextureMode = depthTextureMode;
	}

	void OnValidate() {
		if (cam != null) {
			ApplyExtras();
		}
	}

}
