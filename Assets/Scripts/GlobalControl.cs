using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {
	public static GlobalControl Instance;
	// Use this for initialization
	public bool[] seen = new bool[]{false, false};
	public bool[] completed = new bool[]{false, false};

	void Awake()
	{
		if (Instance == null) {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
	}
}
