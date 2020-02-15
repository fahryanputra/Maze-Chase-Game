using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
	public Node[] parentNode = new Node[2];

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			for (int i = 0; i < parentNode.Length; i++) {
				if (parentNode [i] != null) {
					parentNode [i].weightFromPoints += 10;
				}
			}
			gameObject.SetActive (false);
		}
	}
}
