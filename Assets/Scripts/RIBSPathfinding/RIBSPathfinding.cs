using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RIBSPathfinding : MonoBehaviour {

	public float speed = 2.4f;

	private GameObject player;
	private GameObject enemies;

	public Node startingNode;
	public Node retreatNode;

	private Node playerNode, newPlayerNode;

	[System.NonSerialized]
	public Node currentNode, targetNode, previousNode, nextNode;

	[System.NonSerialized]
	public Vector2 direction, nextDirection;

	public enum EnemyType {
		Red,
		Pink,
		Blue,
		Orange
	}

	public EnemyType enemyType = EnemyType.Red;

	[System.NonSerialized]
	public List<Node> path = new List<Node> ();

	[System.NonSerialized]
	public bool isInEnemySpawn;

	public int redReleaseTimer = 2; 
	public int pinkReleaseTimer = 7;
	public int blueReleaseTimer = 12;
	public int orangeReleaseTimer = 17;

	public float costLimit;

	public Text nodeExpandedText;

	// Use this for initialization
	void Start () {
		SetColor ();

		player = GameObject.FindGameObjectWithTag ("Player");
		enemies = GameObject.FindGameObjectWithTag ("Enemy");

		playerNode = GameObject.Find ("Spawn").GetComponent<Node> ();

		Node node = GetNodeAtPosition (transform.localPosition);
		// Debug.Log (node);

		if (node != null) {
			currentNode = node;

			if (node == startingNode) {
				isInEnemySpawn = true;
			}
			// Debug.Log ("currentNode = " + currentNode);
		}

		direction = Vector2.up;
		targetNode = currentNode.neighbours [0];
		targetNode.gCost = GetDistance (currentNode.transform.position, targetNode.transform.position);
		targetNode.hCost = GetDistance (targetNode.transform.position, player.transform.position);
		targetNode.weightFromVisit = 0;

		previousNode = currentNode;

		if (!isInEnemySpawn) {
			Move ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (GetNodeAtPosition (transform.localPosition) != null) {
			// Debug.Log ("is not null");
			FindPath (GetNodeAtPosition (transform.localPosition).transform.position, GetTargetTile ());

		}
		*/

		ReleaseEnemies ();

		if (!isInEnemySpawn) {
			Move ();
		}

		UpdateOrientation ();
		// Debug.Log ("curentNode: " + currentNode);
	}

	void UpdateOrientation () {
		if (direction == Vector2.left) {
			transform.localScale = new Vector3 (-1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} else if (direction == Vector2.right) {
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} else if (direction == Vector2.up) {
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 90);
		} else if (direction == Vector2.down) {
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 270);
		}
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

	void ReleaseEnemies () {

		float enemyReleaseTimer = GameObject.Find ("Game").GetComponent<GameBoard> ().enemyReleaseTimer;

		if (enemyReleaseTimer >= redReleaseTimer) {
			if (enemyType == EnemyType.Red && isInEnemySpawn) {
				isInEnemySpawn = false;
			}
		}
		if (enemyReleaseTimer >= pinkReleaseTimer) {
			if (enemyType == EnemyType.Pink && isInEnemySpawn) {
				isInEnemySpawn = false;
			}
		}
		if (enemyReleaseTimer >= blueReleaseTimer) {
			if (enemyType == EnemyType.Blue && isInEnemySpawn) {
				isInEnemySpawn = false;
			}
		}
		if (enemyReleaseTimer >= orangeReleaseTimer) {
			if (enemyType == EnemyType.Orange && isInEnemySpawn) {
				isInEnemySpawn = false;
			}
		}
	}

	Vector2 GetRedTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;
		Vector2 targetTile = new Vector2 (Mathf.RoundToInt (playerPosition.x), Mathf.RoundToInt (playerPosition.y));
		// Debug.Log ("Red Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetPinkTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;
		Vector2 playerOrientation = player.GetComponent<RIBSPlayerController> ().orientation;

		int playerPosX = Mathf.RoundToInt (playerPosition.x);
		int playerPosY = Mathf.RoundToInt (playerPosition.y);

		Vector2 playerTile = new Vector2 (playerPosX, playerPosY);
		Vector2 targetTile = playerTile + (2 * playerOrientation);
		// Debug.Log ("Pink Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetBlueTargetTile () {
		Vector2 playerOrientation = player.GetComponent<RIBSPlayerController> ().orientation;

		Vector2 redPosition = GameObject.Find ("Red").transform.localPosition;
		int redPosX = Mathf.RoundToInt (redPosition.x);
		int redPosY = Mathf.RoundToInt (redPosition.y);

		redPosition = new Vector2 (redPosX, redPosY);

		Vector2 targetTile = redPosition + (-2 * playerOrientation);
		// Debug.Log ("Blue Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetOrangeTargetTile () {
		Vector2 playerPosition = player.transform.localPosition;

		float distance = GetDistance (transform.localPosition, playerPosition);
		Vector2 targetTile = Vector2.zero;

		if (distance >= 40) {
			targetTile = new Vector2 (Mathf.RoundToInt (playerPosition.x), Mathf.RoundToInt (playerPosition.y));
		} else {
			targetTile = retreatNode.transform.position;
		}
		// Debug.Log ("Orange Target: " + targetTile);

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

	/*
	// Penjelasan
	// *** Pseudocode ***
	// Method A* Pathfinding
	void FindPath (Vector2 startPos, Vector2 targetPos) {
		// Debug.Log ("FindPath is called");

		// menggunakan fungsi GetNodeAtPosition () untuk mendapatkan Node pada posisi awal
		Node startNode = GetNodeAtPosition (startPos);
		// Debug.Log ("startPos: " + startPos);

		// pencegahan agar Method FindPath () tidak keluar dari arena permainan
		if (targetPos.x < 1) {
			targetPos.x = 1;
		} else if (targetPos.x > 15) {
			targetPos.x = 15;
		}

		if (targetPos.y < 1) {
			targetPos.y = 1;
		} else if (targetPos.y > 19) {
			targetPos.y = 19;
		}

		// menggunakan fungsi GetNodeAtPosition () untuk mendapatkan Node pada posisi target
		Node targetNode = GetNodeAtPosition (targetPos);
		// Debug.Log ("targetPos: " + targetPos);
		// Debug.Log ("targetNode: " + targetNode);

		// mendeklarasikan variabel openSet dengan tipe List
		// *** OPEN //the set of nodes to be evaluated ***
		List<Node> openSet = new List<Node> ();

		// mendeklarasikan variabel closeSet dengan tipe List
		// *** CLOSED //the set of nodes already evaluated ***
		HashSet<Node> closeSet = new HashSet<Node> ();

		// Memasukkan startNode ke dalam openSet apabila startNode dan targetNode tidak bernilai null
		// *** add the start node to OPEN ***
		if (startNode != null && targetNode != null) {
			openSet.Add (startNode);
			// Debug.Log ("Starting openSet: " + startNode);

			// loop untuk memulai pencarian jalur
			// loop akan terus terulang selama jumlah anggota openSet > 0
			// *** loop ***
			while (openSet.Count > 0) {
				// Debug.Log ("openSet.Count > 0");

				// mengisi currentNode dengan anggota openSet pertama
				Node currentNode = openSet [0];

				// melakukan loop sebanyak anggota openSet untuk mengecek Node yang memiliki fCost terendah
				for (int i = 1; i < openSet.Count; i++) {
					// pengecekan Node dengan fCost terendah
					if (openSet [i].fCost < currentNode.fCost ||openSet [i].fCost == currentNode.fCost && openSet [i].hCost < currentNode.hCost) {
						// mengisikan anggota openSet dengan fCost terendah ke dalam currentNode
						// *** current = node in OPEN with the lowest f_cost ***
						currentNode = openSet [i];
						// Debug.Log ("currentNode: " + currentNode);
					}
				}

				// menghapus currentNode dari openSet
				// *** remove current from OPEN ***
				openSet.Remove (currentNode);
				// menambahkan currentNode ke closeSet
				// *** add current to CLOSED ***
				closeSet.Add (currentNode);
				// Debug.Log ("closeSet: " + closeSet);

				// pengecekan apakah currentNode sama dengan targetNode, tujuan sudah tercapai
				// ***  if current is the target node //path has been found ***
				if (currentNode == targetNode) {
					// Debug.Log ("currentNode = targetNode");

					// pemanggilan method RetracePath untuk mengetahui jalur yang akan diambil
					RetracePath (startNode, targetNode);

					// keluar dari loop, algoritma selesai
					// *** return ***
					return;
				}

				// loop untuk mengecek tetangga dari currentNode
				// *** foreach neighbour of the current node ***
				for (int i = 0; i < currentNode.neighbours.Length; i++) {
					// pendeklarasian variabel neighbour yang diisi dengan tetangga currentNode yang akan dicek
					Node neighbour = currentNode.neighbours [i];
					// Debug.Log ("currentNode.neighbours : " + neighbour);

					// pengecekan apakah neighbour merupakan anggota closeSet
					// *** if neighbour is not traversable or neighbour is in CLOSED ***
					if (closeSet.Contains (neighbour)) {
						// berlanjut ke neighbour selanjutnya
						// *** skip to the next neighbour ***
						continue;
					}

					// pendeklarasian variabel newMovementCost yang diisi dengan pembaharuan nilai gCost
					int newMovementCost = Mathf.RoundToInt (currentNode.gCost + GetDistance (currentNode.transform.position, neighbour.transform.position));

					// pengecekan apakah newMovementCost kurang dari gCost atau neighbour bukan anggota openSet
					// *** if new path to neighbour is shorter OR neighbour is not in OPEN ***
					if (newMovementCost < neighbour.gCost || !openSet.Contains (neighbour)) {
						// penghitungan nilai fCost dengan cara memasukkan nilai ke dalam variabel gCost dan hCost
						// *** set f_cost of neighbour ***
						neighbour.gCost = newMovementCost;
						neighbour.hCost = GetDistance (neighbour.transform.position, targetNode.transform.position);

						// pengisian variabel neighbour.parent dengan currentNode
						// *** set parent of neighbour to current ***
						neighbour.parent = currentNode;

						// pengecekan apakah neighbour bukan anggota openSet
						// *** if neighbour is not in OPEN ***
						if (!openSet.Contains (neighbour)) {
							// tambahkan neighbour ke dalam openSet
							// *** add neighbour to OPEN ***
							openSet.Add (neighbour);
							// Debug.Log ("openSet: " + openSet);
						}
					}
				}
			}
		}
	}
	*/

	/*
	public void RetracePath (Node startNode, Node endNode) {
		// List<Node> foundPath = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();

		for (int i = 0; i < path.Count; i++) {
			Debug.Log ("path is: " + path [i] + " at " + path [i].transform.position);
		}
	}
	*/

	Node GetNodeAtPosition (Vector2 pos) {
		GameObject tile = GameObject.Find ("Game").GetComponent<GameBoard> ().board [(int)pos.x, (int)pos.y];

		if (tile != null) {
			if (tile.GetComponent<Node> () != null) {
				return tile.GetComponent<Node> ();
			}
		}

		return null;
	}

	float GetDistance (Vector2 nodeA, Vector2 nodeB) {
		float dstX = Mathf.Abs (nodeA.x - nodeB.x);
		float dstY = Mathf.Abs (nodeA.y - nodeB.y);

		return (10 * dstX) + (10 * dstY);
	}

	Node ChooseNextNode () {
		Vector2 targetTile = Vector2.zero;

		targetTile = GetTargetTile ();

		if (targetTile.x < 1) {
			targetTile.x = 1;
		} else if (targetTile.x > 15) {
			targetTile.x = 15;
		}

		if (targetTile.y < 1) {
			targetTile.y = 1;
		} else if (targetTile.y > 19) {
			targetTile.y = 19;
		}

		costLimit = GetDistance (transform.position, player.transform.position);

		Node moveToNode = null;
		// Node previousMoveToNode = null;

		Node[] foundNodes = new Node[4];
		Vector2[] foundNodesDirection = new Vector2[4];

		int nodeCounter = 0;

		for (int i = 0; i < currentNode.neighbours.Length; i++) {
			currentNode.neighbours [i].isExpanded = true;
			// Debug.Log ("path contains: " + currentNode.neighbours [i]);
			// if ((currentNode.neighbours [i].fCost <= costLimit)) {
			if (currentNode.neighbours [i] != previousNode) {
				foundNodes [nodeCounter] = currentNode.neighbours [i];
				// Debug.Log (foundNodes [i]);	
				foundNodesDirection [nodeCounter] = currentNode.validDirections [i];
				foundNodes [nodeCounter].gCost = GetDistance (currentNode.transform.position, foundNodes [nodeCounter].transform.position);
				foundNodes [nodeCounter].hCost = GetDistance (foundNodes [nodeCounter].transform.position, targetTile);
				nodeCounter++;
			}
			// }
			// Debug.Log (foundNodes [i]);
		}

		if (nodeExpandedText != null) {
			nodeExpandedText.text = "Expd\t: " + nodeCounter.ToString ();
		}
		// Debug.Log ("foundNodes.Length: " + foundNodes.Length);

		if (foundNodes.Length == 1) {
			// Debug.Log ("founNodes.length: 1");
			foundNodes [0].isVisited = true;
			foundNodes [0].parent = currentNode;
			moveToNode = foundNodes [0];
			direction = foundNodesDirection [0];
		} else if (foundNodes.Length > 1) {
			// Debug.Log ("foundNodes.length: more than 1");
			// float leastDistance = 1000f;
			float leastFCost = 1000;
			float leastWeightFromVisit = 1000;

			while (moveToNode == null) {
				for (int i = 0; i < foundNodes.Length; i++) {
					if (foundNodesDirection [i] != Vector2.zero) {
						if (foundNodes [i].fCost < costLimit || foundNodes [i].fCost == costLimit) {
							if (foundNodes [i].fCost < leastFCost) {
								if (foundNodes [i].weightFromVisit < leastWeightFromVisit) {
									leastFCost = foundNodes [i].fCost;
									leastWeightFromVisit = foundNodes [i].weightFromVisit;
									foundNodes [i].isVisited = true;
									foundNodes [i].parent = currentNode;
									moveToNode = foundNodes [i];
									direction = foundNodesDirection [i];
								}
							}
						}
					}
				}
				costLimit += 10;
			}
			if (moveToNode == previousNode) {
				moveToNode.weightFromVisit += currentNode.gCost;
			} else {
				currentNode.weightFromVisit = 0;
				previousNode.weightFromVisit = 0;
				previousNode.isPath = false;
			}
		}

		for (int i = 0; i < previousNode.neighbours.Length; i++) {
			previousNode.neighbours [i].isExpanded = false;
		}
		// previousMoveToNode = moveToNode;

		if (moveToNode != null) {
			moveToNode.weightFromGhost = 60;
			moveToNode.isPath = true;
		}

		return moveToNode;
	}

	void Move () {
		if (targetNode != currentNode && targetNode != null) {
			if (OverShotTarget ()) {
				//Debug.Log ("is Overshot");

				currentNode = targetNode;
				// Debug.Log ("currentNode: " + currentNode);
				transform.localPosition = currentNode.transform.position;

				targetNode = ChooseNextNode ();

				previousNode = currentNode;

				previousNode.weightFromGhost = 0;

				currentNode = null;
			} else {
				//Debug.Log ("not Overshot");

				transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
			} 
		}
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
}