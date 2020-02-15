using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour {
	public Button onButton;
	public Button offButton;

	// Use this for initialization
	void Start () {
		onButton.Select ();
		onButton.OnSelect (null);
	}
	
	// Update is called once per frame
	void Update () {
		if (AudioListener.volume == 0) {
			offButton.Select ();
			offButton.OnSelect (null);
		} else {
			onButton.Select ();
			onButton.OnSelect (null);
		}
	}
}
