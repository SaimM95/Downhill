using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

	public int size = 2;
	public GameObject player;

	private string[] obstacleStructure = {"100", "001", "010", "111"};
	private string[] allObstacleCombs = {"001","010","011","100","101","110","111"};
	private int obstaclePosition = 0;

	// Use this for initialization
	void Start () {
		foreach (string structure in obstacleStructure) {
			createObstacle (structure);
		}
	}

	void LateUpdate () {
		float playerPosition = player.transform.position.z;

		if (playerPosition > 100) {
			if (playerPosition > obstaclePosition) {
				Debug.Log ("<color=yellow>Game Over!</color>");
			}
			return;
		}

		if (playerPosition > obstaclePosition - 20) {
			obstaclePosition += 10;
			generateRandomObstacle ();
		}
	}

	private void generateRandomObstacle() {
		int rand = (int) Random.Range (0, 6);
		createObstacle (allObstacleCombs[rand]);
	}

	private void createObstacle (string structure) {
		obstaclePosition += 10;
		char[] chars = structure.ToCharArray ();

		if (chars [0] == '1') {
			createCubeAtPosition ('L', obstaclePosition);
		} 
		if (chars [1] == '1') {
			createCubeAtPosition ('M', obstaclePosition);
		}
		if (chars [2] == '1') {
			createCubeAtPosition ('R', obstaclePosition);
		}
	}

	private void createCubeAtPosition(char p, int z) {
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
