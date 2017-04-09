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
	public MagnetController activeController { get; set; } // the controller actively affecting this object

	public float mass {
		get {
			return this.body != null ? body.mass : Mathf.Infinity;
		}
	}

	static int magneticFlag_ID;
	static int staticFlag_ID;
	static MaterialPropertyBlock block;

	void Awake() {
		body = GetComponent<Rigidbody>();
		render = GetComponent<Renderer>();
		if (block == null) {
			block = new MaterialPropertyBlock();
			magneticFlag_ID = Shader.PropertyToID("_MagneticFlag");
			staticFlag_ID = Shader.PropertyToID("_StaticFlag");
			block.SetFloat(magneticFlag_ID, 1);
		}
		block.SetFloat(staticFlag_ID, isStatic ? 1 : 0);
		render.SetPropertyBlock(block);
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
