using UnityEngine;

public class Magnet : MonoBehaviour
{
	protected Rigidbody rb;
	//public float force = 1;
	public float gradiant = 1;

    public Vector3 M
	{
		get { return transform.up; }
	}

    protected void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public virtual void force(GameObject target, bool pull)
	{
		// Try to get target's Rigidbody
		Rigidbody targetRB = target.GetComponent<Rigidbody>();

		// Modifier to force to compensate for above/below target
		int directionModifier = 1;

		// Vector to target position
		Vector3 toTarget = target.transform.position - transform.position;
		if(toTarget.y > 0)	// This is below target
		{					// Invert forces
			directionModifier = -1;
		}

		float f = 0;	// Force to be applied

		if(targetRB != null)	// If target is movable ...
		{
			if(rb != null)			// ... and this is movable ...
			{
										// ... apply a force to this and target
				float totalMass = rb.mass + targetRB.mass;
				
				// Force on this
				f = Vector3.Dot(M, toTarget) * ((pull) ? -1 : 1) * (targetRB.mass / totalMass) * directionModifier;
				rb.AddForce(f * toTarget, ForceMode.Force);

				// Force on target
				f = Vector3.Dot(M, toTarget) * ((pull) ? -1 : 1) * (targetRB.mass / totalMass) * directionModifier;
				targetRB.AddForce(f * -toTarget, ForceMode.Force);
			}
			else					// ... and this is non-movable ...
			{
										// ... apply a force to target only
				// Force on target
				f = Vector3.Dot(M, toTarget) * ((pull) ? -1 : 1) * directionModifier;
				targetRB.AddForce(f * -toTarget, ForceMode.Force);
				//Debug.Log("move target " + f);
			}
		}
		else					// If target is non-movable ...
		{
			if(rb != null)			// ... and this is movable ...
			{
										// ... apply a force to this only
				// Force on this
				f = Vector3.Dot(M, toTarget) * (gradiant / toTarget.magnitude) * ((pull) ? -1 : 1) * directionModifier;
				rb.AddForce(f * toTarget, ForceMode.Force);
			}
			else					// ... and this is non-movable ...
			{
										// ... DO NOTHING
			}
		}
	}
}
