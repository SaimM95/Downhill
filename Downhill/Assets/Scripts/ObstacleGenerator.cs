using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

	public int size = 2;
	public GameObject GenerationPoint;

	private string[] obstacleStructure = {"010", "100", "001", "010", "111"};
	private string[] allObstacleCombs = {"001","010","011","100","101","110","111"};
	private int obstaclePosition = 0;

	// Use this for initialization
	void Start () {
		foreach (string structure in obstacleStructure) {
			createObstacle (structure);
		}
	}

	void LateUpdate () {
		float playerPosition = transform.position.z;
		float generationPoint = GenerationPoint.transform.position.z;

//		if (playerPosition > 100) {
//			if (playerPosition > obstaclePosition) {
//				Debug.Log ("<color=yellow>Game Over!</color>");
//			}
//			return;
//		}

		if (generationPoint > obstaclePosition) {
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
			createObstacleAtPosition ('L');
		} 
		if (chars [1] == '1') {
			createObstacleAtPosition ('M');
		}
		if (chars [2] == '1') {
			createObstacleAtPosition ('R');
		}
	}

	private void createObstacleAtPosition(char p) {
		Vector3 position = new Vector3 (getObstacleX(p), 1, obstaclePosition);
		GameObject obstacle = (GameObject) Resources.Load ("Obstacle");
		Instantiate (obstacle, position, obstacle.transform.rotation);
	}

	private float getObstacleX(char p) {
		float x;
		if (p == 'L')
			x = 0 - size - 1;
		else if (p == 'R')
			x = 0 + size + 1;
		else
			x = 0;
		
		return x;
	}
}
