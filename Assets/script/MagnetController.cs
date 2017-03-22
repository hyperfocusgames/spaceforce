using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetController : MonoBehaviour {

	public float forceMagnitude = 100;
	public float range = 10;
	public AnimationCurve attenuation = AnimationCurve.Linear(0, 1, 1, 0);

	public State pushState { get; private set; }
	public State pullState { get; private set; }

	public MagnetBeamEffect pushBeamEffect;
	public MagnetBeamEffect pullBeamEffect;

	public Material pushRippleMaterial;
	public Material pullRippleMaterial;

	Rigidbody body;

	int rippleCenterPropertyID;

	void Awake() {
		body = GetComponent<Rigidbody>();
		pushState = new State();
		pushBeamEffect.Initialize(this, pushState);
		pullState = new State();
		pullBeamEffect.Initialize(this, pullState);
		rippleCenterPropertyID = Shader.PropertyToID("_RippleCenter");
	}

	void FixedUpdate() {
		bool push = Input.GetButton("Push");
		bool pull = Input.GetButton("Pull");
		UpdateState(pushState, push ? Mode.Push : Mode.Off, pushRippleMaterial);
		UpdateState(pullState, pull ? Mode.Pull : Mode.Off, pullRippleMaterial);
	}

	void UpdateState(State state, Mode mode, Material rippleMaterial) {
		// if the mode is switching to off, turn off and do nothing this update
		if (mode == Mode.Off) {
			state.mode = Mode.Off;
			if (state.target != null) {
				state.target.mode = Mode.Off;
				Material[] mats = state.target.render.sharedMaterials;
				if (mats.Length > 1) {
					mats[1] = null;
					state.target.render.sharedMaterials = mats;
				}
			}
			state.target = null;
		}
		else {
			// if the mode switched, retarget
			if (state.mode != mode) {
				state.target = null;
				RaycastHit hit;
				if (Physics.Raycast(transform.position, transform.forward, out hit, range)) {
					Magnetic target = hit.collider.GetComponent<Magnetic>();
					if (target != null && target.mode == Mode.Off) {
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
				state.target.mode = mode;
				Material[] mats = state.target.render.sharedMaterials;
				if (mats.Length < 2) {
					System.Array.Resize(ref mats, mats.Length + 1);
				}
				mats[1] = rippleMaterial;
				state.target.render.sharedMaterials = mats;
				Vector3 center = state.worldSpaceAnchor;
				state.ripplePropertyBlock.SetVector(rippleCenterPropertyID, new Vector4(
					center.x,
					center.y,
					center.z,
					1
				));
				state.target.render.SetPropertyBlock(state.ripplePropertyBlock);
				if (state.target.body != null) {
					state.target.body.AddForceAtPosition(force, center);
				}
				body.AddForce(- force);
			}
		}
	}


	public enum Mode { Off = 0, Push = 1, Pull = -1 }

	public class State {

		public Mode mode;								// is the magnet pushing, pulling, or off
		public Magnetic target;						// currently targeted magnetic object
		public Vector3 anchor;						// where on the target is the effect anchored (local to target space)
		public MaterialPropertyBlock ripplePropertyBlock;

		public State() {
			ripplePropertyBlock = new MaterialPropertyBlock();
		}

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
