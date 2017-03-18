using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : SingletonBehaviour<GameManager> {

	public RectTransform pauseMenu;

	bool _isPaused = false;
	public bool isPaused {
		get {
			return _isPaused;
		}
		set {
			_isPaused = value;
			pauseMenu.gameObject.SetActive(value);
			// Cursor.visible = value;
			if (value) {
				Time.timeScale = 0;
				// Cursor.lockState = CursorLockMode.None;
			}
			else {
				Time.timeScale = 1;
				// Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			isPaused = !isPaused;
		}
	}

}
