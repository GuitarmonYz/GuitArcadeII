using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayPopup : MonoBehaviour {
	public Text text;
	public float fadeTime = 10;
	private bool displayInfo = false;
	
	// Use this for initialization
	void Start () {
		text.color = Color.clear;
	}
	
	// Update is called once per frame
	void Update () {
		fadeText();
	}
	
	void OnMouseOver()
	{
		// Debug.Log("mouse over");
		displayInfo = true;
		if (this.gameObject.name == "Safe") {
			GlobalControl.Instance.seen[0] = true;
		}
		if (this.gameObject.name == "desktop_computer") {
			GlobalControl.Instance.seen[1] = true;
		}
	}
	
	void OnMouseExit()
	{
		displayInfo = false;	
		if (this.gameObject.name == "Safe") {
			GlobalControl.Instance.seen[0] = false;
		}
		if (this.gameObject.name == "desktop_computer") {
			GlobalControl.Instance.seen[1] = false;
		}
	}

	private void fadeText() {
		if (displayInfo) {
			text.color = Color.Lerp(text.color, Color.black, fadeTime * Time.deltaTime);
		} else {
			text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
		}
	}
}
