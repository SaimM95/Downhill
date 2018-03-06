using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {

	public GameObject Player;
	public bool resetting;

	private GameObject edgePoint;

	// look ahead distance relative to the player (used to trigger moving of ground)
	private const float GroundGenerationGap = 300;

	// distance to move the ground (to simulate infinitely long ground)
	private const int MoveDistance = 300;

	// stores initial position of ground components for game resetting purposes
	private Vector3 initialPosition;
	private Vector3 initialEdgePosition;

	// Use this for initialization
	void Start () {
		edgePoint = transform.Find("EdgePoint").gameObject;
		initialPosition = transform.position;
		initialEdgePosition = edgePoint.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (resetting) {
			resetting = false;
			return;
		}

		float generationPoint = Player.transform.position.z + GroundGenerationGap;
		float edge = edgePoint.transform.position.z;

		if (generationPoint > edge) {
			float curPosZ = transform.position.z;
			transform.position = new Vector3(transform.position.x, transform.position.y, curPosZ + MoveDistance);
		}
	}

	public void reset() {
		resetting = true;
		transform.position = initialPosition;
		edgePoint.transform.position = initialEdgePosition;
	}
}
