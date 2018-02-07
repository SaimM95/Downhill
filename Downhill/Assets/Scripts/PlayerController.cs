using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Controls speed of ball movement
	public float Speed = 5.0f;
	public float ForwardSpeed = 1.0f;
	public float JumpForce = 7.0f;
	public float Radius = 1.0f;

	private Rigidbody rigidBody;
	private float verticalVelocity;

	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		float moveH = Input.GetAxis ("Horizontal");

		// Apply vertical force to simulate a jump
		if (Input.GetKeyDown (KeyCode.Space) && isGrounded()) {
			rigidBody.AddForce (new Vector3 (0, JumpForce, 0), ForceMode.Impulse);
		}

		// Apply horizontal forces to the rigid body
		Vector3 movement = new Vector3 (moveH, 0.0f, ForwardSpeed);
		rigidBody.AddForce (movement * Speed);
	}

	private bool isGrounded() {
		return rigidBody.position.y <= Radius;
	}
}
