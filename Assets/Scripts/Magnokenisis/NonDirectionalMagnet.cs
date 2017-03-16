using UnityEngine;

public class NonDirectionalMagnet : Magnet
{
	public override void force(GameObject target, bool pull)
	{
		// Try to get target's Rigidbody
		Rigidbody targetRB = target.GetComponent<Rigidbody>();

		// Vector to target position
		Vector3 toTarget = target.transform.position - transform.position;

		float f = 0;	// Force to be applied

		if(targetRB != null)	// If target is movable ...
		{
			if(rb != null)			// ... and this is movable ...
			{
										// ... apply a force to this and target
				float totalMass = rb.mass + targetRB.mass;
				
				// Force on this
				f = (gradiant / toTarget.magnitude) * ((pull) ? -1 : 1) * (targetRB.mass / totalMass);
				rb.AddForce(f * toTarget, ForceMode.Force);

				// Force on target
				f = (gradiant / toTarget.magnitude) * ((pull) ? -1 : 1) * (rb.mass / totalMass);
				targetRB.AddForce(f * -toTarget, ForceMode.Force);
			}
			else					// ... and this is non-movable ...
			{
										// ... apply a force to target only
				// Force on target
				f = (gradiant / toTarget.magnitude) * ((pull) ? 1 : -1);
				targetRB.AddForce(f * -toTarget, ForceMode.Force);
			}
		}
		else					// If target is non-movable ...
		{
			if(rb != null)			// ... and this is movable ...
			{
										// ... apply a force to this only
				// Force on this
				f = (gradiant / toTarget.magnitude) * ((pull) ? -1 : 1);
				rb.AddForce(f * toTarget, ForceMode.Force);
			}
			else					// ... and this is non-movable ...
			{
										// ... DO NOTHING
			}
		}
	}
}
