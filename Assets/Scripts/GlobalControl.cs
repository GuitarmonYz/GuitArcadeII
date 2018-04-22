using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalControl : MonoBehaviour {
	public static GlobalControl Instance;
	private GameObject safeDoor;
	// private GameObject player;
	public bool[] seen = new bool[]{false, false, false};
	public bool[] completed = new bool[]{false, false, false};
	public enum Stages {
		kRhythm, kCirFifth, kMelody
	};
	private Quaternion target;
	void Awake()
	{
		if (Instance == null) {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
	}
	
	void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       	Debug.Log(completed[(int)Stages.kCirFifth]);
		safeDoor = GameObject.FindGameObjectWithTag("safe_door");
		if (completed[(int)Stages.kCirFifth]) {
			safeDoor.GetComponent<Animator>().SetBool("Door", true);
		}
    }
	void Update() {
		// safeDoor.transform.rotation =  Quaternion.Slerp(safeDoor.transform.rotation, target, Time.deltaTime * 5f);;
	}
}
