using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkTBAPathfinding : MonoBehaviour {

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

	public EnemyType enemyType = EnemyType.Pink;

	[System.NonSerialized]
	public List<Node> path = new List<Node> ();

	[System.NonSerialized]
	public bool isInEnemySpawn, isTraced, isSolutionFound;

	public int redReleaseTimer = 2; 
	public int pinkReleaseTimer = 7;
	public int blueReleaseTimer = 12;
	public int orangeReleaseTimer = 17;

	public int searchLimit = 3;

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

		previousNode = currentNode;

		if (!isInEnemySpawn) {
			Move ();
		}
	}
	
	// Update is called once per frame
	void Update () { 
		if (GetNodeAtPosition (transform.localPosition) != null) {
			// Debug.Log ("is not null");
			TBAFindPath ();

		}

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
		Vector2 playerOrientation = player.GetComponent<TBAPlayerController> ().orientation;

		int playerPosX = Mathf.RoundToInt (playerPosition.x);
		int playerPosY = Mathf.RoundToInt (playerPosition.y);

		Vector2 playerTile = new Vector2 (playerPosX, playerPosY);
		Vector2 targetTile = playerTile + (2 * playerOrientation);
		// Debug.Log ("Pink Target: " + targetTile);

		return targetTile;
	}

	Vector2 GetBlueTargetTile () {
		Vector2 playerOrientation = player.GetComponent<TBAPlayerController> ().orientation;

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

		if (distance >= 60) {
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

	void TBAFindPath () {
		bool solutionFound = false;
		bool solutionFoundAndTraced = false;
		bool doneTrace = true;

		if (!solutionFound) {
			FindPath (GetNodeAtPosition (transform.localPosition).transform.position, GetTargetTile (), searchLimit);// Mathf.RoundToInt (GetDistance (transform.localPosition, player.transform.localPosition) / 30));
			solutionFound = isSolutionFound;
			// Debug.Log ("solutionFound: " + solutionFound);
		}

		if (!solutionFoundAndTraced) {
			doneTrace = isTraced;
			// Debug.Log ("doneTrace: " + doneTrace);

			if (doneTrace) {
				if (path.Contains (GetNodeAtPosition (player.transform.position))) {
					solutionFoundAndTraced = true;
					// Debug.Log ("solutionFoundAndTraced: " + solutionFoundAndTraced);
				}
			}
		}
	}

	void FindPath (Vector2 startPos, Vector2 targetPos, int searchLimit) {
		// Debug.Log ("FindPath is called");

		Node startNode = GetNodeAtPosition (startPos);
		// Debug.Log ("startPos: " + startPos);

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

		Node targetNode = GetNodeAtPosition (targetPos);
		// Debug.Log ("targetPos: " + targetPos);
		// Debug.Log ("targetNode: " + targetNode);

		List<Node> openSet = new List<Node> ();

		HashSet<Node> closeSet = new HashSet<Node> ();

		int searchLimitCount = 0;
		// int searchLimit = Mathf.RoundToInt (GetDistance (transform.localPosition, player.transform.localPosition) / 30); 

		if (startNode != null && targetNode != null) {
			openSet.Add (startNode);
			// Debug.Log ("Starting openSet: " + startNode);

			while (openSet.Count > 0) {
				// Debug.Log ("openSet.Count > 0");

				Node currentNode = openSet [0];
				float leastWeight = 1000f;

				for (int i = 1; i < openSet.Count; i++) {
					if ((openSet [i].pinkFCost < currentNode.pinkFCost || openSet [i].pinkFCost == currentNode.pinkFCost) && openSet [i].pinkHCost < currentNode.pinkHCost) {
						if (openSet [i].weight < leastWeight) {
							leastWeight = openSet [i].weight;
							currentNode = openSet [i];
							// Debug.Log ("currentNode: " + currentNode);
						}
					}
				}

				openSet.Remove (currentNode);
				closeSet.Add (currentNode);
				// Debug.Log ("closeSet: " + closeSet);

				/*
				if (searchLimitCount == searchLimit) {
					targetNode = currentNode;
				}
				*/

				if (searchLimitCount == searchLimit) {
					// Debug.Log ("currentNode = targetNode");
					RetracePath (startNode, currentNode);

					return;
				} else if (currentNode == targetNode) { 
					
					RetracePath (startNode, targetNode);
					isSolutionFound = true;

					return;
				}

				searchLimitCount++;

				for (int i = 0; i < currentNode.neighbours.Length; i++) {
					Node neighbour = currentNode.neighbours [i];
					// Debug.Log ("currentNode.neighbours : " + neighbour);

					if (closeSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCost = Mathf.RoundToInt (GetDistance (currentNode.transform.position, neighbour.transform.position));

					if (newMovementCost < neighbour.pinkGCost || !openSet.Contains (neighbour)) {
						neighbour.pinkGCost = newMovementCost;
						neighbour.pinkHCost = GetDistance (neighbour.transform.position, targetNode.transform.position);

						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
							// Debug.Log ("openSet: " + openSet);
						}
					}
				}
			}
		}
	}

	public void RetracePath (Node startNode, Node endNode) {
		// List<Node> foundPath = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();

		isTraced = true;
		/*
		for (int i = 0; i < path.Count; i++) {
			Debug.Log ("path is: " + path [i] + " at " + path [i].transform.position);
		}
		*/
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

	float GetDistance (Vector2 nodeA, Vector2 nodeB) {
		float dstX = Mathf.Abs (nodeA.x - nodeB.x);
		float dstY = Mathf.Abs (nodeA.y - nodeB.y);

		return (10 * dstX) + (10 * dstY);
	}
		
	Node ChooseNextNode () {
		Vector2 targetTile = Vector2.zero;

		targetTile = GetTargetTile ();

		Node moveToNode = null;

		Node[] foundNodes = new Node[4];
		Vector2[] foundNodesDirection = new Vector2[4];

		int nodeCounter = 0;

		for (int i = 0; i < currentNode.neighbours.Length; i++) {
			if (path.Contains (currentNode.neighbours [i])) {
				// Debug.Log ("path contains: " + currentNode.neighbours [i]);
				foundNodes [nodeCounter] = currentNode.neighbours [i];
				// Debug.Log (foundNodes [i]);	
				foundNodesDirection [nodeCounter] = currentNode.validDirections [i];
				// Debug.Log (foundNodes [i]);
				nodeCounter++;
			}
		}

		// Debug.Log ("foundNodes.Length: " + foundNodes.Length);

		if (foundNodes.Length == 1) {
			// Debug.Log ("founNodes.length: 1");
			moveToNode = foundNodes [0];
			direction = foundNodesDirection [0];
		}

		Node backupNode = null;
		Vector2 backupDirection = Vector2.zero;

		if (foundNodes.Length > 1) {
			// Debug.Log ("foundNodes.length: more than 1");
			float leastDistance = 1000f;
			float leastpinkFCost = 1000f;

			for (int i = 0; i < foundNodes.Length; i++) {
				if (foundNodesDirection [i] != Vector2.zero) {
					if (foundNodes [i] != previousNode) {
						float distance = GetDistance (foundNodes [i].transform.position, targetTile);
		
						if (foundNodes [i].pinkFCost < leastpinkFCost || distance < leastDistance) {
							leastpinkFCost = foundNodes [i].pinkFCost;
							leastDistance = distance;
							moveToNode = foundNodes [i];
							direction = foundNodesDirection [i];
						}
					} else {
						backupNode = foundNodes [i];
						backupDirection = foundNodesDirection [i];
					}
				}
			}
		}

		if (moveToNode != null) {
			moveToNode.weightFromGhost = 30;
		} else {
			moveToNode = backupNode;
			direction = backupDirection;
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