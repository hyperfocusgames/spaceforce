using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Magnetic : MonoBehaviour {

	public bool isStatic {
		get {
			return body == null;
		}
	}

	public Rigidbody body { get; private set; }
	public Renderer render { get; private set; }

	public MagnetController.Mode mode { get; set; } // is this object being pushed, pulled, or not

	void Awake() {
		body = GetComponent<Rigidbody>();
		render = GetComponent<Renderer>();
	}

	// given a world space point on the surface of this object, return the
	// anchor point (in local space) that magnetic forces should be applied to
	public Vector3 GetAnchor(Vector3 hit) {
		return transform.InverseTransformPoint(hit);
		// if (isStatic) {
		// }
		// else {
		// 	return Vector3.zero; // the center of the object in local space is (0, 0, 0)
		// }
	}

}
