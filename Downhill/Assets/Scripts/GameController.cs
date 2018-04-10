using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject Player;
	public GameObject Ground;

	public Text ScoreText;
	public Text HiscoreText;
	public Text TimeText;
	public Text CheckpointText;
	public Text GameOverText;
	public Text DifficultyText;
	public Button PlayAgainButton;
	public RawImage HiScoreImage;

	private string[] InitialObstacleStructure = {"010", "100", "001", "010", "111"};
	private string[] EasyObstacleCombs = {"001","010","011","100","101","110","111"};
	private string[] MediumObstacleCombs = {"001","010","011","100","101","110","111","111","111"};
	private string[] HardObstacleCombs = {"001","010","011","100","101","110","111","111","111","111","111"};
	private string[] SpecialObstacleCombs = { "101300","110300","011300","111200","111020", "110200", "011020",
												"111001", "111100", "101101", "010010", "111101"};

	// look ahead distance in front of the player for obstacle generation
	private const int ObstacleGenerationPointGap = 50;
	private const int ObstacleSize = 2;
	private const int ObstacleDistance = 20;

	// look ahead distance in front of the player for checkpost generation
	private const int CheckPostGenerationPointGap = 300;
	private const int CheckPointDistance = 400;

	// number of seconds to complete each level
	private const int LevelTime = 35;

	// number of seconds to display the "Checkpoint!" text for
	private const int CheckPointDisplayTime = 2;

	// height that obstacles spawn at (and then drop)
	private const int ObstacleDropHeight = 10;

	// Difficulty levels
	private const int EASY = 0, MEDIUM = 1, HARD = 2;
	// Number of levels after the difficulty increases
	private const int DIFFICULTY_MULT = 2;

	private const string KEY_HISCORE = "hiscore";

	private int difficulty = EASY;
	private int hiscore = 0;

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

		hiscore = loadHiScore ();
		createInitialStructure ();

		CheckpointText.enabled = false;
		GameOverText.enabled = false;
		PlayAgainButton.gameObject.SetActive(false);
		HiScoreImage.enabled = false;
		DifficultyText.text = "Difficulty: Easy";
		DifficultyText.color = Color.green;
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
		HiScoreImage.enabled = false;
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
			obstacleRowsGenerated++;
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

		saveHiScore ();
	}

	private bool isGameOver() {
		return timeLeft <= 0 || isPlayerBelowGround();
	}

	private int loadHiScore() {
		if (PlayerPrefs.HasKey(KEY_HISCORE)) {
			return PlayerPrefs.GetInt (KEY_HISCORE);
		}
		return 0;
	}

	private void updateHiScore() {
		if (score > hiscore) {
			hiscore = score;
		}
		HiscoreText.text = "High Score\n" + hiscore.ToString();
	}

	private void saveHiScore() {
		int prevHiScore = loadHiScore();

		if (prevHiScore < hiscore) {
			PlayerPrefs.SetInt (KEY_HISCORE, hiscore);
			Debug.Log ("High score:" + hiscore.ToString () + " saved");
			HiScoreImage.enabled = true;
		}
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

		updateHiScore ();
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
		updateDifficulty ();
	}

	private void updateDifficulty() {
		Debug.Log ("update difficulty");

		if (level > 0 && level % DIFFICULTY_MULT == 0) {
			if (difficulty == EASY) {
				difficulty = MEDIUM;
			} else if (difficulty == MEDIUM) {
				difficulty = HARD;
			}
		}

		if (difficulty == EASY) {
			DifficultyText.text = "Difficulty: Easy";
			DifficultyText.color = Color.green;
		} else if (difficulty == MEDIUM) {
			DifficultyText.text = "Difficulty: Medium";
			DifficultyText.color = Color.blue;
		} else {
			DifficultyText.text = "Difficulty: Hard";
			DifficultyText.color = Color.red;
		}
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
		obstacleRowsGenerated++;

		if (isSpecial ()) {
			int rand = (int)Random.Range (0, SpecialObstacleCombs.Length);
			createSpecialObstacle (SpecialObstacleCombs [rand], posZ);
			return;
		}

		if (difficulty == EASY) {
			int rand = (int)Random.Range (0, EasyObstacleCombs.Length);
			createObstacle (EasyObstacleCombs [rand], posZ);
		} else if (difficulty == MEDIUM) {
			int rand = (int)Random.Range (0, MediumObstacleCombs.Length);
			createObstacle (MediumObstacleCombs [rand], posZ);
		} else {
			int rand = (int)Random.Range (0, HardObstacleCombs.Length);
			createObstacle (HardObstacleCombs [rand], posZ);
		}
			
	}

	private bool isSpecial() {
		int spec = (int)Random.Range (1, 11); // random from 1-10
		if (difficulty == EASY) {
			return spec == 1;
		} else if (difficulty == MEDIUM) {
			return spec <= 3;
		} else {
			return spec <= 5;
		}
	}

	private void createObstacle (string structure, int posZ) {
		createObstacle (structure, posZ, 1);
	}

	private void createObstacle (string structure, int posZ, int row) {
		char[] chars = structure.ToCharArray ();

		if (chars [0] == '1') {
			createObstacleAtPosition ('L', posZ, row);
		} 
		if (chars [1] == '1') {
			createObstacleAtPosition ('M', posZ, row);
		}
		if (chars [2] == '1') {
			createObstacleAtPosition ('R', posZ, row);
		}
	}

	private void createSpecialObstacle(string structure, int posZ) {
		// Create bottom row
		createObstacle (structure, posZ);

		// Create top row
		char[] chars = structure.ToCharArray ();
		if (chars [3] == '3') {
			createSpan3ObstacleAtPosition (posZ);
		} else if (chars [3] == '2') {
			createSpan2ObstacleAtPosition ('L', posZ);
		}

		if (chars [4] == '2') {
			createSpan2ObstacleAtPosition ('R', posZ);
		}

		// If top row is just regular obstacles
		if (chars [3] == '1' || chars [4] == '1' || chars [5] == '1') {
			createObstacle (new string(chars,3,3) , posZ, 2);
		}
	}

	private void createObstacleAtPosition(char p, int posZ) {
		createObstacleAtPosition (p, posZ, 1);
	}

	private void createObstacleAtPosition(char p, int posZ, int row) {
		GameObject obstacle = (GameObject) Resources.Load ("Obstacle");
		float dropHeight = (row == 2) ? (float) (ObstacleDropHeight + 2.5) : (float) ObstacleDropHeight;
		Vector3 position = new Vector3 (getObstacleX(p), dropHeight, posZ);
		Instantiate (obstacle, position, obstacle.transform.rotation);
	}

	private void createSpan2ObstacleAtPosition(char p, int posZ) {
		Debug.Log ("creating span2");
		GameObject obstacle = (GameObject) Resources.Load ("ObstacleSpan2");
		float x = p == 'L' ? (float) -1.5 : (float) 1.5;
		Vector3 position = new Vector3 (x, ObstacleDropHeight + 3, posZ);
		Instantiate (obstacle, position, obstacle.transform.rotation);
	}

	private void createSpan3ObstacleAtPosition(int posZ) {
		GameObject obstacle = (GameObject) Resources.Load ("ObstacleSpan3");
		Vector3 position = new Vector3 (0, ObstacleDropHeight + 3, posZ);
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
