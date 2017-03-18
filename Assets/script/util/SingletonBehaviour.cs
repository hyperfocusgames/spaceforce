using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {

	static T _instance;
	public static T instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<T>();
				if (_instance == null) {
					Debug.LogErrorFormat("Scene is missing instance of {}!", typeof(T).Name);
				}
			}
			return _instance;
		}
	}

}
