using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2Button : Controller
{
	protected Magnetic t1, t2;	// Targets for magnet
	protected float maxDist = 100f;

	protected override void Update()
	{
		base.Update();

		input();
	}
	
	protected virtual void input()
	{
		if(Input.GetAxisRaw("Fire1") > 0)
		{
			t1 = target();
			mag.force(t1.gameObject, true);
		}

		if(Input.GetAxisRaw("Fire2") > 0)
		{
			t2 = target();
			mag.force(t2.gameObject, false);
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
