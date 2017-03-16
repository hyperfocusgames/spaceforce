using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody ), typeof( Magnet ) )]
public class Controller : MonoBehaviour
{
	public static float sensX = 1f;
	public static float sensY = 1f;
	public static float sensZ = 1f;

	protected Magnet mag;


	protected void Start ()
	{
		GetComponent<Rigidbody>().freezeRotation = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		mag = GetComponent<Magnet>();
	}

	protected virtual void Update ()
	{
		rotate();
	}

	// Handle x,y,z rotation
	protected void rotate()
	{
		transform.Rotate(	-(Input.GetAxis("Y") * sensY),
							(Input.GetAxis("X") * sensX),
							(Input.GetAxis("Z") * sensZ));
	}
}
