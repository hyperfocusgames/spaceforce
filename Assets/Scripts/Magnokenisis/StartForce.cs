using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartForce : MonoBehaviour
{
	public Vector3 velocity;

	// Use this for initialization
	void Start ()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if(rb != null)
		{
			rb.velocity = velocity;
		}
	}
}
