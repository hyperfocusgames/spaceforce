using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetBeamEffect : MonoBehaviour {

	MagnetController controller;
	MagnetController.MagnetState state;
	LineRenderer line;

	void Awake() {
		line = GetComponent<LineRenderer>();
		line.numPositions = 2;
		line.useWorldSpace = true;
	}

	public void Initialize(MagnetController controller, MagnetController.MagnetState state) {
		this.controller = controller;
		this.state = state;
	}

	void Update() {
		line.enabled = state.isActive;
		if (state.isActive) {
			line.SetPosition(0, transform.position);
			line.SetPosition(1, state.worldSpaceAnchor);
		}
	}


}
