using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {

	public GameObject GenerationPoint;
	public int MoveDistance = 300;

	private GameObject edgePoint;

	// Use this for initialization
	void Start () {
		edgePoint = transform.Find("EdgePoint").gameObject;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float generationPoint = GenerationPoint.transform.position.z;
		float edge = edgePoint.transform.position.z;

		if (generationPoint > edge) {
			float curPosZ = transform.position.z;
			transform.position = new Vector3(transform.position.x, transform.position.y, curPosZ + MoveDistance);
		}
	}
}
