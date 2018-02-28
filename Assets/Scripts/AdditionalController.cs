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
		if (GlobalControl.Instance.seen[0]) {
			if (Input.GetKey("f")) {
				SceneManager.LoadScene(1);
			}
		} else if (GlobalControl.Instance.seen[1]) {
			if (Input.GetKey("f")) {
				SceneManager.LoadScene(2);
			}
		}
	}
}
