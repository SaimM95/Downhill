using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;

	private Vector3 offset;

	void Start () {
		// Get the offset between the camera and player object (i.e. a vector going from camera to player)
		offset = transform.position - player.transform.position;
	}

	void LateUpdate () {
		// Move the camera along with the player
		transform.position = player.transform.position + offset;
	}
}
