using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour {

	public GameObject soundMenu;
	public GameObject confirmationMenu;
	public GameObject creditMenu;

	public void MouseOver () {
		FindObjectOfType<AudioManager> ().Play ("MouseOver");
	}

	public void PlayGame () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		SceneManager.LoadScene (1);
	}

	public void QuitGame () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		this.gameObject.SetActive (false);
		confirmationMenu.SetActive (true);
	}

	public void Options () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		this.gameObject.SetActive (false);
		soundMenu.SetActive (true);
	}

	public void SoundBackButton () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		soundMenu.SetActive (false);
		this.gameObject.SetActive (true);
	}

	public void ConfirmationYes () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		Debug.Log ("Application Quit");
		Application.Quit ();
	}

	public void ConfirmationNo () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		confirmationMenu.SetActive (false);
		this.gameObject.SetActive (true);
	}

	public void SoundOn () {
		AudioListener.volume = 1;
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
	}

	public void SoundOff () {
		AudioListener.volume = 0;
	}

	public void CreditButton () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		this.gameObject.SetActive (false);
		creditMenu.SetActive (true);
	}

	public void CreditBackButton () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		creditMenu.SetActive (false);
		this.gameObject.SetActive (true);
	}
}
