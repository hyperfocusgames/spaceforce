using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTarget : Controller
{
	protected Magnetic t1, t2;	// Targets for magnet
	protected float maxDist = 100f;

	protected override void Update()
	{
		base.Update();

		if(Input.GetAxisRaw("Fire1") > 0)
		{
			t1 = target();
		}

		if(Input.GetAxisRaw("Fire2") > 0)
		{
			t2 = target();
		}

		if(Input.GetAxisRaw("Hand1") != 0 && t1 != null)
		{
			mag.force(t1.gameObject, Input.GetAxisRaw("Hand1") < 0);
		}

		if(Input.GetAxisRaw("Hand2") != 0 && t2 != null)
		{
			mag.force(t2.gameObject, Input.GetAxisRaw("Hand2") < 0);
		}
	}

	protected Magnetic target()
	{
		Magnetic t = null;	// Target to be returned

		RaycastHit hit;
		if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDist))
		{
			t = hit.collider.gameObject.GetComponent<Magnetic>();
		}

		return t;
	}
}
