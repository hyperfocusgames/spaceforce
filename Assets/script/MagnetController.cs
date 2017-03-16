using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetController : MonoBehaviour {

	public float forceMagnitude = 100;
	public float range = 10;
	public AnimationCurve attenuation = AnimationCurve.Linear(0, 1, 1, 0);
	public float radius = 0.5f;

	public MagnetState state { get; private set; }

	Rigidbody body;

	void Awake() {
		body = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		bool push = Input.GetButton("Push");
		bool pull = Input.GetButton("Pull");
		MagnetState state = this.state;
		if (push || pull) {
			if (state.target == null) {
				RaycastHit hit;
				if (Physics.SphereCast(transform.position, radius, transform.forward, out hit, range)) {
					Magnetic target = hit.collider.GetComponent<Magnetic>();
					if (target != null) {
						state.target = target;
						state.anchor = target.GetAnchor(hit.point);
						Debug.DrawRay(hit.point, hit.normal);
					}
				}
			}
			if (state.target == null) {
				state.mode = Mode.Off;
			}
			else {
				if (push && state.mode != Mode.Push) { state.mode = Mode.Push; }
				if (pull && state.mode != Mode.Pull) { state.mode = Mode.Pull; }
				Vector3 worldSpaceAnchor = state.target.transform.TransformPoint(state.anchor);
				Vector3 force = worldSpaceAnchor - transform.position;
				float distance = force.magnitude;
				force = force.normalized * attenuation.Evaluate(distance / range) * forceMagnitude * (int) state.mode;
				if (state.target.body != null) {
					state.target.body.AddForceAtPosition(force, worldSpaceAnchor);
				}
				body.AddForce(-force);
			}
		}
		else {
			state.target = null;
			state.mode = Mode.Off;
		}
		this.state = state;
	}

	public enum Mode { Off = 0, Push = 1, Pull = -1 }

	public struct MagnetState {
		public Mode mode;				// is the magnet pushing, pulling, or off
		public Magnetic target;		// currently targeted magnetic object
		public Vector3 anchor;		// where on the target is the effect anchored (local to target space)
	}

}
