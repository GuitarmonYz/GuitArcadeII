using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditionalController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalControl.Instance.seen[(int)GlobalControl.Stages.kRhythm]) {
			if (Input.GetKey("f")) {
				SceneManager.LoadScene((int)GlobalControl.Stages.kRhythm+1);
			}
		} else if (GlobalControl.Instance.seen[(int)GlobalControl.Stages.kCirFifth] && GlobalControl.Instance.completed[(int)GlobalControl.Stages.kRhythm]) {
			if (Input.GetKey("f")) {
				SceneManager.LoadScene((int)GlobalControl.Stages.kCirFifth+1);
			}
		}
		
	}
}
