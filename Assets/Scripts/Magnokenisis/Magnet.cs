using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Magnet : MonoBehaviour
{
	private Rigidbody rb;
	//public float force = 1;
	public float gradiant = 1;

	public GameObject target;		// TESTING : REMOVE WHEN DONE

    public Vector3 M
	{
		get { return transform.up; }
	}

    void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		//Vector3 toTarget = target.transform.position - transform.position;
		//Debug.Log(toTarget);
	}

	public void force(GameObject target, bool pull)
	{
		// Try to get target's Rigidbody
		Rigidbody targetRB = target.GetComponent<Rigidbody>();

		// Modifier to force to compensate for above/below target
		int directionModifier = 1;

		// Vector to target position
		Vector3 toTarget = target.transform.position - transform.position;
		if(toTarget.y > 0)
		{								// TODO: Fix this
										// Problem: If this is below target, push/pull is inverted
			directionModifier = -1;

			Debug.Log("below");
			//toTarget = transform.position - target.transform.position;
		}

		float f;	// Force to be applied

		if(targetRB != null)	// If target is movable ...
		{
			if(rb != null)			// ... and this is movable ...
			{
										// ... apply a force to this and target
				float totalMass = rb.mass + targetRB.mass;
				
				// Force on this
				f = Vector3.Dot(M, toTarget) * ((pull) ? -1 : 1) * (rb.mass / totalMass) * directionModifier;
				rb.AddForce(f * toTarget, ForceMode.Force);

				// Force on target
				f = Vector3.Dot(M, toTarget) * ((pull) ? -1 : 1) * (targetRB.mass / totalMass) * directionModifier;
				targetRB.AddForce(f * -toTarget, ForceMode.Force);

				Debug.Log("move both");
			}
			else					// ... and this is non-movable ...
			{
										// ... apply a force to target only
				// Force on target
				f = Vector3.Dot(M, toTarget) * ((pull) ? -1 : 1) * directionModifier;
				targetRB.AddForce(f * -toTarget, ForceMode.Force);

				Debug.Log("move target");
			}
		}
		else					// If target is non-movable ...
		{
			if(rb != null)			// ... and this is movable ...
			{
										// ... apply a force to this only
				// Force on this
				f = Vector3.Dot(M, toTarget.normalized) * (gradiant / toTarget.magnitude) * ((pull) ? -1 : 1) * directionModifier;
				rb.AddForce(f * toTarget, ForceMode.Force);

				Debug.Log("move this");
			}
			else					// ... and this is non-movable ...
			{
										// ... DO NOTHING
				Debug.Log("move nothing");
			}
		}
	}

	/*protected Vector3 magneticMoment()
	{
		Vector3 m = transform.forward;
	}*/
}

[CustomEditor(typeof(Magnet))]
public class MagnetEditor : Editor
{
	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
		Magnet t = (Magnet) target;
		
		if(GUILayout.Button("pull"))
		{
			t.force(t.target, true);
		}

		if(GUILayout.Button("push"))
		{
			t.force(t.target, false);
		}
	}
}
