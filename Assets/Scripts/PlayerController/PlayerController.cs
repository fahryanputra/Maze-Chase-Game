using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed = 2.4f;
	// public GameObject spawnPoint;

	public Vector2 orientation;

	private Vector2 direction = Vector2.zero;
	private Vector2 nextDirection;

	private Node currentNode, previousNode, targetNode;

	public Node playerSpawn;

	private GameObject[] enemies;

	private int score = 0;
	private int life = 3;

	public Text scoreText, endScoreText, lifeText, endText;
	public GameObject endGameMenu;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1;

		Node node = GetNodeAtPosition (transform.localPosition);

		if (node != null) {
			currentNode = node;
			// Debug.Log (currentNode);
		}

		orientation = Vector2.left;
		direction = Vector2.left;
		ChangePosition (direction);

		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
	}

	void Update() {
		CheckInput ();

		Move ();

		UpdateOrientation ();
	}

	void CheckInput () {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			ChangePosition (Vector2.left);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			ChangePosition (Vector2.right);
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			ChangePosition (Vector2.up);
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			ChangePosition (Vector2.down);
		}
	}

	void ChangePosition (Vector2 d) {
		if (d != direction) {
			nextDirection = d;
		}

		if (currentNode != null) {
			Node moveToNode = CanMove (d);

			if (moveToNode != null) {
				direction = d;
				targetNode = moveToNode;
				previousNode = currentNode;
				currentNode = null;
			}
		}
	}

	void Move () {
		if (targetNode != currentNode && targetNode != null) {
			if (OverShotTarget ()) {
				currentNode = targetNode;

				transform.localPosition = currentNode.transform.position;

				Node moveToNode = CanMove (nextDirection);

				if (moveToNode != null) {
					direction = nextDirection;
				}

				if (moveToNode == null) {
					moveToNode = CanMove (direction);
				}

				if (moveToNode != null) {
					targetNode = moveToNode;
					previousNode = currentNode;
					currentNode = null;
				} else {
					direction = Vector2.zero;
				}
			} else {
				transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
			}
		}
	}

	void MoveToNode (Vector2 d) {
		Node moveToNode = CanMove (d);

		if (moveToNode != null) {
			transform.localPosition = moveToNode.transform.localPosition;
			currentNode = moveToNode;
		}
	}

	void UpdateOrientation () {
		if (direction == Vector2.left) {
			orientation = Vector2.left;
			transform.localScale = new Vector3 (-1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} else if (direction == Vector2.right) {
			orientation = Vector2.right;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} else if (direction == Vector2.up) {
			orientation = Vector2.up;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 90);
		} else if (direction == Vector2.down) {
			orientation = Vector2.down;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 270);
		}
	}

	Node CanMove (Vector2 d) {
		Node moveToNode = null;

		for (int i = 0; i < currentNode.neighbours.Length; i++) {
			if (currentNode.validDirections [i] == d) {
				moveToNode = currentNode.neighbours [i];
				break;
			}
		}

		return moveToNode;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Enemy") {
			// Debug.Log ("Hit");

			GameObject enemySpawn = GameObject.Find ("Enemy Spawn");

			GameBoard setTimer = GameObject.Find ("Game").GetComponent<GameBoard> ();

			setTimer.enemyReleaseTimer = 0;

			GameObject.Find ("Game").GetComponent<GameBoard> ().gameStart = false;

			transform.localPosition = playerSpawn.transform.localPosition;
			currentNode = playerSpawn;
			orientation = Vector2.left;
			direction = Vector2.left;
			ChangePosition (direction);

			for (int i = 0; i < enemies.Length; i++) {
				enemies [i].transform.position = enemySpawn.transform.localPosition;
			}

			RedEnemyPathfinding resetRed = GameObject.Find ("Red").GetComponent<RedEnemyPathfinding> ();

			resetRed.currentNode = enemySpawn.GetComponent<Node> ();
			resetRed.isInEnemySpawn = true;
			resetRed.direction = Vector2.up;
			resetRed.targetNode = resetRed.currentNode.neighbours [0];
			resetRed.previousNode = resetRed.currentNode;

			PinkEnemyPathfinding resetPink = GameObject.Find ("Pink").GetComponent<PinkEnemyPathfinding> ();

			resetPink.currentNode = enemySpawn.GetComponent<Node> ();
			resetPink.isInEnemySpawn = true;
			resetPink.direction = Vector2.up;
			resetPink.targetNode = resetPink.currentNode.neighbours [0];
			resetPink.previousNode = resetPink.currentNode;

			BlueEnemyPathfinding resetBlue = GameObject.Find ("Blue").GetComponent<BlueEnemyPathfinding> ();

			resetBlue.currentNode = enemySpawn.GetComponent<Node> ();
			resetBlue.isInEnemySpawn = true;
			resetBlue.direction = Vector2.up;
			resetBlue.targetNode = resetBlue.currentNode.neighbours [0];
			resetBlue.previousNode = resetBlue.currentNode;

			OrangeEnemyPathfinding resetOrange = GameObject.Find ("Orange").GetComponent<OrangeEnemyPathfinding> ();

			resetOrange.currentNode = enemySpawn.GetComponent<Node> ();
			resetOrange.isInEnemySpawn = true;
			resetOrange.direction = Vector2.up;
			resetOrange.targetNode = resetOrange.currentNode.neighbours [0];
			resetOrange.previousNode = resetOrange.currentNode;

			life -= 1;
			SetLifeText ();

			if (life == 0) {
				GameObject.Find ("Game").GetComponent<GameBoard> ().disableInput = true;
				if (Time.timeScale == 1) {
					FindObjectOfType<AudioManager> ().Stop ("ThemeSong");
					FindObjectOfType<AudioManager> ().Play ("GameLose");

					Time.timeScale = 0;

					gameObject.SetActive (false);

					SetEndText ();
					endText.gameObject.SetActive (true);
					if (endGameMenu != null) {
						endGameMenu.SetActive (true);
					}
				}
			}
		} else if (other.tag == "Point") {

			FindObjectOfType<AudioManager> ().Play ("GetPoint");
			score += 10;
			SetScoreText ();

			if (score == 1560) {
				if (Time.timeScale == 1) {
					Time.timeScale = 0;

					SetEndText ();
					if (endGameMenu != null) {
						endGameMenu.SetActive (true);
					}
				}
			}
		}
	}

	Node GetNodeAtPosition (Vector2 pos) {
		GameObject tile = GameObject.Find ("Game").GetComponent<GameBoard> ().board [(int)pos.x, (int)pos.y];

		if (tile != null) {
			return tile.GetComponent<Node> ();
		}

		return null;
	}

	bool OverShotTarget () {
		float nodeToTarget = LengthFromNode (targetNode.transform.position);
		float nodeToSelf = LengthFromNode (transform.localPosition);

		return nodeToSelf > nodeToTarget;
	}

	float LengthFromNode (Vector2 targetPosition) {
		Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
		return vec.sqrMagnitude;
	}

	void SetScoreText () {
		scoreText.text = "Score: " + score.ToString ();
		if (endScoreText != null) {
			endScoreText.text = "Score: " + score.ToString ();
		}
	}

	void SetLifeText () {
		lifeText.text = "Life: " + life.ToString ();
	}

	void SetEndText () {
		if (score == 1560) {
			FindObjectOfType<AudioManager> ().Stop ("ThemeSong");
			FindObjectOfType<AudioManager> ().Play ("GameWin");

			endText.text = "YOU WIN!";
		}

		if (life == 0) {
			endText.text = "YOU LOSE!";
		}
	}
}

/*
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.CompareTag ("Point")) {
			other.gameObject.SetActive (false);
			score = score + 10;
			// SetScoreText ();
		}
		if (other.gameObject.CompareTag ("Enemy")) {
			// this.transform.position = spawnPoint.transform.position;
			life = life - 1;
			// SetLifeText ();
			if (life == 0) {
				Destroy (gameObject);
			}
		}
	}

	void SetScoreText () {
		scoreText.text = "Score: " + score.ToString ();
	}

	void SetLifeText () {
		lifeText.text = "Life: " + life.ToString ();
	}
	*/
