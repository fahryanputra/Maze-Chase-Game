using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlgorithmMenu : MonoBehaviour {

	public void MouseOver () {
		FindObjectOfType<AudioManager> ().Play ("MouseOver");
	}

	public void PlayAStar () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		FindObjectOfType<AudioManager> ().Play ("ThemeSong");
		SceneManager.LoadScene (2);
	}

	public void PlayTBAStar () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		FindObjectOfType<AudioManager> ().Play ("ThemeSong");
		SceneManager.LoadScene (3);
	}

	public void PlayRIBS () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		FindObjectOfType<AudioManager> ().Play ("ThemeSong");
		SceneManager.LoadScene (4);
	}

	public void BackToMainMenu () {
		FindObjectOfType<AudioManager> ().Play ("MenuClick");
		SceneManager.LoadScene (0);
	}

}
