using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
}
