using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Controls speed of ball movement
	public float speed;

	// Store reference to 'Rigid Body' component
	private Rigidbody rigidBody;

	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		float moveH = Input.GetAxis ("Horizontal");
		float moveV = Input.GetAxis ("Vertical");

		// Apply vertical and horizontal forces to the rigid body
		Vector3 movement = new Vector3 (moveH, 0.0f, moveV);
		rigidBody.AddForce (movement * speed);
		
	}
}
