using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float speed = 2.9f;

	private GameObject player;

	public Node startingNode;

	private Node currentNode, targetNode, previousNode;
	private Vector2 direction, nextDirection;

	public enum EnemyType {
		Red,
		Pink,
		Blue,
		Orange
	}

	public EnemyType enemyType = EnemyType.Red;

	// Use this for initialization
	void Start () {
		// SetColor ();

		player = GameObject.FindGameObjectWithTag ("Player");

		Node node = GetNodeAtPosition (transform.localPosition);
		//Debug.Log (node);

		if (node != null) {
			currentNode = node;
			//Debug.Log ("currentNode = " + currentNode);
		}

		if (enemyType == EnemyType.Red) {
			direction = Vector2.left;
			targetNode = ChooseNextNode ();
		} else if (enemyType == EnemyType.Pink) {
			direction = Vector2.up;
			targetNode = currentNode.neighbours [0];
		} else if (enemyType == EnemyType.Blue) {
			direction = Vector2.down;
			targetNode = currentNode.neighbours [1];
		} else if (enemyType == EnemyType.Orange) {
			direction = Vector2.down;
			targetNode = currentNode.neighbours [1];
		}

		previousNode = currentNode;

		Move ();
	}
	
	// Update is called once per frame
	void Update () {
		Move ();

		SetColor ();
	}

	void SetColor () {
		if (enemyType == EnemyType.Red) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		} else if (enemyType == EnemyType.Pink) {
			GetComponent<SpriteRenderer> ().color = Color.magenta;
		} else if (enemyType == EnemyType.Blue) {
			GetComponent<SpriteRenderer> ().color = Color.blue;
		} else if (enemyType == EnemyType.Orange) {
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 0.64f, 0.0f);
		} 
	}

	// Method untuk menggerakkan agent
	void Move () {
		// Percabangan untuk menentukan apakah Node target sudah terlewati
		if (targetNode != currentNode && targetNode != null) {
			if (OverShotTarget ()) {
				//Debug.Log ("is Overshot");

				// Jika targer sudah terlewati maka akan mengisi nilai currentNode dengan nilai targetNode
				currentNode = targetNode;
				// Posisi saat ini akan diisi dengan posisi dari currentNode
				transform.localPosition = currentNode.transform.position;

				// Mencari Node baru yang akan dituju
				targetNode = ChooseNextNode ();
				//Debug.Log (targetNode + " at " + targetNode.transform.position);

				// Mengisi nilai previousNode dengan nilai currentNode
				previousNode = currentNode;

				// Mengosongkan nilai currentNode
				currentNode = null;
			} else {
				//Debug.Log ("not Overshot");

				transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
			} 
		}
	}

	Vector2 GetRedTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;
		Vector2 targetTile = new Vector2 (Mathf.RoundToInt (playerPosition.x), Mathf.RoundToInt (playerPosition.y));
		Debug.Log ("Red Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetPinkTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;
		Vector2 playerOrientation = player.GetComponent<PlayerController> ().orientation;

		int playerPosX = Mathf.RoundToInt (playerPosition.x);
		int playerPosY = Mathf.RoundToInt (playerPosition.y);

		Vector2 playerTile = new Vector2 (playerPosX, playerPosY);
		Vector2 targetTile = playerTile + (4 * playerOrientation);
		Debug.Log ("Pink Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetBlueTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;
		Vector2 playerOrientation = player.GetComponent<PlayerController> ().orientation;

		int playerPosX = Mathf.RoundToInt (playerPosition.x);
		int playerPosY = Mathf.RoundToInt (playerPosition.y);

		Vector2 playerTile = new Vector2 (playerPosX, playerPosY);
		Vector2 targetTile = playerTile + (2 * playerOrientation);

		Vector2 redPosition = GameObject.Find ("Red").transform.localPosition;

		int redPosX = Mathf.RoundToInt (redPosition.x);
		int redPosY = Mathf.RoundToInt (redPosition.y);

		redPosition = new Vector2 (redPosX, redPosY);

		float distance = GetDistance (redPosition, targetTile);

		targetTile = new Vector2 (redPosition.x + distance/2, redPosY + distance/2);
		Debug.Log ("Blue Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetOrangeTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;

		float distance = GetDistance (transform.localPosition, playerPosition);
		Vector2 targetTile = Vector2.zero;

		if (distance >= 6) {
			targetTile = new Vector2 (Mathf.RoundToInt (playerPosition.x), Mathf.RoundToInt (playerPosition.y));
		} else {
			targetTile = startingNode.transform.position;
		}
		Debug.Log ("Orange Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetTargetTile() {
		Vector2 targetTile = Vector2.zero;

		if (enemyType == EnemyType.Red) {
			targetTile = GetRedTargetTile ();
		} else if (enemyType == EnemyType.Pink) {
			targetTile = GetPinkTargetTile ();
		} else if (enemyType == EnemyType.Blue) {
			targetTile = GetBlueTargetTile ();
		} else if (enemyType == EnemyType.Orange) {
			targetTile = GetOrangeTargetTile ();
		}

		return targetTile;
	}

	Node ChooseNextNode () {
		// Inisialisasi variable targetTile
		Vector2 targetTile = Vector2.zero;

		targetTile = GetTargetTile ();
		/*
		Vector2 playerPosition = player.transform.position;
		targetTile = new Vector2 (Mathf.RoundToInt (playerPosition.x), Mathf.RoundToInt (playerPosition.y));
		*/

		Node moveToNode = null;

		// Inisialisasi array founNodes dan array foundNodesDirection
		// foundNodes untuk menyimpan Node mana yang telah ditemukan
		// foundNodesDirection untuk menyimpan arah Node yang telah ditemukan
		Node[] foundNodes = new Node[4];
		Vector2[] foundNodesDirection = new Vector2[4];

		int nodeCounter = 0;

		// Melakukan perulangan untuk mengisi variable foundNodes dan founNodesDirection dengan neighbors Node saat ini
		for (int i = 0; i < currentNode.neighbours.Length; i++) {
			if (currentNode.validDirections [i] != direction * -1) {
				foundNodes [nodeCounter] = currentNode.neighbours [i];
				foundNodesDirection [nodeCounter] = currentNode.validDirections [i];
				nodeCounter++;
			}
		}

		// Melakukan percabangan untuk menentukan jarak Node yang terdekat
		// Jika Node yang ditemukan hanya 1 maka Node tersebut akan dipilih secara otomatis
		if (foundNodes.Length == 1) {
			moveToNode = foundNodes [0];
			direction = foundNodesDirection [0];
		}

		// Jika Node yang ditemukan lebih dari 1 maka akan dilakukan perulangan untuk mencari jarak Node yang terdekat
		if (foundNodes.Length > 1) {
			float leastDistance = 1000f;

			for (int i = 0; i < foundNodes.Length; i++) {
				if (foundNodesDirection [i] != Vector2.zero) {
					// Menghitung jarak dari Node yang akan dipilih menuju Node tujuan kemudian diisikan ke dalam variable distance
					float distance = GetDistance (foundNodes [i].transform.position, targetTile);

					// Percabangan untuk menentukan jarak yang terpendek
					if (distance < leastDistance) {
						leastDistance = distance;
						// Melakukan pengisian nilai variable Node yang dituju dan arah yang dituju
						moveToNode = foundNodes [i];
						direction = foundNodesDirection [i];
					}
				}
			}
		}

		return moveToNode;
	}

	Node GetNodeAtPosition (Vector2 pos) {
		GameObject tile = GameObject.Find ("Game").GetComponent<GameBoard> ().board [(int)pos.x, (int)pos.y];

		if (tile != null) {
			if (tile.GetComponent<Node> () != null) {
				return tile.GetComponent<Node> ();
			}
		}
			
		return null;
	}

	// Menghitung jarak antara Node tujuan dan Node sebelumnya
	float LengthFromNode (Vector2 targetPosition) {
		Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
		return vec.sqrMagnitude;
	}

	// Mengecek apakah agent telah melewati Node target
	bool OverShotTarget () {
		// Melakukan penghitungan jarak dari Node sebelumnya menuju Node target
		float nodeToTarget = LengthFromNode (targetNode.transform.position);
		// Melakukan penghitungan jarak dari Node sebelumnya menuju Node saat ini
		float nodeToSelf = LengthFromNode (transform.localPosition);

		// jika jarak dari Node sebelumnya menuju Node saat ini lebih besar dari jarak Node sebelumnya menuju target maka Method ini akan menghasilkan nilai True
		return nodeToSelf > nodeToTarget;
	}

	// Penghitungan jarak antara 2 posisi
	float GetDistance (Vector2 posA, Vector2 posB) {
		float dx = posA.x - posB.x;
		float dy = posA.y - posB.y;

		float distance = Mathf.Sqrt (dx * dx + dy * dy);

		return distance;
	}
}
