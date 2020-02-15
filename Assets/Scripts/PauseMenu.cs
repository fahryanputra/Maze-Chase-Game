using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public GameObject confirmationMenu;
	public GameObject soundMenu;

	public void MouseOver () {
		FindObjectOfType<AudioManager> ().Play ("MouseOver");
	}

	public void RestartGame () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		FindObjectOfType<AudioManager> ().Play ("ThemeSong");
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void BackToMainMenu () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		this.gameObject.SetActive (false);
		confirmationMenu.SetActive (true);
	}

	public void ConfirmationYes () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		SceneManager.LoadScene (0);
	}

	public void ConfirmationNo () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		confirmationMenu.SetActive (false);
		this.gameObject.SetActive (true);
	}

	public void ResumeGame () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		FindObjectOfType<AudioManager> ().UnPause ("ThemeSong");
		this.gameObject.SetActive (false);
		Time.timeScale = 1;
	}

	public void SoundMenu () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		this.gameObject.SetActive (false);
		soundMenu.SetActive (true);
	}

	public void SoundBackButton () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		soundMenu.SetActive (false);
		this.gameObject.SetActive (true);
	}

	public void SoundOn () {
		AudioListener.volume = 1;
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
	}

	public void SoundOff () {
		AudioListener.volume = 0;
	}
}
