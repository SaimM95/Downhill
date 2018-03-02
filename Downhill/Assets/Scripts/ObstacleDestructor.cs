using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDestructor : MonoBehaviour {

	public GameObject DestructionPoint;

	// Use this for initialization
	void Start () {
		DestructionPoint = GameObject.Find ("ObstacleDestructionPoint");
	}
	
	// Update is called once per frame
	void Update () {
		float obstaclePosition = transform.position.z;
		float destructionPoint = DestructionPoint.transform.position.z;

		// If the obstacle is behind the destruction point, destroy it
		if (obstaclePosition < destructionPoint) {
			Destroy (gameObject);	
		}
	}
}
