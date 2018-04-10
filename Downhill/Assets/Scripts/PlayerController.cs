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
	public bool gameOver = false;
	public bool resetting;

	private Rigidbody rigidBody;
	private float verticalVelocity;

	// stores initial position of player for game resetting purposes
	private Vector3 initialPosition;

	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		initialPosition = transform.position;
	}

	void FixedUpdate () {
		if (resetting) {
			resetting = false;
			return;
		}
			
		if (gameOver) {
			// stop player from moving when game over
			rigidBody.velocity = Vector3.zero;
			return;
		}

		float moveH = Input.GetAxis ("Horizontal");

		// Jump when player presses "space" key
		if (Input.GetKeyDown (KeyCode.Space)) {
			jump ();
		}

		// Apply horizontal forces to the rigid body
		Vector3 movement = new Vector3 (moveH, 0.0f, ForwardSpeed);
		rigidBody.AddForce (movement * Speed);
	}

	public void reset() {
		resetting = true;
		transform.position = initialPosition;
		gameOver = false;
	}

	private void jump() {
		Debug.Log ("isgrounded:" + isGrounded ());
		if (isGrounded ()) {
			// Apply vertical force to simulate a jump
			rigidBody.AddForce (new Vector3 (0, JumpForce, 0), ForceMode.Impulse);

			// Play jump sound
			SoundManagerScript.PlayJumpSound();
		}
	}

	private bool isGrounded() {
		Vector3 position = transform.position;
		Vector3 direction = Vector3.down;
		float distance = 1.0f;

		return Physics.Raycast(position, direction, distance, groundLayer);
	}
}
