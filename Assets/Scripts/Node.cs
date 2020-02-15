using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour {

	public Node[] neighbours;
	public Vector2[] validDirections;

	public float gCost;
	public float hCost;

	public float fCost {
		get {
			return gCost + hCost;
		}
	}

	public float redGCost;
	public float redHCost;

	public float redFCost {
		get {
			return redGCost + redHCost;
		}
	}

	public float pinkGCost;
	public float pinkHCost;

	public float pinkFCost {
		get {
			return pinkGCost + pinkHCost;
		}
	}

	public float blueGCost;
	public float blueHCost;

	public float blueFCost {
		get {
			return blueGCost + blueHCost;
		}
	}

	public float orangeGCost;
	public float orangeHCost;

	public float orangeFCost {
		get {
			return orangeGCost + orangeHCost;
		}
	}
		
	public float weightFromGhost;
	public float weightFromPoints;
	public float weightFromVisit;

	public float weight {
		get {
			return weightFromGhost + weightFromPoints;
		}
	}

	public Node parent;
	public bool isVisited = false;
	public bool isExpanded = false;
	public bool isPath = false;
	public bool nodeReset = false;

	public Text fCostText, hCostText, gCostText, weightText, parentText, nameText;
	// public LayerMask collisionMask;  

	// Use this for initialization
	void Start () {
		/*
		Vector2 origin = this.transform.position;
		Vector2 direction = this.transform.right;
		float distance = 3f;
	
		RaycastHit2D hit;
		hit = Physics2D.Raycast (origin, direction, distance, collisionMask);
		Debug.DrawLine (origin, origin + (direction * distance), Color.green);
		if (hit) {
			Debug.Log (hit.collider.name);
		}
		*/

		validDirections = new Vector2[neighbours.Length];

		for (int i = 0; i < neighbours.Length; i++) {
			Node neighbour = neighbours [i];
			Vector2 tempVector = neighbour.transform.position - transform.position;

			validDirections [i] = tempVector.normalized;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (isPath == true) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		} else {
			if (isPath == false && isExpanded == true) {
				GetComponent<SpriteRenderer> ().color = Color.yellow;
			} else if (isPath == false && isExpanded == false || nodeReset == true) {
				GetComponent<SpriteRenderer> ().color = Color.white;
			}
		}

		//*** use this for TBA
		isExpanded = false;
		isPath = false;

	}

	void OnMouseDown () {
		// Debug.Log ("Clicked");
		nameText.text = "Name\t: " + name;
		fCostText.text = "fCost\t: " + fCost.ToString ();
		gCostText.text = "gCost\t: " + gCost.ToString ();
		hCostText.text = "hCost\t: " + hCost.ToString ();
		weightText.text = "Weight\t: " + weight.ToString ();
		if (parent != null) {
			parentText.text = "Parent\t: " + parent.name;
		} else {
			parentText.text = "Parent\t: -";
		}
	}
}