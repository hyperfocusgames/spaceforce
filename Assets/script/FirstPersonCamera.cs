using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonCamera : MonoBehaviour {

	public float mouselookSensitivity = 5;
	public float rollSpeed = 25;

	void Awake() {
	}

	void Update() {
		float roll = Input.GetAxis("Roll") * rollSpeed * Time.deltaTime;
		Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		mouse *= mouselookSensitivity;
		transform.Rotate(-mouse.y, mouse.x, roll);
	}

}
