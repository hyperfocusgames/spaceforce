using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetController : MonoBehaviour {

	public float forceMagnitude = 100;
	public float range = 10;
	public AnimationCurve attenuation = AnimationCurve.Linear(0, 1, 1, 0);
	public float radius = 0.5f;


	public MagnetState pushState { get; private set; }
	public MagnetState pullState { get; private set; }

	public MagnetBeamEffect pushBeamEffect;
	public MagnetBeamEffect pullBeamEffect;

	Rigidbody body;

	void Awake() {
		body = GetComponent<Rigidbody>();
		pushState = new MagnetState();
		pushBeamEffect.Initialize(this, pushState);
		pullState = new MagnetState();
		pullBeamEffect.Initialize(this, pullState);
	}

	void FixedUpdate() {
		bool push = Input.GetButton("Push");
		bool pull = Input.GetButton("Pull");
		UpdateState(pushState, push ? Mode.Push : Mode.Off);
		UpdateState(pullState, pull ? Mode.Pull : Mode.Off);
	}

	void UpdateState(MagnetState state, Mode mode) {
		// if the mode is switching to off, turn off and do nothing this update
		if (mode == Mode.Off) {
			state.mode = Mode.Off;
			state.target = null;
		}
		else {
			// if the mode switched, retarget
			if (state.mode != mode) {
				state.target = null;
				RaycastHit hit;
				if (Physics.SphereCast(transform.position, radius, transform.forward, out hit, range)) {
					Magnetic target = hit.collider.GetComponent<Magnetic>();
					if (target != null) {
						state.target = target;
						state.anchor = target.GetAnchor(hit.point);
						// Debug.DrawRay(hit.point, hit.normal);
					}
				}
			}
			// if we have no target, turn off. otherwise, use requested mode
			if (state.target == null) {
				state.mode = Mode.Off;
			}
			else {
				state.mode = mode;
			}
			// if the mode is not off, we have a target. do the magnet thing
			if (state.isActive) {
				Vector3 force = state.worldSpaceAnchor - transform.position;
				float distance = force.magnitude;
				force = force.normalized * attenuation.Evaluate(distance / range) * forceMagnitude * (int) mode;
				if (state.target.body != null) {
					state.target.body.AddForceAtPosition(force, state.worldSpaceAnchor);
				}
				body.AddForce(- force);
			}
		}
	}

	void OnDrawGizmos() {
		Color color = Gizmos.color;
		if (pushState != null && pushState.isActive) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(pushState.worldSpaceAnchor, radius);
		}
		if (pullState != null && pullState.isActive) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(pullState.worldSpaceAnchor, radius);
		}
		Gizmos.color = color;
	}

	public enum Mode { Off = 0, Push = 1, Pull = -1 }

	public class MagnetState {

		public Mode mode;								// is the magnet pushing, pulling, or off
		public Magnetic target;						// currently targeted magnetic object
		public Vector3 anchor;						// where on the target is the effect anchored (local to target space)

		public bool isActive {
			get {
				return mode != Mode.Off;
			}
		}

		public Vector3 worldSpaceAnchor {
			get {
				return target.transform.TransformPoint(anchor);
			}
		}

	}

}
