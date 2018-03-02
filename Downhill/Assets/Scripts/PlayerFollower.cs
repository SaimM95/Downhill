using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
		transform.position = player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float playerZ = player.transform.position.z;
		transform.position = new Vector3(transform.position.x, transform.position.y, playerZ);
	}
}
