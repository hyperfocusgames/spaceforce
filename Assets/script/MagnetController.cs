using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetController : MonoBehaviour {

	public enum Polarity { Push = 1, Pull = -1 }

	public Polarity polarity = Polarity.Push;					// does this magnet push or pull?
	public string axisName = "Push";
	public float forceMagnitude = 100;
	public float range = 10;
	public AnimationCurve attenuation = AnimationCurve.Linear(0, 1, 1, 0);


	public Material rippleMaterial;

	Rigidbody body;

	int rippleCenterPropertyID;

	public Magnetic target { get; private set; }						// currently targeted magnetic object
	public Vector3 anchor { get; private set; }						// where on the target is the effect anchored (local to target space)
	public Vector3 normal { get; private set; }						// the surface normal of the anchor
	public MaterialPropertyBlock ripplePropertyBlock { get; private set; }

	public bool isActive {
		get {
			return target != null;
		}
	}

	public Vector3 worldSpaceAnchor {
		get {
			return target.transform.TransformPoint(anchor);
		}
	}

	void Awake() {
		body = GetComponentInParent<Rigidbody>();
		rippleCenterPropertyID = Shader.PropertyToID("_RippleCenter");
		ripplePropertyBlock = new MaterialPropertyBlock();
	}

	void FixedUpdate() {
		bool input = Input.GetButton(axisName);
		Transform source = body.transform; // the source of the magnetic affect is not this object, but the root body
		// if the mode is switching to off, turn off and do nothing this update
		if (!input) {
			if (target != null) {
				Material[] mats = target.render.sharedMaterials;
				if (mats.Length > 1) {
					mats[1] = null;
					target.render.sharedMaterials = mats;
				}
				target.activeController = null;
			}
			target = null;
		}
		else {
			// if we dont have a target, get one
			if (target == null) {
				RaycastHit hit;
				if (Physics.Raycast(source.position, source.forward, out hit, range)) {
					target = hit.collider.GetComponent<Magnetic>();
					if (target != null && target.activeController == null) {
						anchor = target.GetAnchor(hit.point);
						target.activeController = this;
						// Debug.DrawRay(hit.point, hit.normal);
					}
					else {
						target = null;
					}
				}
			}
			// if we have a target, do the magnet thing
			if (target ) {
				Material[] mats = target.render.sharedMaterials;
				if (mats.Length < 2) {
					System.Array.Resize(ref mats, mats.Length + 1);
				}
				mats[1] = rippleMaterial;
				target.render.sharedMaterials = mats;
				Vector3 worldSpaceAnchor = this.worldSpaceAnchor;
				ripplePropertyBlock.SetVector(rippleCenterPropertyID, new Vector4(
					worldSpaceAnchor.x,
					worldSpaceAnchor.y,
					worldSpaceAnchor.z,
					1
				));
				target.render.SetPropertyBlock(ripplePropertyBlock);
				Vector3 toTarget = (worldSpaceAnchor - source.position);
				Vector3 dirToTarget = toTarget.normalized;
				float distance = toTarget.magnitude;
				normal = (source.forward - dirToTarget);
				Vector3 force
					= (dirToTarget + normal * (int) polarity).normalized
					* attenuation.Evaluate(distance / range)
					* forceMagnitude * (int) polarity;
				if (target.body != null) {
					target.body.AddForceAtPosition(force, worldSpaceAnchor);
				}
				body.AddForce(- force);
			}
		}
	}

}
