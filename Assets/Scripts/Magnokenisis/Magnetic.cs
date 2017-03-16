using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
	public Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
}
