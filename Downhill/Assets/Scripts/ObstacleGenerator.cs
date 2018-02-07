using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

	public int size = 2;

	private string[] obstacleStructure = {"100", "001", "010", "111"};
	private string[] allObstacleCombs = {"001","010","011","100","101","110","111"};

	// Use this for initialization
	void Start () {
		int z = 0;
		foreach (string structure in obstacleStructure) {
			z += 10;
			char[] chars = structure.ToCharArray ();

			if (chars [0] == '1') {
				createObstacle ('L', z);
			} 
			if (chars [1] == '1') {
				createObstacle ('M', z);
			}
			if (chars [2] == '1') {
				createObstacle ('R', z);
			}
		}
	}

	void FixedUpdate () {
		
	}

	private void createObstacle (char p, int z) {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Rigidbody rigidBody = cube.AddComponent<Rigidbody>();
		rigidBody.mass = 100;
		cube.transform.localScale = new Vector3 (size, size, size);
		setObstaclePosition (p, cube, z);
	}

	private void setObstaclePosition(char p, GameObject cube, int z) {
		float x;
		if (p == 'L')
			x = 0 - size - 1;
		else if (p == 'R')
			x = 0 + size + 1;
		else
			x = 0;

		cube.transform.position = new Vector3(x, 1, z);
	}
}
