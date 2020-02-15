using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {

	private static int boardWidth = 21;
	private static int boardHeight = 25;

	public float enemyReleaseTimer = 0;
	public float gameTimer = 0;

	public Text timeText, endTimeText;

	public GameObject[,] board = new GameObject[boardWidth, boardHeight];
	public Node[] allNode;

	public GameObject pauseMenu;
	public GameObject confirmationMenu;
	public GameObject startGameMenu;

	public bool gameStart = false;
	public bool debugMode = false;

	public bool disableInput = false;

	// Use this for initialization
	void Start () {
		Object[] objects = GameObject.FindObjectsOfType (typeof(GameObject));
		foreach (GameObject o in objects) {
			Vector2 pos = o.transform.position;

			// if (o.name != "Player" && o.tag != "Enemy" && o.tag != "Blocking" && o.tag != "Point" && o.tag != "UI" && o.tag != "PauseMenu") {
			if (o.tag == "Node") {
				board [(int)pos.x, (int)pos.y] = o;
				// Debug.Log ("object at: " + pos);
			} else {
				// Debug.Log ("Player at: " + pos);
			}
		}

		gameStart = false;
		if (startGameMenu != null) {
			startGameMenu.SetActive (true);
		}
	}

	// Update is called once per frame
	void Update () {
		// Time.timeScale = 0;
		enemyReleaseTimer += Time.deltaTime;

		gameTimer += Time.deltaTime;
		timeText.text = "Time: " + gameTimer.ToString ("N");
		endTimeText.text = timeText.text;

		if (disableInput == false && gameStart == false) {
			Time.timeScale = 0;
			startGameMenu.SetActive (true);
			if (disableInput == false && Input.GetKeyDown (KeyCode.Space)) {
				if (startGameMenu != null) {
					startGameMenu.SetActive (false);
				}
				Time.timeScale = 1;
				gameStart = true;
			}
		}

		if (disableInput == false && Input.GetKeyDown (KeyCode.Escape) && gameStart == true) {
			if (Time.timeScale == 1) {
				FindObjectOfType<AudioManager> ().Pause ("ThemeSong");
				FindObjectOfType<AudioManager> ().Play ("MenuClick");

				Time.timeScale = 0;
				pauseMenu.gameObject.SetActive (true);
				// Debug.Log ("The game is paused");
			} else {
				FindObjectOfType<AudioManager> ().Play ("MenuClick");
				FindObjectOfType<AudioManager> ().UnPause ("ThemeSong");

				Time.timeScale = 1;
				pauseMenu.gameObject.SetActive (false);
				if (confirmationMenu != null) {
					confirmationMenu.gameObject.SetActive (false);
				}
				// Debug.Log ("The game is resumed");
			}
		}

		if (debugMode == true) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				if (Time.timeScale == 1) {
					Time.timeScale = 0;
				} else {
					Time.timeScale = 1;
				}
			}
		}
	}

}