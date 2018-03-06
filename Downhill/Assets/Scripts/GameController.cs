using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject Player;
	public GameObject Ground;

	public Text ScoreText;
	public Text TimeText;
	public Text CheckpointText;
	public Text GameOverText;
	public Button PlayAgainButton;

	private string[] InitialObstacleStructure = {"010", "100", "001", "010", "111"};
	private string[] AllObstacleCombs = {"001","010","011","100","101","110","111"};

	// look ahead distance in front of the player for obstacle generation
	private const int ObstacleGenerationPointGap = 50;
	private const int ObstacleSize = 2;
	private const int ObstacleDistance = 20;

	// look ahead distance in front of the player for checkpost generation
	private const int CheckPostGenerationPointGap = 300;
	private const int CheckPointDistance = 500;

	// number of seconds to complete each level
	private const int LevelTime = 35;

	// number of seconds to display the "Checkpoint!" text for
	private const int CheckPointDisplayTime = 2;

	// height that obstacles spawn at (and then drop)
	private const int ObstacleDropHeight = 10;

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

	private float checkPointTextTimer = 0.0f;
	private bool displayCheckpointText = false;

	private bool resetting = false;
	private bool over = false;

	private PlayerController playerController;
	private GroundGenerator groundGenerator;

	// Use this for initialization
	void Start () {
		playerController = Player.GetComponent<PlayerController> ();
		groundGenerator = Ground.GetComponent<GroundGenerator> ();

		createInitialStructure ();

		CheckpointText.enabled = false;
		GameOverText.enabled = false;
		PlayAgainButton.gameObject.SetActive(false);
	}

	void LateUpdate () {
		if (isResetting()) {
			return;
		}

		if (isGameOver ()) {
			gameOver ();
			return;
		}

		over = false;

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

		if (displayCheckpointText) {
			showCheckPointText ();
		}
	}

	public void resetGame() {
		resetting = true;

		// reset timers
		timer = 0;
		checkPointTextTimer = 0;
		timeLeft = LevelTime;

		// reset score
		level = 0;
		row = 0;
		score = 0;

		// reset counters
		obstacleRowsGenerated = 1;
		checkpointsGenerated = 1;
		nextCheckpointPos = CheckPointDistance;
		nextObstacleRowPos = ObstacleDistance;

		// reset game objects
		groundGenerator.reset ();
		playerController.reset ();

		// reset UI
		CheckpointText.enabled = false;
		GameOverText.enabled = false;
		PlayAgainButton.gameObject.SetActive(false);

		destroyExistingObjects ();
		createInitialStructure ();

		resetting = false;
	}

	private bool isResetting() {
		return resetting || playerController.resetting || groundGenerator.resetting;
	}

	private void createInitialStructure() {
		foreach (string structure in InitialObstacleStructure) {
			int newObstaclePosition = obstacleRowsGenerated * ObstacleDistance;
			createObstacle (structure, newObstaclePosition);
		}
	}
		
	private void gameOver() {
		if (over) {
			return;
		}

		CheckpointText.enabled = false;
		playerController.gameOver = true;
		GameOverText.enabled = true;
		PlayAgainButton.gameObject.SetActive(true);
		over = true;
	}

	private bool isGameOver() {
		return timeLeft <= 0 || isPlayerBelowGround();
	}

	// update the score based on level and rows counters
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
		ScoreText.text = "Score\n" + score.ToString ();
	}

	private void updateTime() {
		timer += Time.deltaTime;
		int seconds = (int) timer % 60;
		timeLeft = LevelTime - seconds;
		TimeText.text = "Time\n" + timeLeft.ToString ();

		if (timeLeft <= 10) {
			TimeText.color = Color.red;
		} else {
			TimeText.color = Color.green;
		}
	}

	private void levelUp() {
		level++;
		timer = 0;
		timeLeft = LevelTime;
		displayCheckpointText = true;
	}

	// display checkpoint text for X amount of seconds and then disable it
	private void showCheckPointText() {
		checkPointTextTimer += Time.deltaTime;
		int seconds = (int)checkPointTextTimer % 60;
		CheckpointText.enabled = true;

		if (seconds >= CheckPointDisplayTime) {
			checkPointTextTimer = 0;
			CheckpointText.enabled = false;
			displayCheckpointText = false;
		}
	}

	private void createCheckpostAtPosition(int posZ) {
		GameObject checkpost = (GameObject) Resources.Load ("Checkpost");
		Vector3 position = new Vector3 (checkpost.transform.position.x, checkpost.transform.position.y, posZ);
		Instantiate (checkpost, position, checkpost.transform.rotation);
		checkpointsGenerated++;
	}

	private void generateRandomObstacle(int posZ) {
		int rand = (int) Random.Range (0, AllObstacleCombs.Length);
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
		Vector3 position = new Vector3 (getObstacleX(p), ObstacleDropHeight, posZ);
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

	private bool isPlayerBelowGround() {
		float playerY = transform.position.y;
		return playerY <= 0;
	}

	private void destroyExistingObjects() {
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject obstacle in obstacles) {
			Destroy(obstacle);
		}

		GameObject[] checkposts = GameObject.FindGameObjectsWithTag("Checkpost");
		foreach (GameObject checkpost in checkposts) {
			Destroy(checkpost);
		}
	}
}
