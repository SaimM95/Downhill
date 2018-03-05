using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	private string[] InitialObstacleStructure = {"010", "100", "001", "010", "111"};
	private string[] AllObstacleCombs = {"001","010","011","100","101","110","111"};

	// distance in front of the player for obstacle generation
	private const int ObstacleGenerationPointGap = 50;
	private const int ObstacleSize = 2;
	private const int ObstacleDistance = 20;

	// distance in front of the player for checkpost generation
	private const int CheckPostGenerationPointGap = 300;
	private const int CheckPointDistance = 500;

	// number of seconds to complete each level
	private const int LevelTime = 30;

	private int obstacleRowsGenerated = 1;
	private int checkpointsGenerated = 1;

	// values to keep track of the position of the next checkpoint/row relative to the player
	private int nextCheckpointPos = CheckPointDistance;
	private int nextObstacleRowPos = ObstacleDistance;

	// number of checkpoints crossed
	private int level = 0;
	// number of obstacle rows crossed
	private int row = 0;

	private int score = 0;
	private float timer = 0.0f;
	private int timeLeft = LevelTime;

	// Use this for initialization
	void Start () {
		foreach (string structure in InitialObstacleStructure) {
			int newObstaclePosition = obstacleRowsGenerated * ObstacleDistance;
			createObstacle (structure, newObstaclePosition);
		}
	}

	void LateUpdate () {
		float playerPosition = transform.position.z;
		float checkPostGenerationPoint = playerPosition + CheckPostGenerationPointGap;
		float obstacleGenerationPoint = playerPosition + ObstacleGenerationPointGap;

		int newCheckPostPosition = checkpointsGenerated * CheckPointDistance;
		if (checkPostGenerationPoint > newCheckPostPosition) {
			createCheckpostAtPosition (newCheckPostPosition);
		}

		int newObstaclePosition = obstacleRowsGenerated * ObstacleDistance;
		if (obstacleGenerationPoint > newObstaclePosition) {
			generateRandomObstacle (newObstaclePosition);
		}

		updateScore (playerPosition);
		updateTime ();
	}

	private void updateScore(float playerPosition) {
		if (playerPosition >= nextCheckpointPos) {
			levelUp ();
			nextCheckpointPos += CheckPointDistance;
		}

		if (playerPosition >= nextObstacleRowPos) {
			row++;
			nextObstacleRowPos += ObstacleDistance;
		}

		score = (level * 100) + row;

		Debug.Log("Level:" + level + " Row:" + row + " Score:" + score);
	}

	private void updateTime() {
		timer += Time.deltaTime;
		int seconds = (int) timer % 60;
		timeLeft = LevelTime - seconds;
		Debug.Log ("Time Left:" + timeLeft);
	}

	private void levelUp() {
		level++;
		timer = 0;
		timeLeft = LevelTime;
	}

	private void createCheckpostAtPosition(int posZ) {
		GameObject checkpost = (GameObject) Resources.Load ("Checkpost");
		Vector3 position = new Vector3 (checkpost.transform.position.x, checkpost.transform.position.y, posZ);
		Instantiate (checkpost, position, checkpost.transform.rotation);
		checkpointsGenerated++;
	}

	private void generateRandomObstacle(int posZ) {
		int rand = (int) Random.Range (0, 6);
		createObstacle (AllObstacleCombs[rand], posZ);
	}

	private void createObstacle (string structure, int posZ) {
		char[] chars = structure.ToCharArray ();

		if (chars [0] == '1') {
			createObstacleAtPosition ('L', posZ);
		} 
		if (chars [1] == '1') {
			createObstacleAtPosition ('M', posZ);
		}
		if (chars [2] == '1') {
			createObstacleAtPosition ('R', posZ);
		}

		obstacleRowsGenerated++;
	}

	private void createObstacleAtPosition(char p, int posZ) {
		GameObject obstacle = (GameObject) Resources.Load ("Obstacle");
		Vector3 position = new Vector3 (getObstacleX(p), 1, posZ);
		Instantiate (obstacle, position, obstacle.transform.rotation);
	}

	private float getObstacleX(char p) {
		float x;
		if (p == 'L')
			x = 0 - ObstacleSize - 1;
		else if (p == 'R')
			x = 0 + ObstacleSize + 1;
		else
			x = 0;

		return x;
	}
}
