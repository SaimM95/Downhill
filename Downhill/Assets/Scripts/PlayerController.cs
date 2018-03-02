using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Controls speed of ball movement
	public float Speed = 5.0f;
	public float ForwardSpeed = 1.0f;
	public float JumpForce = 7.0f;
	public float Radius = 1.0f;

	public LayerMask groundLayer;

	private Rigidbody rigidBody;
	private float verticalVelocity;
	private float distanceToGround;

	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		
	}

	void FixedUpdate () {
		float moveH = Input.GetAxis ("Horizontal");

		// Jump when player presses "space" key
		if (Input.GetKeyDown (KeyCode.Space)) {
			jump ();
		}

		// Apply horizontal forces to the rigid body
		Vector3 movement = new Vector3 (moveH, 0.0f, ForwardSpeed);
		rigidBody.AddForce (movement * Speed);
	}

	private void jump() {
		if (isGrounded ()) {
			// Apply vertical force to simulate a jump
			rigidBody.AddForce (new Vector3 (0, JumpForce, 0), ForceMode.Impulse);
		}
	}

	private bool isGrounded() {
		Vector3 position = transform.position;
		Vector3 direction = Vector3.down;
		float distance = 1.0f;

		return Physics.Raycast(position, direction, distance, groundLayer);
	}
}
