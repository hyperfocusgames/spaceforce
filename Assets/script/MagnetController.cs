using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetController : MonoBehaviour {

	public enum Polarity { Push = 1, Pull = -1 }

	public Polarity polarity = Polarity.Push;					// does this magnet push or pull?
	public string axisName = "Push";
	public float maxRadialForce = 750;
	public AnimationCurve radialAttenuation = AnimationCurve.Linear(0, 1, 1, 0);
	public float maxLateralForce = 100;
	public AnimationCurve lateralAttenuation = AnimationCurve.Linear(0, 1, 1, 0);
	public float range = 10;


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
		if (!input) {
			Detarget();
		}
		else {
			// if we dont have a target, get one
			if (target == null) {
				RaycastHit hit;
				if (Physics.Raycast(source.position, source.forward, out hit, range)) {
					target = hit.collider.GetComponent<Magnetic>();
					if (target != null) {
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
				
				Vector3 worldSpaceAnchor = this.worldSpaceAnchor;
				Vector3 radial = worldSpaceAnchor - source.position;
				float distance = radial.magnitude;

				if (distance > range) {
					Detarget();
				}
				else {
					// otherwise, do the magnet thing

					// magnet force
					Vector3 lateral = Vector3.ProjectOnPlane(source.forward, radial);
					float attenuation = distance / range;
					Vector3 force
						= radial.normalized * radialAttenuation.Evaluate(attenuation) * maxRadialForce * (int) polarity
						+ lateral * lateralAttenuation.Evaluate(attenuation) * maxLateralForce;
					normal = force.normalized;
					if (target.mass < body.mass) {
						// if the target is less massive then the player, apply a weaker force
						force *= target.mass / body.mass;
					}
					if (target.body != null) {
						target.body.AddForceAtPosition(force, worldSpaceAnchor);
					}
					body.AddForce(- force);

					// ripple effect
					Material[] mats = target.render.sharedMaterials;
					if (mats.Length < 2) {
						System.Array.Resize(ref mats, mats.Length + 1);
					}
					mats[1] = rippleMaterial;
					target.render.sharedMaterials = mats;
					ripplePropertyBlock.SetVector(rippleCenterPropertyID, new Vector4(
						worldSpaceAnchor.x,
						worldSpaceAnchor.y,
						worldSpaceAnchor.z,
						1
					));
					target.render.SetPropertyBlock(ripplePropertyBlock);
				}
			}
		}
	}

	void Detarget() {
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


}
