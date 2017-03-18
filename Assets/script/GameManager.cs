using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : SingletonBehaviour<GameManager> {

	public RectTransform pauseScreen;

	bool _isPaused = false;
	public bool isPaused {
		get {
			return _isPaused;
		}
		set {
			_isPaused = value;
			pauseScreen.gameObject.SetActive(value);
			Cursor.visible = value;
			if (value) {
				Time.timeScale = 0;
				Cursor.lockState = CursorLockMode.None;
			}
			else {
				Time.timeScale = 1;
				// unity pls fix your linux editor
				if (Application.platform != RuntimePlatform.LinuxEditor) {
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
		}
	}

	void Start() {
		isPaused = false;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			isPaused = !isPaused;
		}
	}

}
