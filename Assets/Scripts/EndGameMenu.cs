using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour {

	public GameObject confirmationMenu;

	public void BackToMainMenu () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		this.gameObject.SetActive (false);
		confirmationMenu.SetActive (true);
	}

	public void ConfirmationNo () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		confirmationMenu.SetActive (false);
		this.gameObject.SetActive (true);
	}
}
